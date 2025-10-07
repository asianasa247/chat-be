using AutoMapper;
using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManageEmployee.Services.P_ProcedureServices;

public class P_LeaveService : IP_LeaveService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly CodeGenerator _codeGenerator;

    public P_LeaveService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _codeGenerator = new CodeGenerator(context);
    }

    public async Task<PagingResult<P_LeavePagingModel>> GetAll(ProcedurePagingRequestModel param, List<string> RoleName, int userId)
    {

        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page > 0)
            param.Page -= 1;

        var result = new PagingResult<P_LeavePagingModel>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
        };


        var query = _context.P_Leave.Where(x => x.Id > 0 && (string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText)));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.LEAVE));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.LEAVE) && x.log.UserId == userId)
            .Select(x => x.procedure).Distinct();
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);
        var data = await query.Skip(param.Page * param.PageSize).Take(param.PageSize).ToListAsync();
        var listOut = new List<P_LeavePagingModel>();
        foreach (var item in data)
        {
            var user = await _context.Users.FindAsync(item.UserCreated);
            listOut.Add(new P_LeavePagingModel
            {
                Id = item.Id,
                Name = item.Name,
                CreateAt = item.CreatedAt,
                ProcedureNumber = item.ProcedureNumber,
                ProcedureStatusName = item.ProcedureStatusName,
                UserCreatedName = user?.FullName,
                Reason = item.Reason,
                Fromdt = item.Fromdt,
                Todt = item.Todt
            });
        }
        result.TotalItems = await query.CountAsync();
        result.Data = listOut;
        return result;

    }

    public async Task<P_LeaveViewModel> GetById(int id)
    {
        var item = await _context.P_Leave.FindAsync(id);
        var itemOut = _mapper.Map<P_LeaveViewModel>(item);
        itemOut.Items = await _context.P_Leave_Items.Where(x => x.LeaveId == id)
                            .Select(x => _mapper.Map<P_Leave_Item, P_Leave_ItemModel>(x))
                            .ToListAsync();
        return itemOut;
    }
    //public class CodeGenerator
    //{
    //    private int currentNumber = 1;
    //    private int currentMonth = DateTime.Now.Month;

    //    public string GenerateProcedureCode()
    //    {
    //        int month = DateTime.Now.Month;
    //        int year = DateTime.Now.Year % 100;

    //        // Reset số thứ tự nếu tháng thay đổi
    //        if (month != currentMonth)
    //        {
    //            currentMonth = month;
    //            currentNumber = 1;
    //        }

    //        // Tạo mã theo định dạng "NPXX-YY-XXXX"
    //        string code = $"NP{month:D2}-{year:D2}-{currentNumber:D4}";

    //        currentNumber++;
    //        return code;
    //    }
    //}

    public async Task Create(P_LeaveViewModel param, int userId)
    {
        using var transition = await _context.Database.BeginTransactionAsync();
        var item = _mapper.Map<P_Leave>(param);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.LEAVE));
        item.ProcedureNumber = await _codeGenerator.GenerateProcedureCodeAsync();
        item.ProcedureStatusId = status.Id;
        item.ProcedureStatusName = status.P_StatusName;
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        item.UserCreated = userId;
        item.UserUpdated = userId;
        await _context.P_Leave.AddAsync(item);
        await _context.SaveChangesAsync();

        if (param.Items != null)
        {
            var detailAdds = new List<P_Leave_Item>();
            foreach (var detail in param.Items)
            {
                detailAdds.Add(new P_Leave_Item
                {
                    LeaveId = item.Id,
                    Date = detail.Date,
                    IsLicensedItem = detail.IsLicensedItem,
                    SymbolCode = detail.SymbolCode,
                });
            }
            await _context.P_Leave_Items.AddRangeAsync(detailAdds);
            await _context.SaveChangesAsync();
        }

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.LEAVE), status.Id, item.Id, userId, item.ProcedureNumber);
        await _context.Database.CommitTransactionAsync();
    }


    public async Task Update(P_LeaveViewModel param, int userId)
    {
        using var transition = await _context.Database.BeginTransactionAsync();
        var item = _mapper.Map<P_Leave>(param);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.LEAVE));
        item.ProcedureStatusId = status.Id;
        item.ProcedureStatusName = status.P_StatusName;
        item.UpdatedAt = DateTime.Now;
        item.UserUpdated = userId;
        _context.P_Leave.Update(item);
        var detailDels = await _context.P_Leave_Items.Where(x => x.LeaveId == item.Id).ToListAsync();
        _context.P_Leave_Items.RemoveRange(detailDels);

        if (param.Items != null)
        {
            var detailAdds = new List<P_Leave_Item>();
            foreach (var detail in param.Items)
            {
                detailAdds.Add(new P_Leave_Item
                {
                    LeaveId = item.Id,
                    Date = detail.Date,
                    IsLicensedItem = detail.IsLicensedItem,
                    SymbolCode = detail.SymbolCode,
                });
            }
            await _context.P_Leave_Items.AddRangeAsync(detailAdds);
        }
        await _context.SaveChangesAsync();
        await _context.Database.CommitTransactionAsync();
    }

    public async Task Accept(P_LeaveViewModel param, int userId)
    {
        var item = await _context.P_Leave.FindAsync(param.Id);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(item.ProcedureStatusId, userId);
        item.ProcedureStatusId = status.Id;
        item.ProcedureStatusName = status.P_StatusName;
        if (!item.IsFinished)
            item.IsFinished = status.IsFinish;

        item.UpdatedAt = DateTime.Now;
        item.UserUpdated = userId;

        _context.P_Leave.Update(item);
        await _context.SaveChangesAsync();
        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.LEAVE), status.Id, item.Id, userId, item.ProcedureNumber);
    }

    public async Task Delete(int id)
    {
        var item = await _context.P_Leave.FindAsync(id);
        if (item is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        _context.P_Leave.Remove(item);
        var detailDels = await _context.P_Leave_Items.Where(x => x.LeaveId == item.Id).ToListAsync();
        _context.P_Leave_Items.RemoveRange(detailDels);

        await _context.SaveChangesAsync();
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.LEAVE));
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.P_Leave.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString()));
        if (item == null)
            return $"{nameof(ProcedureEnum.LEAVE)}_{dt}_0000001";
        var procedureNumbers = item.ProcedureNumber.Split("_");
        try
        {
            var procedureNumber = procedureNumbers[2];
            while (true)
            {
                if (procedureNumber.Length > 7)
                {
                    break;
                }
                procedureNumber = "0" + procedureNumber;
            }
            return $"{nameof(ProcedureEnum.LEAVE)}_{dt}_{procedureNumber}";
        }
        catch
        {
            return $"{nameof(ProcedureEnum.LEAVE)}_{dt}_0000001";
        }
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.P_Leave.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.LEAVE));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.P_Leave.Update(produce);

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.LEAVE), status.Id, userId, id, produce.ProcedureNumber, true);
        await _context.SaveChangesAsync();
    }

}