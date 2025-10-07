using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.UserModels.SalaryModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.UserEntites;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserService
{
    Task<UserMapper.Auth> Authenticate(string username, string password);

    Task<IEnumerable<UserActiveModel>> GetAllUserActive(List<string> listRole, int userId);

    Task<IEnumerable<string>> GetAllUserName();

    Task<IEnumerable<User>> GetMany(Expression<Func<User, bool>> where);

    Task<User> GetByIdAsync(int id);

    Task<User> GetByUserName(string username);

    Task Create(UserModel userParam, string password, string dbName = "");

    Task CreateExcel(List<UserModel> users, int userId);

    Task Update(UserModel userParam, string dbName = "");

    Task ResetPassword(User userParam, string password = null);

    Task<bool> CheckPassword(int id, string oldPassword);

    Task UpdatePassword(PasswordModel passwordModel);

    Task UpdateLastLogin(int userId);

    Task Delete(int id);

    Task<BaseResponseModel> GetPaging(UserMapper.FilterParams param);

    Task<List<UserModel>> GetForExcel(List<int> ids, int userId, List<string> roles);

    Task<List<ThongKeTongQuat>> GetListThongKeTongQuat(List<string> listRole, int userId);

    Task<List<SelectListModel>> HeaderThongKeTongQuat();

    Task UpdateSalarySocial(SalarySocial data);

    Task<string> GetUserName();

    Task<List<SalarySocial>> GetListSalarySocial();
    Task UpdateCurrentYear(int year, int userId);
    Task<List<int>> GetYearSales();
    Task<object> GetUserStatistics(List<string> listRole, int userId);
    Task<SalarySocialDetailModel> GetSalarySocialById(int id, int year);
    Task<string> ExportExcel(List<int> ids, int userId, List<string> roles, bool allowImages);
    IQueryable<User> QueryUserForPermission(int userId, List<string> roles);
    Task<IQueryable<User>> GetListUserCommon1(List<string> listRole, int userId);
    Task<IEnumerable<UserActiveModel>> GetAllUserActive1(List<string> listRole, int userId);
    Task<IEnumerable<object>> GetAllUserNotRole();
    FileDetailModel UploadFile(IFormFile file, string folder, string fileNameUpload);
    Task ResetPasswordForMultipleUser(List<int> ids);
    Task<List<UserModel>> GetUsersByReminderAsync(ReminderModel reminder);
    Task<List<User>> GetAllAsync();
}