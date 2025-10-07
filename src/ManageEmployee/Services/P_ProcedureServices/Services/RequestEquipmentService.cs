using ManageEmployee.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Extends;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Options;
using Common.Extensions;
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
using Microsoft.Extensions.Primitives;

namespace ManageEmployee.Services.P_ProcedureServices.Services;
public class RequestEquipmentService : IRequestEquipmentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly IRequestEquipmentOrderService _requestEquipmentOrderService;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;

    public RequestEquipmentService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService, IRequestEquipmentOrderService requestEquipmentOrderService, ICompanyService companyService, IConverter converterPDF, IProcedureExportHelper procedureExportHelper, IOptions<AppSettings> appSettings)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _requestEquipmentOrderService = requestEquipmentOrderService;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _procedureExportHelper = procedureExportHelper;
        _appSettings = appSettings.Value;
    }
    public async Task<PagingResult<RequestEquipmentPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.RequestEquipments
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.REQUEST_EQUIPMENT));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.REQUEST_EQUIPMENT) && x.log.UserId == userId && !x.procedure.IsFinished
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
        var itemOuts = new List<RequestEquipmentPagingModel>();

        foreach (var item in datas)
        {
            var itemOut = _mapper.Map<RequestEquipmentPagingModel>(item);
            itemOut.UserName = await _context.Users.Where(x => x.Id == item.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
            itemOut.ShoulDelete = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending
                                    && itemOut.ProcedureStatusName == startStatus.P_StatusName;
            itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;
            if (!string.IsNullOrEmpty(item.FileStr))
            {
                itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
            }
            if (item.IsFinished)
            {
                itemOut.ProcedureNumber = item.Code;
            }
            itemOuts.Add(itemOut);
        }
        var totalItem = await query.CountAsync();

        return new PagingResult<RequestEquipmentPagingModel>
        {
            Data = itemOuts,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<RequestEquipmentModel> GetDetail(int id, int userId)
    {
        var item = await _context.RequestEquipments.FirstOrDefaultAsync(x => x.Id == id);
        var itemOut = _mapper.Map<RequestEquipmentModel>(item);
        itemOut.Items = await _context.RequestEquipmentDetails.Where(x => x.RequestEquipmentId == id)
                        .Select(x => _mapper.Map<RequestEquipmentDetailModel>(x)).ToListAsync();
        itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
        itemOut.IsSave = await CheckButton(id, userId);
        return itemOut;
    }

    public async Task Create(RequestEquipmentModel form, int userId)
    {
        var procedure = _mapper.Map<RequestEquipment>(form);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;
        procedure.FileStr = JsonConvert.SerializeObject(form.Files);

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.IsFinished = false;

        await _context.RequestEquipments.AddAsync(procedure);
        await _context.SaveChangesAsync();

        var details = form.Items.Select(x => _mapper.Map<RequestEquipmentDetail>(x)).ToList();
        details = details.ConvertAll(x =>
        {
            x.RequestEquipmentId = procedure.Id;
            return x;
        });
        await _context.RequestEquipmentDetails.AddRangeAsync(details);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.REQUEST_EQUIPMENT), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }

    public async Task Update(RequestEquipmentModel form, int userId)
    {
        var procedure = await _context.RequestEquipments.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        procedure.FileStr = JsonConvert.SerializeObject(form.Files);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        _context.RequestEquipments.Update(procedure);

        var detailIds = form.Items.Select(X => X.Id).ToList();
        // delete item detail
        var detailDels = await _context.RequestEquipmentDetails.Where(X => X.RequestEquipmentId == form.Id && !detailIds.Contains(X.Id)).ToListAsync();
        if (detailDels.Any())
        {
            _context.RequestEquipmentDetails.RemoveRange(detailDels);
        }

        var details = form.Items.Select(x => _mapper.Map<RequestEquipmentDetail>(x)).ToList();
        details = details.ConvertAll(x =>
        {
            x.RequestEquipmentId = procedure.Id;
            return x;
        });
        _context.RequestEquipmentDetails.UpdateRange(details);
        await _context.SaveChangesAsync();

    }

    public async Task Accept(int id, int userId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        var procedure = await _context.RequestEquipments.FindAsync(id);
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
            await _requestEquipmentOrderService.Create(id);
        }

        _context.RequestEquipments.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.REQUEST_EQUIPMENT), status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.REQUEST_EQUIPMENT), status.Id, id, userId, procedure.ProcedureNumber, status.IsFinish);

        await transaction.CommitAsync();
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.RequestEquipments.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.RequestEquipments.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.REQUEST_EQUIPMENT), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.REQUEST_EQUIPMENT));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        var item = await _context.RequestEquipments.FindAsync(id);
        if (item != null)
        {
            _context.RequestEquipments.Remove(item);
            await _context.SaveChangesAsync();
        }
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.REQUEST_EQUIPMENT));

    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.RequestEquipments.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{nameof(ProcedureEnum.REQUEST_EQUIPMENT)}-{procedureNumber}";
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.RequestEquipments.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "MS");
    }

    private async Task<bool> CheckButton(int id, int userId)
    {
        var procedure = await _context.RequestEquipments.FindAsync(id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.REQUEST_EQUIPMENT));
        bool isSave = procedure.ProcedureStatusId == status.Id && procedure.UserCreated == userId;

        return isSave;
    }

    public async Task<IEnumerable<SelectListModel>> GetList()
    {
        return await _context.RequestEquipments.Where(x => x.IsFinished).Select(x => new SelectListModel
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Note
        }).ToListAsync();
    }

    public async Task<string> Export(int requestEquipmentId)
    {
        var payment = await _context.RequestEquipments.FindAsync(requestEquipmentId);

        var company = await _companyService.GetCompany();

        var soThapPhan = $"N{company.MethodCalcExportPrice}";

        var details = await _context.RequestEquipmentDetails.Where(x => x.RequestEquipmentId == requestEquipmentId).ToListAsync();

        var resultSignHtml = await _procedureExportHelper.SignPlace(requestEquipmentId, nameof(ProcedureEnum.REQUEST_EQUIPMENT));

        var resultHtml = GenerateExportTable(details, soThapPhan);
        var paymentDate = payment.Date;
        var depatmentName = await _context.Departments.Where(x => x.Id == payment.DepartmentId).Select(x => x.Name).FirstOrDefaultAsync();
        var values = new List<(string FieldName, string FieldValue)>
        {
            ("Code", payment.Code),
            ("Note", payment.Note),
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
            ("DepartmentName", depatmentName),
        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "DeNghiMuaSam.html"
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
            "DeNghiMuaSam"
        );
    }
    private string GenerateExportTable(List<RequestEquipmentDetail> expenditurePlanDetails, string soThapPhan)
    {
        //genaral tableHeader
        bool isExistGoodCategory = expenditurePlanDetails.Any(x => !string.IsNullOrEmpty(x.GoodCategory));
        bool isExistGoodProducer = expenditurePlanDetails.Any(x => !string.IsNullOrEmpty(x.GoodProducer));
        bool isExistGoodCatalog = expenditurePlanDetails.Any(x => !string.IsNullOrEmpty(x.GoodCatalog));

        var header = new StringBuilder();
        header.Append(@"
                        <thead class='font-b txt-center title-tbl' style='font-size: 12pt;'>
                        <tr>
                            <th>STT</th>
                            <th>Tên hàng</th>");
        if (isExistGoodCategory)
        {
            header.Append(" <th>Chủng loại</th>");
        }
        if (isExistGoodProducer)
        {
            header.Append(" <th>Nhà sản xuất</th>");
        }
        if (isExistGoodCatalog)
        {
            header.Append(" <th>Số catalog</th>");
        }

        header.Append(@"    <th>Đơn vị tính</th>
                            <th>Số lượng</th>
                            <th>Thời gian mong muốn nhận hàng</th>
                        </tr>
                    </thead>
                    <tbody class=""tbody-replace"">");
        var resultHtml = header;
        int index = 0;
        foreach (var expenditurePlanDetail in expenditurePlanDetails)
        {
            index++;
            resultHtml.Append(GenerateExportRow(expenditurePlanDetail, soThapPhan, index, isExistGoodCategory, isExistGoodProducer, isExistGoodCatalog));
        }
        resultHtml.Append("</tbody>");
        return resultHtml.ToString();
    }
    private string GenerateExportRow(RequestEquipmentDetail detail, string soThapPhan, int index, bool isExistGoodCategory, bool isExistGoodProducer, bool isExistGoodCatalog)
    {
        var body = new StringBuilder();
        body.Append($@"<tr>
                                <td class='txt-left'>{index}</td>
                                <td class='txt-left'>{detail.GoodName}</td>  ");
        if (isExistGoodCategory)
        {
            body.Append($" <td class='txt-left'>{detail.GoodCategory}</td>");
        }
        if (isExistGoodProducer)
        {
            body.Append($" <td class='txt-left'>{detail.GoodProducer}</td> ");
        }
        if (isExistGoodCatalog)
        {
            body.Append($"  <td class='txt-left'>{detail.GoodCatalog}</td>");
        }
        body.Append($@"                              
                                
                                <td class='txt-left'>{detail.GoodUnit}</td>
                                <td class='txt-right'>{detail.Quantity.FormatAmount(soThapPhan)}</td>
                                <td class='txt-center'>{detail.Date.ToString("dd/MM/yyyy")}</td>
                            </tr>");
        return body.ToString();
    }

}
