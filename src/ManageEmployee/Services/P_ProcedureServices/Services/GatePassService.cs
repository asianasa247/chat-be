using AutoMapper;
using Common.Constants;
using DinkToPdf.Contracts;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Services.P_ProcedureServices.Services;

public class GatePassService : IGatePassService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;

    public GatePassService(ApplicationDbContext context, IMapper mapper,
        IProcedureHelperService procedureHelperService,
        ICompanyService companyService,
        IConverter converterPDF,
        IOptions<AppSettings> appSettings, IProcedureExportHelper procedureExportHelper)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _appSettings = appSettings.Value;
        _procedureExportHelper = procedureExportHelper;
    }

    public async Task<PagingResult<GatePassPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        if (param.ToAt.HasValue)
            param.ToAt = param.ToAt.Value.Date.AddDays(1).AddSeconds(-1);

        var query = _context.GatePasses
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GATE_PASS));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.GATE_PASS));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId));

        if (param.StatusTab == ProduceProductStatusTab.Approved)
        {
            query = query
            .Join(_context.ProcedureLogs,
                    b => b.Id,
                    d => d.ProcedureId,
                    (b, d) => new
                    {
                        procedure = b,
                        log = d
                    })
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.GATE_PASS) && x.log.UserId == userId
                            && x.log.NotAcceptCount == 0 && !x.procedure.IsFinished)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var datas = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var listOut = new List<GatePassPagingModel>();
        foreach (var item in datas)
        {
            var itemOut = _mapper.Map<GatePassPagingModel>(item);
            itemOut.ShoulDelete = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending
                                    && itemOut.ProcedureStatusName == startStatus.P_StatusName;
            itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;
            if (item.IsFinished)
            {
                itemOut.ProcedureNumber = item.Code;
            }
            listOut.Add(itemOut);
        }
        var totalItem = await query.CountAsync();
        return new PagingResult<GatePassPagingModel>
        {
            Data = listOut,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<GatePassModel> GetDetail(int id)
    {
        var item = await _context.GatePasses.FirstOrDefaultAsync(x => x.Id == id);
        var itemOut = _mapper.Map<GatePassModel>(item);
        itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);

        return itemOut;
    }

    public async Task Create(GatePassModel form, int userId)
    {
        var procedure = _mapper.Map<GatePass>(form);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GATE_PASS));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;
        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.IsFinished = false;

        await _context.GatePasses.AddAsync(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.GATE_PASS), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }

    public async Task Update(GatePassModel form, int userId)
    {
        var procedure = await _context.GatePasses.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        procedure.IsSpecial = form.IsSpecial;
        procedure.CarName = form.CarName;
        procedure.Content = form.Content;
        procedure.Date = form.Date;
        procedure.Local = form.Local;
        procedure.Note = form.Note;
        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        _context.GatePasses.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.GatePasses.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId, procedure.IsSpecial);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            procedure.IsFinished = status.IsFinish;
            procedure.Code = await GetCodeAsync();
        }

        _context.GatePasses.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.GATE_PASS), status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.GATE_PASS), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.GatePasses.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GATE_PASS));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.GatePasses.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.GATE_PASS), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.GATE_PASS));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        var item = await _context.GatePasses.FindAsync(id);
        if (item != null)
        {
            _context.GatePasses.Remove(item);
            await _context.SaveChangesAsync();
        }
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.GATE_PASS));

    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.GatePasses.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{nameof(ProcedureEnum.GATE_PASS)}-{procedureNumber}";
    }

    public async Task<ProcedureCheckButton> CheckButton(int id, int userId)
    {
        var itemOut = new ProcedureCheckButton()
        {
            IsAdd = false,
            IsAccept = false,
            IsDelete = false,
            IsSave = false,
        };

        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            throw new ErrorException(ErrorMessages.UserNotFound);
        }
        var userRoleIds = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();
        var step = new P_ProcedureStatusStep();

        if (id == 0)
        {
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.GATE_PASS));
            step = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdFrom == status.Id);
            itemOut.IsAdd = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));
            return itemOut;
        }

        var procedure = await _context.AdvancePayments.FindAsync(id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        if (procedure.IsFinished)
        {
            return itemOut;
        }

        step = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdFrom == procedure.ProcedureStatusId);

        if (step is null)
        {
            throw new ErrorException(ErrorMessages.ProcedureStepNotFound);
        }
        itemOut.IsDelete = step.IsInit;

        itemOut.IsSave = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));

        itemOut.IsAccept = procedure.UserId == userId && step.IsInit ||
                                await _context.P_ProcedureStatusRole
                                .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdTo
                                            && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));

        return itemOut;
    }

    public async Task<string> Export(int id)
    {
        var gatepass = await _context.GatePasses.FindAsync(id);
        if (gatepass == null || !gatepass.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }
        var company = await _companyService.GetCompany();
        var soThapPhan = "N" + company.MethodCalcExportPrice;
        string _template = "GiayRaCong.html";
        var folderPath = @"Uploads\Html\ProduceProduct";
        var path = Path.Combine(Directory.GetCurrentDirectory(), folderPath, _template);
        var allText = File.ReadAllText(path);
        var resultHTML = string.Empty; 
        var resultSignHTML = string.Empty;

        resultSignHTML = await _procedureExportHelper.SignPlaceLastest(id, nameof(ProcedureEnum.GATE_PASS));
        var userAcceptIds = await _context.ProcedureLogs.Where(x => x.ProcedureCode == nameof(ProcedureEnum.GATE_PASS)
                            && x.ProcedureId == id && x.NotAcceptCount == 0).Select(x => x.UserId).Distinct().ToListAsync();
        var userRoleIds = await _context.UserRoles.Where(x => gatepass.IsSpecial ?
                                                            x.Code == UserRoleConst.AdminBranch || x.Code == UserRoleConst.SuperAdmin
                                                            : x.Code == UserRoleConst.TruongPhong).Select(x => x.Id).ToListAsync();

        var users = await _context.Users.Where(x => userAcceptIds.Contains(x.Id)).ToListAsync();
        User userFind = null;
        foreach (var user in users)
        {
            var userRoleIdChecks = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();
            if (userRoleIdChecks.Any(x => userRoleIds.Contains(x)))
            {
                userFind = user;
            }
            if (userFind != null)
            {
                break;
            }
        }
        var userPosition = userFind != null ? 
                            await _context.PositionDetails
                                           .FirstOrDefaultAsync(x => 
                                                x.Id == userFind.PositionDetailId
                                            )
                            : null;

        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "CompanyName", company.Name },
                { "CompanyAddress", company.Address },
                { "Local", gatepass.Local },
                { "Hour", gatepass.Date.Hour.ToString() },
                { "Day", gatepass.Date.Day.ToString() },
                { "Month", gatepass.Date.Month.ToString() },
                { "Year", gatepass.Date.Year.ToString() },
                { "CarName", gatepass.CarName },
                { "Note", gatepass.Note },
                { "UserSign", $"{_appSettings.UrlHost}{userFind?.SignFile ?? string.Empty}" },
                { "UserName", userFind?.FullName ?? string.Empty },
                { "MST", company.MST},
                { "Phone", company.Phone},
                { "CompanyImage", $"{_appSettings.UrlHost}{company?.FileLogo}"},
                { "UserPosition", $"{userPosition?.Name?.ToUpper() ?? string.Empty}"},
                { "ProduceCode", $"{gatepass.Code}"},
            };

        v_dicFixed.Keys.ToList().ForEach(x => allText = allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
        allText = allText.Replace("##SIGN_REPLACE_PLACE##", resultSignHTML);

        return ExcelHelpers.ConvertUseDink(allText, _converterPDF, Directory.GetCurrentDirectory(), "GiayRaCong");
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;
        var codeNumber = await _context.GatePasses.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "RC");
    }

}