using Common.Constants;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.UserServices;

public class UserRoleService : IUserRoleService
{
    private readonly ApplicationDbContext _context;

    public UserRoleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserRole>> GetAll(int userId, List<string> listRole)
    {
        var userRoles = await _context.Users.Where(x => x.Id == userId).Select(x => x.UserRoleIds).FirstOrDefaultAsync();
        if (userRoles is null)
        {
            throw new ErrorException(ErrorMessages.AccessDenined);
        }

        var userRoleIds = userRoles.Split(",").Select(x => int.Parse(x)).ToList();

        return await _context.UserRoles.Where(x => listRole.Contains(UserRoleConst.SuperAdmin)
                    || x.UserCreated == userId
                    || userRoleIds.Contains(x.Id)
                    ).OrderBy(x => x.Order).ToListAsync();
    }

    public async Task<IEnumerable<UserRole>> GetAll_Login()
    {
        return await _context.UserRoles.OrderBy(x => x.Order).ToListAsync();
    }

    public async Task<UserRole> GetById(int id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);
        if (userRole is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        return userRole;
    }

    public async Task<UserRole> Create(UserRole userRole)
    {
        // validation
        if (string.IsNullOrWhiteSpace(userRole.Title))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();

        return userRole;
    }

    public async Task<UserRole> Update(UserRole userRoleParam)
    {
        var userRole = await _context.UserRoles.FindAsync(userRoleParam.Id);

        if (userRole == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        userRole.Title = userRoleParam.Title;
        userRole.Code = userRoleParam.Code;
        userRole.Note = userRoleParam.Note;
        userRole.Order = userRoleParam.Order;
        _context.UserRoles.Update(userRole);
        await _context.SaveChangesAsync();
        return userRole;
    }

    public async Task Delete(int id)
    {
        var userRole = await _context.UserRoles.FindAsync(id);
        if (userRole != null)
        {
            if (userRole.IsNotAllowDelete)
            {
                return;
            }
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        }
    }
}