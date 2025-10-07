using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.MenuModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;

public class ProcedureService : IProcedureService
{
    private readonly ApplicationDbContext _context;
    private readonly IMenuService _menuService;

    public ProcedureService(ApplicationDbContext context, IMenuService menuService)
    {
        _context = context;
        _menuService = menuService;
    }

    public async Task<PagingResult<P_Procedure>> GetPaging(PagingRequestModel param)
    {
        var query = _context.P_Procedure
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText) || x.Code.Contains(param.SearchText));
        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var totalItem = await query.CountAsync();
        return new PagingResult<P_Procedure>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<IEnumerable<P_Procedure>> GetAll()
    {
        return await _context.P_Procedure.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<P_ProcedureViewModel> GetById(int id)
    {
        var procedure = await _context.P_Procedure.FindAsync(id);
        var statusItems = await _context.P_ProcedureStatus.Where(x => x.P_ProcedureId == id).ToListAsync();
        var roleItems = await _context.P_ProcedureStatusRole.Where(x => x.P_ProcedureId == id).ToListAsync();

        var itemOut = new P_ProcedureViewModel
        {
            Code = procedure.Code,
            Name = procedure.Name,
            Note = procedure.Note,
            Id = id,
            StatusItems = new List<P_ProcedureStatusViewModel>()
        };
        foreach (var statusItem in statusItems)
        {
            var status = new P_ProcedureStatusViewModel
            {
                Note = statusItem.Note,
                Name = statusItem.Name,
                Type = statusItem.Type,
                UserIds = roleItems.Where(x => x.P_ProcedureStatusId == statusItem.Id).Select(x => x.UserId ?? 0).Distinct().ToList(),
                RoleIds = roleItems.Where(x => x.P_ProcedureStatusId == statusItem.Id).Select(x => x.RoleId ?? 0).Distinct().ToList(),
                Id = statusItem.Id,
            };
            itemOut.StatusItems.Add(status);
        }
        return itemOut;
    }

    public async Task Create(P_ProcedureViewModel param)
    {
        var procedure = new P_Procedure
        {
            Code = param.Code.Trim(),
            Name = param.Name,
            Note = param.Note,
        };
        await _context.P_Procedure.AddAsync(procedure);
        await _context.SaveChangesAsync();
        await UpdateProcedureStatus(procedure.Id, param.StatusItems);
    }

    public async Task Update(P_ProcedureViewModel param)
    {
        var procedure = await _context.P_Procedure.FindAsync(param.Id);
        procedure.Code = param.Code.Trim();
        procedure.Name = param.Name;
        procedure.Note = param.Note;
        _context.P_Procedure.Update(procedure);

        var roleDels = await _context.P_ProcedureStatusRole.Where(x => x.P_ProcedureId == param.Id).ToListAsync();
        _context.P_ProcedureStatusRole.RemoveRange(roleDels);
        await UpdateProcedureStatus(param.Id, param.StatusItems);
    }

    private async Task UpdateProcedureStatus(int procedureId, List<P_ProcedureStatusViewModel> statusItems)
    {
        if (statusItems != null)
        {

            var itemUpdates = statusItems.Where(x => x.Id > 0 && !x.IsDeleted);
            foreach (var item in itemUpdates)
            {
                var status = new P_ProcedureStatus
                {
                    Id = item.Id,
                    P_ProcedureId = procedureId,
                    Name = item.Name,
                    Note = item.Note,
                    Type = item.Type,
                };
                _context.P_ProcedureStatus.Update(status);

                var roleUsers = item.UserIds?.ConvertAll(x => new P_ProcedureStatusRole
                {
                    P_ProcedureStatusId = status.Id,
                    UserId = x,
                    P_ProcedureId = procedureId
                });

                var roles = item.RoleIds?.ConvertAll(x => new P_ProcedureStatusRole
                {
                    P_ProcedureStatusId = status.Id,
                    RoleId = x,
                    P_ProcedureId = procedureId
                });
                await _context.P_ProcedureStatusRole.AddRangeAsync(roleUsers);
                await _context.P_ProcedureStatusRole.AddRangeAsync(roles);
            }
        }

        var itemDeleteIds = statusItems.Where(x => x.Id > 0 && x.IsDeleted).Select(x => x.Id);
        var itemDeletes = await _context.P_ProcedureStatus.Where(x => itemDeleteIds.Contains(x.Id)).ToListAsync();
        _context.P_ProcedureStatus.RemoveRange(itemDeletes);

        var itemAdds = statusItems.Where(x => x.Id == 0 && !x.IsDeleted).ToList();
        foreach (var item in itemAdds)
        {
            var status = new P_ProcedureStatus
            {
                P_ProcedureId = procedureId,
                Name = item.Name,
                Note = item.Note,
                Type = item.Type,
            };
            await _context.P_ProcedureStatus.AddAsync(status);
            await _context.SaveChangesAsync();

            var roleUsers = item.UserIds?.ConvertAll(x => new P_ProcedureStatusRole
            {
                P_ProcedureStatusId = status.Id,
                UserId = x,
                P_ProcedureId = procedureId
            });

            var roles = item.RoleIds?.ConvertAll(x => new P_ProcedureStatusRole
            {
                P_ProcedureStatusId = status.Id,
                RoleId = x,
                P_ProcedureId = procedureId
            });
            await _context.P_ProcedureStatusRole.AddRangeAsync(roleUsers);
            await _context.P_ProcedureStatusRole.AddRangeAsync(roles);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var procedure = await _context.P_Procedure.FindAsync(id);
        if (procedure != null)
        {
            _context.P_Procedure.Remove(procedure);
            var status = await _context.P_ProcedureStatus.Where(x => x.P_ProcedureId == id).ToListAsync();
            _context.P_ProcedureStatus.RemoveRange(status);

            var roles = await _context.P_ProcedureStatusRole.Where(x => x.P_ProcedureId == id).ToListAsync();
            _context.P_ProcedureStatusRole.RemoveRange(roles);

            var steps = await _context.P_ProcedureStatusSteps.Where(x => x.P_ProcedureId == id).ToListAsync();
            _context.P_ProcedureStatusSteps.RemoveRange(steps);

            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<MenuCheckRole>> GetProcedureNeedAccept(int userId)
    {
        List<MenuCheckRole> listMenuCheckRole = await _menuService.GetListMenu(userId);
        var procedureEnums = Enum.GetValues(typeof(ProcedureEnum))
                               .Cast<ProcedureEnum>()
                               .Select(v => v.ToString())
                               .ToList();

        var userRoleIds = await _context.Users.Where(x => x.Id == userId).Select(x => x.UserRoleIds).FirstOrDefaultAsync();
        var userIdStr = $";{userId};";
        var userRoleIdArrs = userRoleIds.Split(",").Select(x => int.Parse(x)).ToList();

        foreach (var procedureEnum in procedureEnums)
        {
            var query = _context.ProcedureLogs.Where(x => x.ProcedureCode == procedureEnum && x.IsSendNotification);
            int count = await query.Where(x => x.UserIds.Contains(userIdStr)).CountAsync();
            var logIds = new List<int>();
            if (count > 0)
            {
                logIds = await query.Where(x => x.UserIds.Contains(userIdStr)).Select(x => x.Id).ToListAsync();
            }

            foreach (var userRoleId in userRoleIdArrs)
            {
                var userRoleIdStr = $";{userRoleId};";
                var countRole = await query.Where(x => x.RoleIds.Contains(userRoleIdStr) && !logIds.Contains(x.Id)).CountAsync();
                if (countRole > 0)
                {
                    var logIdRoles = await query.Where(x => x.RoleIds.Contains(userRoleIdStr) && !logIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();
                    logIds.AddRange(logIdRoles);

                    count += countRole;
                }
            }
            var menu = listMenuCheckRole.Find(c => c.MenuCode == procedureEnum);
            if (menu != null)
            {
                menu.ProcedureCount = count;
            }
        }
        return listMenuCheckRole;
    }

    public async Task ResetProcedureCountAsync(int userId, string procedureCode)
    {
        var userRoleIds = await _context.Users.Where(x => x.Id == userId).Select(x => x.UserRoleIds).FirstOrDefaultAsync();
        var userIdStr = $";{userId};";
        var userRoleIdArrs = userRoleIds.Split(",").Select(x => int.Parse(x)).ToList();

        var query = _context.ProcedureLogs.Where(x => x.ProcedureCode == procedureCode);

        var logIds = await query.Where(x => x.UserIds.Contains(userIdStr)).Select(x => x.Id).ToListAsync();
        foreach (var userRoleId in userRoleIdArrs)
        {
            var userRoleIdStr = $";{userRoleId};";
            var logFromRoleIds = await query.Where(x => x.RoleIds.Contains(userRoleIdStr)).Select(x => x.Id).ToListAsync();
            logIds.AddRange(logFromRoleIds);
        }
        logIds = logIds.Distinct().ToList();

        var logs = await _context.ProcedureLogs.Where(x => logIds.Contains(x.Id)).ToListAsync();
        _context.ProcedureLogs.RemoveRange(logs);
        await _context.SaveChangesAsync();
    }
}