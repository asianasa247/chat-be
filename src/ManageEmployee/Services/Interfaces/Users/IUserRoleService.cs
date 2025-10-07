using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserRoleService
{
    Task<IEnumerable<UserRole>> GetAll(int userId, List<string> listRole);

    Task<UserRole> GetById(int id);

    Task<UserRole> Create(UserRole userRole);

    Task<UserRole> Update(UserRole userRoleParam);

    Task Delete(int id);

    Task<IEnumerable<UserRole>> GetAll_Login();
}
