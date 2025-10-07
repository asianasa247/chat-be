using AutoMapper;
using Common.Extensions;
using DinkToPdf.Contracts;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Services.P_ProcedureServices.Services;

public class AdvancePaymentService : IAdvancePaymentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly AppSettings _appSettings;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly IProcedureExportHelper _procedureExportHelper;

    public AdvancePaymentService(ApplicationDbContext context, IMapper mapper,
        IProcedureHelperService procedureHelperService,
        IOptions<AppSettings> appSettings, ICompanyService companyService, IConverter converterPDF, IProcedureExportHelper procedureExportHelper)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _appSettings = appSettings.Value;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _procedureExportHelper = procedureExportHelper;
    }

    public async Task<PagingResult<AdvancePaymentPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.AdvancePayments
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ADVANCE_PAYMENT));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.ADVANCE_PAYMENT));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId)
        || param.StatusTab == ProduceProductStatusTab.Pending && x.UserId == userId && x.ProcedureStatusId == startStatus.Id);

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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.ADVANCE_PAYMENT) && x.log.UserId == userId
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }

        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        switch (param.StatusTab)
        {
            case ProduceProductStatusTab.Done:
                query = query.Where(x => x.IsFinished && !x.IsDone);
                break;
            case ProduceProductStatusTab.Finish:
                query = query.Where(x => x.IsDone);
                break;
            case ProduceProductStatusTab.Approved:
                query = query.Where(x => !x.IsFinished);
                break;
        }

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var itemOuts = new List<AdvancePaymentPagingModel>();
        foreach (var item in data)
        {
            var itemOut = _mapper.Map<AdvancePaymentPagingModel>(item);
            itemOut.ShoulDelete = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending
                                    && itemOut.ProcedureStatusName == startStatus.P_StatusName;
            itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;

            if (!string.IsNullOrEmpty(item.FileStr))
            {
                itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
            }
            if (!string.IsNullOrEmpty(item.SettlementFileStr))
            {
                itemOut.SettlementFile = JsonConvert.DeserializeObject<FileDetailModel>(item.SettlementFileStr);
            }
            if (item.IsFinished)
            {
                itemOut.ProcedureNumber = item.Code;
            }
            itemOuts.Add(itemOut);
        }
        var totalItem = await query.CountAsync();

        return new PagingResult<AdvancePaymentPagingModel>
        {
            Data = itemOuts,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<AdvancePaymentModel> GetDetail(int id)
    {
        var item = await _context.AdvancePayments.Where(x => x.Id == id).FirstOrDefaultAsync();
        var itemOut = _mapper.Map<AdvancePaymentModel>(item);
        if (!string.IsNullOrEmpty(item.FileStr))
        {
            itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
        }
        if (!string.IsNullOrEmpty(item.SettlementFileStr))
        {
            itemOut.SettlementFile = JsonConvert.DeserializeObject<FileDetailModel>(item.SettlementFileStr);
        }

        itemOut.Items = await _context.AdvancePaymentDetails.Where(x => x.AdvancePaymentId == id)
            .Select(x => new AdvancePaymentDetailModel
            {
                Id = x.Id,
                Amount = x.Amount,
                Note = x.Note,
            }).ToListAsync();
        return itemOut;
    }

    public async Task Create(AdvancePaymentModel form, int userId)
    {
        var procedure = _mapper.Map<AdvancePayment>(form);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ADVANCE_PAYMENT));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;
        procedure.UserId = userId;
        procedure.Date = DateTime.Now;
        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.Amount = form.Items.Sum(x => x.Amount);
        procedure.IsFinished = false;
        procedure.IsImmediate = form.IsImmediate;

        await _context.AdvancePayments.AddAsync(procedure);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, procedure.Id);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.ADVANCE_PAYMENT), status.Id, procedure.Id, userId, procedure.ProcedureNumber);

    }

    public async Task Update(AdvancePaymentModel form, int userId)
    {
        var procedure = await _context.AdvancePayments.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        if (procedure.IsFinished)
        {
            if (form.SettlementFile != null)
            {
                procedure.SettlementFileStr = JsonConvert.SerializeObject(form.SettlementFile);
                procedure.IsDone = true;
                _context.AdvancePayments.Update(procedure);
                await _context.SaveChangesAsync();
            }

            return;
        }
        procedure.Note = form.Note;
        procedure.Date = form.Date;
        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        procedure.Amount = form.Items.Sum(x => x.Amount);
        procedure.IsImmediate = form.IsImmediate;

        _context.AdvancePayments.Update(procedure);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, form.Id);
    }

    private async Task AddDetail(List<AdvancePaymentDetailModel> items, int id)
    {
        var detailDels = await _context.AdvancePaymentDetails.Where(x => x.AdvancePaymentId == id).ToListAsync();
        if (detailDels.Any())
        {
            _context.AdvancePaymentDetails.RemoveRange(detailDels);
        }

        var detailAdds = items.Select(x => new AdvancePaymentDetail
        {
            AdvancePaymentId = id,
            Amount = x.Amount,
            Note = x.Note,
        }).ToList();
        await _context.AdvancePaymentDetails.AddRangeAsync(detailAdds);
        await _context.SaveChangesAsync();
    }
    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.AdvancePayments.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
        if (status.ProcedureConditionCode == nameof(ProcedureOrderProduceProductConditionEnum.SameDepartment))
        {
            var userCreatedDepartmentId = await _context.Users.Where(x => x.Id == procedure.UserCreated).Select(x => x.DepartmentId).FirstOrDefaultAsync();
            var checkSameDepartment = await _context.Users.AnyAsync(x => x.Id == userId && x.DepartmentId == userCreatedDepartmentId);
            if (!checkSameDepartment)
            {
                throw new ErrorException(ErrorMessages.ProcedureNotNotSameDepartment);
            }
        }

        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            procedure.IsFinished = status.IsFinish;
            procedure.Code = await GetCodeAsync();
        }

        _context.AdvancePayments.Update(procedure);
        await _context.SaveChangesAsync();
        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.ADVANCE_PAYMENT), status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.ADVANCE_PAYMENT), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);
    }

    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.ADVANCE_PAYMENT));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }

        var item = await _context.AdvancePayments.FindAsync(id);

        if (item != null)
        {
            _context.AdvancePayments.Remove(item);
            await _context.SaveChangesAsync();
        }
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.ADVANCE_PAYMENT));

    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.AdvancePayments.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{nameof(ProcedureEnum.ADVANCE_PAYMENT)}-{procedureNumber}";
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
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ADVANCE_PAYMENT));
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

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;
        var codeNumber = await _context.AdvancePayments.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "TU");
    }

    public async Task<string> Export(int advancePaymentId)
    {
        var advancePayment = await _context.AdvancePayments.FindAsync(advancePaymentId);
        if (!advancePayment.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        var company = await _companyService.GetCompany();

        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var details = await _context.AdvancePaymentDetails.Where(x => x.AdvancePaymentId == advancePaymentId).ToListAsync();

        var resultSignHtml = await _procedureExportHelper.SignPlace(advancePaymentId, nameof(ProcedureEnum.ADVANCE_PAYMENT));

        var resultHtml = GenerateExportTable(details, soThapPhan);

        var paymentDate = advancePayment.DatePayment;
        var totalAmount = details.Sum(x => x.Amount);
        var values = new List<(string FieldName, string FieldValue)>
        {
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("TotalAmount", totalAmount.FormatAmount(soThapPhan)),
            ("TotalAmountText", totalAmount.ConvertFromDecimal()),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("Note", advancePayment.Note),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
            ("ProduceCode", string.IsNullOrEmpty(advancePayment.Code) ? string.Empty : $"Số: {advancePayment.Code}"),

        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "PhieuDeNghiTamUng.html"
        );

        var allTextBuilder = new StringBuilder(allText);

        values.ForEach(x =>
        {
            allTextBuilder.Replace("{{{" + x.FieldName + "}}}", x.FieldValue);
        });

        allTextBuilder.Replace("##SIGN_REPLACE_PLACE##", resultSignHtml);
        allTextBuilder.Replace("##REPLACE_PLACE##", resultHtml);

        return ExcelHelpers.ConvertUseDink(
            allTextBuilder.ToString(),
            _converterPDF,
            Directory.GetCurrentDirectory(),
            "PhieuDeNghiTamUng"
        );
    }
    private string GenerateExportTable(List<AdvancePaymentDetail> details, string soThapPhan)
    {
        var resultHtml = new StringBuilder();
        for (int i = 1; i <= details.Count; i++)
        {
            var paymentDetail = details[i - 1];
            resultHtml.Append(GenerateExportRow(i, paymentDetail, soThapPhan));
        }

        return resultHtml.ToString();
    }

    private string GenerateExportRow(int rowIndex, AdvancePaymentDetail advancePaymentDetail, string soThapPhan)
    {
        string txt = $@"<tr>
                                <td class='txt-center'>{rowIndex}</td>
                                <td>{advancePaymentDetail.Note}</td>
                                <td class='txt-right'>{advancePaymentDetail.Amount.FormatAmount(soThapPhan)}</td>
                                <td></td>
                            </tr>";
        return txt;
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.AdvancePayments.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ADVANCE_PAYMENT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.AdvancePayments.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.ADVANCE_PAYMENT), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task<IEnumerable<SelectListModel>> GetList()
    {
        return await _context.AdvancePayments.Where(x => x.IsFinished).Select(x => new SelectListModel
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Note
        }).ToListAsync();
    }
}