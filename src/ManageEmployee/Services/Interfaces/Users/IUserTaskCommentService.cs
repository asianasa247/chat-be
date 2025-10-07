using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserTaskCommentService
{
    Task<UserTaskCommentModel> GetById(int id);
    Task<List<UserTaskCommentModel>> GetAll();
    Task<UserTaskCommentModel> Add(UserTaskCommentModel entity, List<UserTaskFileModel> FileLink);
    Task<UserTaskCommentModel> Edit(UserTaskCommentModel entity);
    Task<List<UserTaskCommentModel>> GetByTaskId(int taskId, int userId);

}
