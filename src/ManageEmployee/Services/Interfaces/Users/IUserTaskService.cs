using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserTaskService
{
    Task<UserTaskModel> Copy(int id);

    Task<PagingResult<UserTaskModeList>> GetListMode(UserTaskRequestModel request, int userId);

    Task<PagingResult<UserTaskModeList>> GetDueDateMode(UserTaskRequestModel request, int UserId);

    Task<List<UserTaskModeList>> GetParentList(int UserId);

    Task PinTask(int UserTaskId, int UserId);

    Task<UserTask> StatusTask(UserTaskStatusModel status, int UserId);

    Task<UserTaskModel> Add(UserTask entityUserTask, List<UserTaskCheckList> entityCheckList,
         List<UserTaskRoleDetails> entityTaskRole, List<UserTaskFileModel> FileLink, int createdBy);

    Task<UserTaskModel> Edit(UserTask entity, List<UserTaskCheckList> entityCheckList,
         List<UserTaskRoleDetails> entityTaskRole, List<UserTaskFileModel> FileLink);

    Task<UserTask> Delete(int id);

    Task<UserTaskModel> Get(int UserTaskId, int UserId);

    Task<string> TrackingTask(int UserTaskId, int IsStart, int UserId, int? currentStatus);

    Task<PagingResult<UserTask>> GetListTaskProject(UserTaskRequestModel request, int userId);

    Task<PagingResult<UserTaskModeList>> GetTaskProjectParent(UserTaskRequestModel request, int userId);

    Task<List<UserTaskModeList>> GetTaskProjectChildren(int userId, int ParentId, UserTaskRequestModel request);

    Task ChangeStatusTaskForManager(int taskId, int statusForManager, int userId);
}
