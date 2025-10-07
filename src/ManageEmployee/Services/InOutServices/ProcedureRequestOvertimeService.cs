using AutoMapper;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.InOutServices;
public class ProcedureRequestOvertimeService: IProcedureRequestOvertimeService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public ProcedureRequestOvertimeService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }

    public async Task<PagingResult<ProcedureRequestOvertimePagingModel>> GetPaging(ProcedureRequestOvertimePagingRequestModel param, int userId)
    {
        var query = _context.ProcedureRequestOvertimes
                    .Where(x => x.Id > 0);

        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.OVERTIME));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.OVERTIME) && (x.log.UserId == userId || x.procedure.UserCreated == userId)
                        && x.log.NotAcceptCount == 0 && !x.procedure.IsFinished)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        query = query.QueryDate(param);
        //query = query.QuerySearchTextProcedure(param);

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).Select(x => _mapper.Map<ProcedureRequestOvertimePagingModel>(x)).ToListAsync();
        var totalItem = await query.CountAsync();
        
        return new PagingResult<ProcedureRequestOvertimePagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<ProcedureRequestOvertimeModel> GetDetail(int id)
    {
        var procedure = await _context.ProcedureRequestOvertimes.FirstOrDefaultAsync(x => x.Id == id);
        var itemOut = _mapper.Map<ProcedureRequestOvertimeModel>(procedure);
        if (!string.IsNullOrEmpty(procedure.UserIdStr))
        {
            itemOut.UserIds = JsonConvert.DeserializeObject<List<int>>(procedure.UserIdStr);
        }
        return itemOut;
    }

    public async Task Create(ProcedureRequestOvertimeModel form, int userId)
    {
        var procedure = _mapper.Map<ProcedureRequestOvertime>(form);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.OVERTIME));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.UserIdStr = JsonConvert.SerializeObject(form.UserIds);
        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId; 
        procedure.IsFinished = false;

        await _context.ProcedureRequestOvertimes.AddAsync(procedure);
        await _context.SaveChangesAsync();
        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.OVERTIME), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }

    public async Task Update(ProcedureRequestOvertimeModel form, int userId)
    {
        var procedure = await _context.ProcedureRequestOvertimes.FindAsync(form.Id);
        if(procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        procedure.FromAt = form.FromAt;
        procedure.ToAt = form.ToAt;
        procedure.Name = form.Name;
        procedure.UserIdStr = JsonConvert.SerializeObject(form.UserIds);

        procedure.SymbolId = form.SymbolId;
        procedure.Coefficient = form.Coefficient;

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        _context.ProcedureRequestOvertimes.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.ProcedureRequestOvertimes.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (!procedure.IsFinished)
            procedure.IsFinished = status.IsFinish;

        _context.ProcedureRequestOvertimes.Update(procedure);
        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.OVERTIME), status.Id, userId, id, procedure.ProcedureNumber);

        await _context.SaveChangesAsync();
        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.OVERTIME), status.Id, procedure.Id, userId, procedure.ProcedureNumber);
    }
    public async Task Delete(int id)
    {
        var item = await _context.ProcedureRequestOvertimes.FindAsync(id);
        if (item != null)
        {
            _context.ProcedureRequestOvertimes.Remove(item);
            await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.OVERTIME));
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.ProcedureRequestOvertimes.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);

        return $"{nameof(ProcedureEnum.OVERTIME)}-{procedureNumber}";
    }

    public async Task Copy(int id, List<int> userIds, int userIdSetter)
    {
        var procedure = await _context.ProcedureRequestOvertimes.FindAsync(id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.OVERTIME));

        var itemAdd = new ProcedureRequestOvertime
        {
            UserIdStr = JsonConvert.SerializeObject(userIds),
            ProcedureNumber = procedure.ProcedureNumber,
            Name = procedure.Name,
            FromAt = procedure.FromAt,
            ToAt = procedure.ToAt,
            SymbolId = procedure.SymbolId,
            Coefficient = procedure.Coefficient,
            IsFinished = false,
            ProcedureStatusId = status.Id,
            ProcedureStatusName = status.P_StatusName,
            UserCreated = userIdSetter,
            UserUpdated = userIdSetter
        };
        
        await _context.ProcedureRequestOvertimes.AddAsync(itemAdd);
        await _context.SaveChangesAsync();
    }

    public async Task NotAccept(int id, int userId)
    {

        var procedure = await _context.ProcedureRequestOvertimes.FindAsync(id);
        if (procedure.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var produceCode = nameof(ProcedureEnum.OVERTIME);

        procedure.NoteNotAccept = procedure.ProcedureStatusName + "; " + procedure.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(produceCode);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.UpdatedAt = DateTime.Now;
        _context.ProcedureRequestOvertimes.Update(procedure);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(produceCode, status.Id, userId, id, procedure.ProcedureNumber, true);
    }

}
