using AutoMapper;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Ledgers.V3;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.V3;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.Services.LedgerServices;

public class LedgerProcedureProductService : ILedgerProcedureProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ILedgerV3Service _ledgerV3Service;
    private readonly IFileService _fileService;
    public LedgerProcedureProductService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService, ILedgerV3Service ledgerV3Service,
        IFileService fileService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _ledgerV3Service = ledgerV3Service;
        _fileService = fileService;
    }

    public async Task<PagingResult<LedgerProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId, string type)
    {
        var query = _context.LedgerProcedureProducts.Where(x => x.Type == type)
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));
        var produceCode = GetProduceCode(type);
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, produceCode);
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
            .Where(x => x.log.ProcedureCode == produceCode && x.log.UserId == userId && !x.procedure.IsFinished
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var listOut = new List<LedgerProduceProductPagingModel>();

        foreach ( var item in data)
        {
            var itemOut = _mapper.Map<LedgerProduceProductPagingModel>(item);
            if (!string.IsNullOrEmpty(item.FileStr))
            {
                itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
            }
            itemOut.TotalAmount = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == item.Id).SumAsync(x => x.Amount);
            itemOut.TotalQuantity = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == item.Id).SumAsync(x => x.Quantity);
            itemOut.Content = string.Join(";", await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == item.Id).Select(x => x.OrginalDescription).ToListAsync());
            if (item.IsFinished)
            {
                itemOut.ProcedureNumber = item.Code;
            }
            listOut.Add(itemOut);
        }
        var totalItem = await query.CountAsync();

        return new PagingResult<LedgerProduceProductPagingModel>
        {
            Data = listOut,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<LedgerProduceProductModel> GetDetail(int id)
    {
        var item = await _context.LedgerProcedureProducts.FindAsync(id);
        var itemOut = _mapper.Map<LedgerProduceProductModel>(item);

        if (!string.IsNullOrEmpty(item.FileStr))
        {
            itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileStr);
        }
        itemOut.Items = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == id).Select(x => _mapper.Map<LedgerProduceProductDetailModel>(x)).ToListAsync();

        return itemOut;
    }

    public async Task Create(List<Ledger> ledgers, int userId, string type, int year)
    {
        if (ledgers is null || !ledgers.Any())
        {
            throw new ErrorException("Bạn chưa chọn phát sinh!!");
        }

        var produceCode = GetProduceCode(type);

        var procedure = new LedgerProcedureProduct();
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(produceCode);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;
        procedure.ProcedureNumber = await GetProcedureNumber(type);
        procedure.Type = type;

        procedure.Date = DateTime.Now;
        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.IsFinished = false;

        await _context.LedgerProcedureProducts.AddAsync(procedure);
        await _context.SaveChangesAsync();

        var detailAdds = new List<LedgerProcedureProductDetail>();

        foreach (var ledger in ledgers)
        {
            var detailAdd = _mapper.Map<LedgerProcedureProductDetail>(ledger);
            detailAdd.Id = 0;
            detailAdd.LedgerProcedureProductId = procedure.Id;
            detailAdd.LedgerSourceId = (int)ledger.Id;

            ChartOfAccount account = new ChartOfAccount();

            if (type == "NK")
            {
                if (!string.IsNullOrEmpty(ledger.DebitDetailCodeSecond))
                {
                    string parentRef = ledger.DebitCode + ":" + ledger.DebitDetailCodeFirst;
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.DebitDetailCodeSecond && x.Type == 6
                                    && x.ParentRef == parentRef);
                }
                else if (!string.IsNullOrEmpty(ledger.DebitDetailCodeFirst))
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.DebitDetailCodeFirst && x.Type == 5
                                   && x.ParentRef == ledger.DebitCode);
                }
                else
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.DebitCode);
                }
            }
            else if (type == "XK")
            {
                if (!string.IsNullOrEmpty(ledger.CreditDetailCodeSecond))
                {
                    string parentRef = ledger.CreditCode + ":" + ledger.CreditDetailCodeFirst;
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.CreditDetailCodeSecond && x.Type == 6
                                    && x.ParentRef == parentRef);
                }
                else if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.CreditDetailCodeFirst && x.Type == 5
                                   && x.ParentRef == ledger.CreditCode);
                }
                else
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.CreditCode);
                }
            }

            detailAdd.StockUnit = account?.StockUnit;
            detailAdds.Add(detailAdd);
        }

        await _context.LedgerProcedureProductDetails.AddRangeAsync(detailAdds);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLogToSendNotification(produceCode, status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }

    public async Task Update(LedgerProduceProductModel form, int userId)
    {
        var procedure = await _context.LedgerProcedureProducts.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        if (form.Files != null)
        {
            procedure.FileStr = JsonConvert.SerializeObject(form.Files);
        }

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        _context.LedgerProcedureProducts.Update(procedure);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, form.Id);
    }


    private async Task AddDetail(List<LedgerProduceProductDetailModel> items, int id)
    {
        var detailDels = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == id).ToListAsync();
        if (detailDels.Any())
        {
            _context.LedgerProcedureProductDetails.RemoveRange(detailDels);
        }

        var detailAdds = _mapper.Map<List<LedgerProcedureProductDetail>>(items);
        detailAdds = detailAdds.ConvertAll(x =>
        {
            x.Id = 0;
            x.LedgerProcedureProductId = id;
            return x;
        });

        await _context.LedgerProcedureProductDetails.AddRangeAsync(detailAdds);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId, int year)
    {

        var procedure = await _context.LedgerProcedureProducts.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
        var produceCode = GetProduceCode(procedure.Type);

        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            procedure.IsFinished = status.IsFinish;
            procedure.Code = await GetCodeAsync(procedure.Type);

            // add ledger
            var ledgers = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == id)
                                    .Select(x => _mapper.Map<LedgerV3UpdateModel>(x)).ToListAsync();

            ledgers = ledgers.ConvertAll(x =>
            {
                x.Id = 0;
                x.IsInternal = 1;
                x.AttachVoucher = procedure.Code;
                return x;
            });

            await _ledgerV3Service.UpdateAsync(ledgers, year);
        }

        _context.LedgerProcedureProducts.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(produceCode, status.Id, userId, id, procedure.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(produceCode, status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }

    public async Task Delete(int id)
    {
        var item = await _context.LedgerProcedureProducts.FindAsync(id);
        if (item == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        var produceCode = GetProduceCode(item.Type);

        var isExistLog = await _procedureHelperService.ExistLog(id, produceCode);
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        _context.LedgerProcedureProducts.Remove(item);

        var details = await _context.LedgerProcedureProductDetails.Where(x => x.LedgerProcedureProductId == id).ToListAsync();
        _context.LedgerProcedureProductDetails.RemoveRange(details);
        await _context.SaveChangesAsync();
        await _procedureHelperService.DeleteLog(id, produceCode);

    }

    private async Task<string> GetProcedureNumber(string type)
    {
        var produceCode = GetProduceCode(type);

        var item = await _context.LedgerProcedureProducts.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber) && x.Type == type).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"{produceCode}-{procedureNumber}";
    }


    public async Task<IEnumerable<FileDetailModel>> UploadFile(List<IFormFile> files, int id)
    {
        var procedure = await _context.LedgerProcedureProducts.FindAsync(id);
        if (procedure == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        var fileUploads = new List<FileDetailModel>();
        foreach (var file in files)
        {
            var fileUpload = _fileService.UploadFile(file, "LedgerProduceExport", file.FileName);
            fileUploads.Add(fileUpload);
        }
        procedure.FileStr = JsonConvert.SerializeObject(fileUploads);

        _context.LedgerProcedureProducts.Update(procedure);
        await _context.SaveChangesAsync();
        return fileUploads;
    }

    private async Task<string> GetCodeAsync(string type)
    {
        var month = DateTime.Today.Month;
        var codeNumber = await _context.LedgerProcedureProducts.Where(x => x.Type == type && x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, type);
    }

    private string GetProduceCode(string type)
    {
        var code = "";
        switch (type)
        {
            case "NK":
                code = nameof(ProcedureEnum.LEDGER_IMPORT);
                break;
            case "XK":
                code = nameof(ProcedureEnum.LEDGER_EXPORT);
                break;
            case "PC":
                code = nameof(ProcedureEnum.LEDGER_PAYMENT);
                break;
            case "PT":
                code = nameof(ProcedureEnum.LEDGER_RECEIPT);
                break;
        }
        return code;
    }

    public async Task NotAccept(int id, int userId)
    {

        var procedure = await _context.LedgerProcedureProducts.FindAsync(id);
        if (procedure.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var produceCode = GetProduceCode(procedure.Type);

        procedure.NoteNotAccept = procedure.ProcedureStatusName + "; " + procedure.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(produceCode);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.UpdatedAt = DateTime.Now;
        _context.LedgerProcedureProducts.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(produceCode, status.Id, userId, id, procedure.ProcedureNumber, true);
    }

}