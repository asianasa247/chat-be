using AutoMapper;
using Common.Extensions;
using DinkToPdf.Contracts;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Cars;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.Services.CarServices;
public class CarLocationService: ICarLocationService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;
    public CarLocationService(ApplicationDbContext dbcontext,
        IMapper mapper,
        IProcedureHelperService procedureHelperService,
        ICompanyService companyService, 
        IConverter converterPDF,
        IOptions<AppSettings> appSettings, 
        IProcedureExportHelper procedureExportHelper)
    {
        _context = dbcontext;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _appSettings = appSettings.Value;
        _procedureExportHelper = procedureExportHelper;
    }
    public async Task<PagingResult<CarLocationPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.CarLocations.Where(X => X.Id > 0);

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.CAR_LOCATION));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.CAR_LOCATION));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.CAR_LOCATION) && x.log.UserId == userId && !x.procedure.IsFinished
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }

        var data = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize)
            .Select(x => _mapper.Map<CarLocationPagingModel>(x)).ToListAsync();

        foreach (var item in data)
        {
            item.ShoulDelete = param.StatusTab == ProduceProductStatusTab.Pending && item.ProcedureStatusName == startStatus.P_StatusName;
            item.ShoulNotAccept = param.StatusTab == ProduceProductStatusTab.Pending && item.ProcedureStatusName != startStatus.P_StatusName;
        }
        var totalItem = await query.CountAsync();

        return new PagingResult<CarLocationPagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<CarLocationModel> GetDetail(int id)
    {
        var item = await _context.CarLocations.Where(x => x.Id == id).Select(x => _mapper.Map<CarLocationModel>(x)).FirstOrDefaultAsync();
        item.Items = new List<CarLocationDetailModel>();
        var details = await _context.CarLocationDetails.Where(X => X.CarLocationId == id).ToListAsync();
        foreach(var detail in details)
        {
            var itemOut = _mapper.Map<CarLocationDetailModel>(detail);
            if (!string.IsNullOrEmpty(detail.FileStr))
            {
                itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(detail.FileStr);
            }
            item.Items.Add(itemOut);
        }
        
        return item;
    }

    public async Task Create(CarLocationModel form, int userId)
    {
        var produce = new CarLocation
        {
            Date = DateTime.Now,
            Note = form.Note,
            CreatedAt = DateTime.Now,
            UserCreated = userId,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
        };

        if (string.IsNullOrEmpty(produce.ProcedureNumber))
        {
            produce.ProcedureNumber = await GetProcedureNumber();
        }

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.CAR_LOCATION));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;
        await _context.CarLocations.AddAsync(produce);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, produce.Id);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.CAR_LOCATION), status.Id, produce.Id, userId, produce.ProcedureNumber);
    }

    public async Task Update(CarLocationModel form, int userId)
    {
        var produce = await _context.CarLocations.FindAsync(form.Id);

        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.CarLocations.Update(produce);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, produce.Id);
    }

    private async Task AddDetail(List<CarLocationDetailModel> items, int produceId)
    {
        var planningDetailIds = items.Select(x => x.Id).ToList();
        var itemDels = await _context.CarLocationDetails.Where(x => x.CarLocationId == produceId).ToListAsync();
        _context.CarLocationDetails.RemoveRange(itemDels);

        var detailAdds = new List<CarLocationDetail>(); 
        foreach(var item in items)
        {
            detailAdds.Add(new CarLocationDetail
            {
                Id = 0,
                CarLocationId = produceId,
                FileStr = JsonConvert.SerializeObject(item.Files),
                DriverName = item.DriverName,
                LicensePlates = item.LicensePlates,
                Location = item.Location,
                Note = item.Note,
                Payload = item.Payload,
                PlanExpected = item.PlanExpected,
                PlanInprogress = item.PlanInprogress,
            });
        }

        _context.CarLocationDetails.AddRange(detailAdds);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var produce = await _context.CarLocations.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        // validate condition
        var status = await _procedureHelperService.GetStatusAccept(produce.ProcedureStatusId ?? 0, userId);
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;
        produce.IsFinished = status.IsFinish;

        produce.UpdatedAt = DateTime.Now;

        _context.CarLocations.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.CAR_LOCATION), status.Id, userId, id, produce.ProcedureNumber);
        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.CAR_LOCATION), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.CarLocations.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.CAR_LOCATION));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.CarLocations.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.CAR_LOCATION), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.CarLocations.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);

        return $"{nameof(ProcedureEnum.CAR_LOCATION)}-{procedureNumber}";
    }

    public async Task Delete(int id)
    {
        var item = await _context.CarLocations.FindAsync(id);
        if (item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var isExistLog = await _context.ProcedureLogs.AnyAsync(x => x.ProcedureId == id && x.ProcedureCode == nameof(ProcedureEnum.CAR_LOCATION));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var details = await _context.CarLocationDetails.Where(x => x.CarLocationId == id).ToListAsync();
        _context.CarLocationDetails.RemoveRange(details);
        _context.CarLocations.Remove(item);

        await _context.SaveChangesAsync();
    }

    public async Task<string> Export(int id)
    {
        var carLocation = await _context.CarLocations.FindAsync(id);
        var company = await _companyService.GetCompany();
        var details = await _context.CarLocationDetails.Where(x => x.CarLocationId == id).ToListAsync();
        var resultSignHtml = await _procedureExportHelper.SignPlace(id, nameof(ProcedureEnum.CAR_LOCATION));
        var resultHtml = GenerateExportTable(details);
        var paymentDate = carLocation.Date;

        var values = new List<(string FieldName, string FieldValue)>
        {
            ("CompanyName", company.Name),
            ("CompanyAddress", company.Address),
            ("Day", $"{paymentDate.Day}"),
            ("Month", $"{paymentDate.Month}"),
            ("Year", $"{paymentDate.Year}"),
            ("Note", carLocation.Note),
            ("MST", company.MST),
            ("Phone", company.Phone),
            ("CompanyImage", $"{_appSettings.UrlHost}{company.FileLogo}"),
        };

        var allText = await FileExtension.ReadContent(
            Directory.GetCurrentDirectory(),
            @"Uploads\Html\ProduceProduct",
            "ViTriXe.html"
        );

        var allTextBuilder = new StringBuilder(allText);

        values.ForEach(x =>
        {
            allTextBuilder.Replace("{{{" + x.FieldName + "}}}", x.FieldValue);
        });

        allTextBuilder.Replace("##SIGN_REPLACE_PLACE##", resultSignHtml);
        allTextBuilder.Replace("##REPLACE_PLACE##", resultHtml);

        return ExcelHelpers.ConvertUseDinkLandscape(
            allTextBuilder.ToString(),
            _converterPDF,
            Directory.GetCurrentDirectory(),
            "ViTriXe"
        );
    }

    private string GenerateExportTable(List<CarLocationDetail> paymentDetails)
    {
        var resultHtml = new StringBuilder();
        for (int i = 1; i <= paymentDetails.Count; i++)
        {
            var paymentDetail = paymentDetails[i - 1];
            resultHtml.Append(GenerateExportRow(i, paymentDetail));
        }

        return resultHtml.ToString();
    }

    private static string GenerateExportRow(int rowIndex, CarLocationDetail paymentDetail)
    {
        string txt = $@"<tr>
                                <td class='txt-center'>{rowIndex}</td>
                                <td>{paymentDetail.LicensePlates}</td>
                                <td>{paymentDetail.Type}</td>
                                <td>{paymentDetail.Payload}</td>
                                <td>{paymentDetail.DriverName}</td>
                                <td>{paymentDetail.Location}</td>
                                <td>{paymentDetail.PlanInprogress}</td>
                                <td>{paymentDetail.PlanExpected}</td>
                                <td>{paymentDetail.Note}</td>
                            </tr>";
        return txt;
    }

}
