using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.InOutServices;
public class ProcedureChangeShiftService : IProcedureChangeShiftService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public ProcedureChangeShiftService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }

    public async Task<PagingResult<ProcedureChangeShiftModel>> GetPaging(ProcedureRequestOvertimePagingRequestModel param)
    {
        var query = _context.ProcedureChangeShifts
                    .Where(x => (param.FromAt == null || x.FromAt >= param.FromAt)
                                && (param.ToAt == null || x.ToAt <= param.ToAt));
        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).Select(x => _mapper.Map<ProcedureChangeShiftModel>(x)).ToListAsync();
        var totalItem = await query.CountAsync();
        return new PagingResult<ProcedureChangeShiftModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<ProcedureChangeShiftModel> GetDetail(int id)
    {
        return await _context.ProcedureChangeShifts.Select(x => _mapper.Map<ProcedureChangeShiftModel>(x)).FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task Create(ProcedureChangeShiftModel form, int userId)
    {
        var procedure = _mapper.Map<ProcedureChangeShift>(form);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.CHANGE_SHIFT));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.IsFinished = false;

        await _context.ProcedureChangeShifts.AddAsync(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ProcedureChangeShiftModel form, int userId)
    {
        var procedure = await _context.ProcedureChangeShifts.FindAsync(form.Id);
        procedure.FromAt = form.FromAt;
        procedure.ToAt = form.ToAt;
        procedure.FromUserId = form.FromUserId;
        procedure.ToUserId = form.ToUserId;

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        _context.ProcedureChangeShifts.Update(procedure);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(ProcedureChangeShiftModel form, int userId)
    {
        var procedure = await _context.ProcedureChangeShifts.FindAsync(form.Id);
        procedure.FromAt = form.FromAt;
        procedure.ToAt = form.ToAt;
        procedure.FromUserId = form.FromUserId;
        procedure.ToUserId = form.ToUserId;

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (!procedure.IsFinished)
            procedure.IsFinished = status.IsFinish;

        _context.ProcedureChangeShifts.Update(procedure);
        await _context.SaveChangesAsync();
    }
    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.ProcedureChangeShifts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.CHANGE_SHIFT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.ProcedureChangeShifts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.CHANGE_SHIFT), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    public async Task Delete(int id)
    {
        var item = await _context.ProcedureChangeShifts.FindAsync(id);
        if (item != null)
        {
            _context.ProcedureChangeShifts.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.ProcedureChangeShifts.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString()));
        if (item == null)
            return $"CHANGESHIFT_{dt}_0000001";
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
            return $"CHANGESHIFT_{dt}_{procedureNumber}";
        }
        catch
        {
            return $"CHANGESHIFT_{dt}_0000001";
        }
    }

    public async Task<ProcedureCheckButton> CheckButton(int id, int userId)
    {
        var itemOut = new ProcedureCheckButton()
        {
            IsAccept = false,
            IsDelete = false,
            IsSave = false,
            IsAdd = false,
        };

        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            throw new ErrorException(ErrorMessages.UserNotFound);
        }
        var step = new P_ProcedureStatusStep();
        var userRoleIds = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();

        if (id == 0)
        {
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.CHANGE_SHIFT));
            step = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdFrom == status.Id);
            itemOut.IsAdd = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));
            return itemOut;
        }

        var procedure = await _context.ProcedureChangeShifts.FindAsync(id);
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

        itemOut.IsAccept = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdTo
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));

        return itemOut;
    }
}
