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
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.Services.P_ProcedureServices.Services;

public class RequestEquipmentOrderService : IRequestEquipmentOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;

    public RequestEquipmentOrderService(ApplicationDbContext context,
        IMapper mapper,
        IProcedureHelperService procedureHelperService,
        ICompanyService companyService,
        IConverter converterPDF,
        IOptions<AppSettings> appSettings,
        IProcedureExportHelper procedureExportHelper)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _appSettings = appSettings.Value;
        _procedureExportHelper = procedureExportHelper;
    }

    public async Task<PagingResult<RequestEquipmentOrderPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.RequestEquipmentOrders
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER) && x.log.UserId == userId && !x.procedure.IsFinished
            && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);


        var datas = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var listOut = new List<RequestEquipmentOrderPagingModel>();
        foreach (var item in datas)
        {
            var itemOut = _mapper.Map<RequestEquipmentOrderPagingModel>(item);
            itemOut.UserName = await _context.Users.Where(x => x.Id == itemOut.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
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

        return new PagingResult<RequestEquipmentOrderPagingModel>
        {
            Data = listOut,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<RequestEquipmentOrderModel> GetDetail(int id)
    {
        var item = await _context.RequestEquipmentOrders.FirstOrDefaultAsync(x => x.Id == id);
        var itemOut = _mapper.Map<RequestEquipmentOrderModel>(item);
        if (!string.IsNullOrEmpty(item.FileStr))
        {
            itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
        }

        itemOut.Items = await _context.RequestEquipmentOrderDetails.Where(x => x.RequestEquipmentOrderId == id)
                        .Select(x => _mapper.Map<RequestEquipmentOrderDetailModel>(x)).ToListAsync();

        return itemOut;
    }

    public async Task Create(int requestEquipmentId)
    {
        var isvalidate = await _context.RequestEquipmentOrders.AnyAsync(x => x.RequestEquipmentId == requestEquipmentId);
        if (isvalidate)
        {
            throw new ErrorException("Quy trình đã tồn tại");
        }

        var requestEquipment = await _context.RequestEquipments.FindAsync(requestEquipmentId);
        var procedure = new RequestEquipmentOrder()
        {
            ProcedureNumber = await GetProcedureNumber(),
            RequestEquipmentId = requestEquipmentId,
            RequestEquipmentCode = requestEquipment.Code,
            Date = DateTime.Now,
            UserId = requestEquipment.UserCreated
        };

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = requestEquipment.UserCreated;
        procedure.UserUpdated = requestEquipment.UserCreated;
        procedure.IsFinished = false;
        await _context.RequestEquipmentOrders.AddAsync(procedure);
        await _context.SaveChangesAsync();

        var details = await _context.RequestEquipmentDetails.Where(x => x.RequestEquipmentId == requestEquipmentId).Select(x => _mapper.Map<RequestEquipmentOrderDetail>(x)).ToListAsync();
        details = details.ConvertAll(x =>
        {
            x.Id = 0;
            x.RequestEquipmentOrderId = procedure.Id;
            return x;
        });

        await _context.RequestEquipmentOrderDetails.AddRangeAsync(details);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER), status.Id, procedure.Id, requestEquipment.UserCreated, procedure.ProcedureNumber);
    }

    public async Task Update(RequestEquipmentOrderModel form, int userId)
    {
        var procedure = await _context.RequestEquipmentOrders.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        procedure.Note = form.Note;
        procedure.CustomerId = form.CustomerId ?? 0;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        if (form.Files != null)
        {
            procedure.FileStr = JsonConvert.SerializeObject(form.Files);
        }

        _context.RequestEquipmentOrders.Update(procedure);

        var detailIds = form.Items.Select(X => X.Id).ToList();
        // delete item detail
        var detailDels = await _context.RequestEquipmentOrderDetails.Where(X => X.RequestEquipmentOrderId == form.Id && !detailIds.Contains(X.Id)).ToListAsync();
        if (detailDels.Any())
        {
            _context.RequestEquipmentOrderDetails.RemoveRange(detailDels);
        }

        var details = form.Items.Select(x => _mapper.Map<RequestEquipmentOrderDetail>(x)).ToList();
        details = details.ConvertAll(x =>
        {
            x.RequestEquipmentOrderId = procedure.Id;
            return x;
        });
        _context.RequestEquipmentOrderDetails.UpdateRange(details);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.RequestEquipmentOrders.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);

        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            procedure.IsFinished = status.IsFinish;
            procedure.Code = await GetCodeAsync();
        }

        _context.RequestEquipmentOrders.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER), status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.RequestEquipmentOrders.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.RequestEquipmentOrders.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        var item = await _context.RequestEquipmentOrders.FindAsync(id);
        if (item != null)
        {
            _context.RequestEquipmentOrders.Remove(item);
            var details = await _context.RequestEquipmentOrderDetails.FindAsync(id);
            _context.RequestEquipmentOrderDetails.RemoveRange(details);

            await _context.SaveChangesAsync();
        }
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));

    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.RequestEquipmentOrders.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER)}-{procedureNumber}";
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.RequestEquipmentOrders.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "DMH");
    }

    public async Task<string> Export(int requestEquipmentOrderId)
    {
        var payment = await _context.RequestEquipmentOrders.FindAsync(requestEquipmentOrderId);
        if (!payment.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        var company = await _companyService.GetCompany();

        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var details = await _context.RequestEquipmentOrderDetails.Where(x => x.RequestEquipmentOrderId == requestEquipmentOrderId).ToListAsync();

        var resultSignHtml = await _procedureExportHelper.SignPlaceSameTr(requestEquipmentOrderId, nameof(ProcedureEnum.REQUEST_EQUIPMENT_ORDER));

        var resultHtml = GenerateExportTable(details, soThapPhan);

        var paymentDate = payment.Date;
        var totalAmount = details.Sum(x => x.Quantity * x.UnitPrice);
        var values = new List<(string FieldName, string FieldValue)>
        {
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("TotalAmount", totalAmount.FormatAmount(soThapPhan)),
            ("TotalAmountText", totalAmount.ConvertFromDecimal()),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("Note", payment.Note),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "DonMuaHang.html"
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
            "DonMuaHang"
        );
    }
    private string GenerateExportTable(List<RequestEquipmentOrderDetail> expenditurePlanDetails, string soThapPhan)
    {
        var resultHtml = new StringBuilder();
        foreach (var expenditurePlanDetail in expenditurePlanDetails)
        {
            resultHtml.Append(GenerateExportRow(expenditurePlanDetail, soThapPhan));
        }

        return resultHtml.ToString();
    }
    private string GenerateExportRow(RequestEquipmentOrderDetail expenditurePlanDetail, string soThapPhan)
    {
        string txt = $@"<tr>
                                <td class='txt-left'>{expenditurePlanDetail.GoodName}</td>
                                <td class='txt-left'>{expenditurePlanDetail.GoodName}</td>                                
                                <td class='txt-left'>{expenditurePlanDetail.GoodUnit}</td>
                                <td class='txt-right'>{expenditurePlanDetail.Quantity.FormatAmount(soThapPhan)}</td>                                
                                <td class='txt-right'>{expenditurePlanDetail.UnitPrice.FormatAmount(soThapPhan)}</td>
                                <td class='txt-right'>{(expenditurePlanDetail.UnitPrice * expenditurePlanDetail.Quantity).FormatAmount(soThapPhan)}</td>
                            </tr>";
        return txt;
    }

    public async Task<IEnumerable<SelectListModel>> GetList()
    {
        return await _context.RequestEquipmentOrders.Where(x => x.IsFinished).Select(x => new SelectListModel
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Note
        }).ToListAsync();
    }
}