using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Entities.SupplyEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices.Services;
public class RequestExportGoodService : IRequestExportGoodService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;

    public RequestExportGoodService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
    }

    public async Task<PagingResult<RequestExportGoodPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.RequestExportGoods
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.ProcedureNumber.Contains(param.SearchText));

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.EXPORT_GOOD));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.EXPORT_GOOD));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.EXPORT_GOOD) && x.log.UserId == userId && !x.procedure.IsFinished
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }

        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).Select(x => _mapper.Map<RequestExportGoodPagingModel>(x)).ToListAsync();
        var totalItem = await query.CountAsync();

        return new PagingResult<RequestExportGoodPagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<RequestExportGoodModel> GetDetail(int id)
    {
        var data = await _context.RequestExportGoods.Where(x => x.Id == id).Select(x => _mapper.Map<RequestExportGoodModel>(x)).FirstOrDefaultAsync();
        data.Items = await _context.RequestExportGoodDetails.Where(x => x.RequestExportGoodId == id).Select(x => _mapper.Map<RequestExportGoodDetailModel>(x)).ToListAsync();
        return data;
    }

    public async Task Create(RequestExportGoodModel form, int userId)
    {
        var procedure = _mapper.Map<RequestExportGood>(form);
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.EXPORT_GOOD));
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        procedure.CreatedAt = DateTime.Now;
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserCreated = userId;
        procedure.UserUpdated = userId;
        procedure.IsFinished = false;

        await _context.RequestExportGoods.AddAsync(procedure);
        await _context.SaveChangesAsync();
        var items = form.Items.ConvertAll(x => new RequestExportGoodDetail
        {
            RequestExportGoodId = procedure.Id,
            GoodId = x.GoodId,
            Quantity = x.Quantity,
            Note = x.Note,
            TotalAmout = x.TotalAmout,
        });
        await _context.RequestExportGoodDetails.AddRangeAsync(items);

    }

    public async Task Update(RequestExportGoodModel form, int userId)
    {
        var procedure = await _context.RequestExportGoods.FindAsync(form.Id);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        procedure.UserId = form.UserId;

        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        _context.RequestExportGoods.Update(procedure);

        var itemDels = await _context.RequestExportGoodDetails.Where(x => x.RequestExportGoodId == form.Id).ToListAsync();
        _context.RequestExportGoodDetails.RemoveRange(itemDels);

        var items = form.Items.ConvertAll(x => new RequestExportGoodDetail
        {
            RequestExportGoodId = form.Id,
            GoodId = x.GoodId,
            Quantity = x.Quantity,
            Note = x.Note,
            TotalAmout = x.TotalAmout,
        });
        await _context.RequestExportGoodDetails.AddRangeAsync(items);

        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var procedure = await _context.RequestExportGoods.FindAsync(id);
        procedure.UpdatedAt = DateTime.Now;
        procedure.UserUpdated = userId;
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusAccept(procedure.ProcedureStatusId, userId);
        procedure.ProcedureStatusId = status.Id;
        procedure.ProcedureStatusName = status.P_StatusName;

        if (!procedure.IsFinished)
            procedure.IsFinished = status.IsFinish;

        _context.RequestExportGoods.Update(procedure);
        await _context.SaveChangesAsync();
    }
    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.EXPORT_GOOD));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        var item = await _context.RequestExportGoods.FindAsync(id);
        if (item != null)
        {
            _context.RequestExportGoods.Remove(item);
            var items = await _context.RequestExportGoodDetails.Where(x => x.RequestExportGoodId == id).ToListAsync();
            _context.RequestExportGoodDetails.RemoveRange(items);

            await _context.SaveChangesAsync();
        }
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.EXPORT_GOOD));

    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.RequestExportGoods.Where(x => !string.IsNullOrEmpty(x.ProcedureNumber)).OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : "0" + DateTime.Now.Month.ToString());
        if (item == null)
            return $"{nameof(ProcedureEnum.EXPORT_GOOD)}_{dt}_0000001";
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
            return $"{nameof(ProcedureEnum.EXPORT_GOOD)}_{dt}_{procedureNumber}";
        }
        catch
        {
            return $"{nameof(ProcedureEnum.EXPORT_GOOD)}_{dt}_0000001";
        }
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
            ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.EXPORT_GOOD));
            step = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdFrom == status.Id);
            itemOut.IsAdd = await _context.P_ProcedureStatusRole
                    .AnyAsync(x => x.P_ProcedureStatusId == step.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));
            return itemOut;
        }

        var procedure = await _context.RequestExportGoods.FindAsync(id);
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
}
