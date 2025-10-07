using AutoMapper;
using Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services;

public class UserTaskService : IUserTaskService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserTaskCommentService _userTaskCommentService;

    public UserTaskService(ApplicationDbContext context, IMapper mapper, IUserTaskCommentService userTaskCommentService)
    {
        _context = context;
        _mapper = mapper;
        _userTaskCommentService = userTaskCommentService;
    }

    public async Task<UserTaskModel> Add(UserTask entityUserTask, List<UserTaskCheckList> entityCheckList,
         List<UserTaskRoleDetails> entityTaskRole, List<UserTaskFileModel> FileLink, int createdBy)
    {
        try
        {
            // Tạo mới công việc
            entityUserTask.Id = 0;
            entityUserTask.FileLink = FileLink != null && FileLink.Count > 0 ? JsonSerializer.Serialize(FileLink) : "";
            entityUserTask.CreatedDate = DateTime.Now;
            entityUserTask.Status = 0;
            entityUserTask.IsDeleted = false;
            entityUserTask.IsStatusForManager = 0;

            await _context.Database.BeginTransactionAsync();
            await _context.UserTasks.AddAsync(entityUserTask);
            await _context.SaveChangesAsync();
            // Tạo danh sách kiểm tra
            if (entityCheckList.Count > 0)
            {
                foreach (var item in entityCheckList)
                {
                    item.UserTaskId = entityUserTask.Id;
                    await _context.UserTaskCheckLists.AddAsync(item);
                }
            }
            // Tạo Task role
            var taskRoleAdds = new List<UserTaskRoleDetails>();
            if (entityTaskRole.Count > 0)
            {
                foreach (var item in entityTaskRole)
                {
                    item.UserTaskId = entityUserTask.Id;
                    taskRoleAdds.Add(item);
                }
            }
            if (!taskRoleAdds.Any(x => x.UserTaskRoleId == UserTaskRoleConst.Responsible && x.UserId > 0))
            {
                taskRoleAdds.Add(new UserTaskRoleDetails
                {
                    UserTaskId = entityUserTask.Id,
                    UserTaskRoleId = UserTaskRoleConst.Responsible,
                    UserId = createdBy,
                });
            }

            await _context.UserTaskRoleDetails.AddRangeAsync(taskRoleAdds);

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            // return kết quả
            return await GetById(entityUserTask.Id);
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<UserTaskModel> Get(int UserTaskId, int UserId)
    {
        var result = await GetById(UserTaskId);
        if (result != null)
        {
            await CreateUserTaskComment(UserTaskId, UserId);
        }
        return result;
    }

    public async Task<List<UserTaskModeList>> GetParentList(int UserId)
    {
        try
        {
            var taskList = _context.UserTasks.Where(
                 o => (o.UserCreated == UserId || o.ViewAll == true)
                 && o.Status != 3
                 && o.isProject
                 && o.IsDeleted == false);//&& o.ParentId == 0

            var tasklistByRole = (from task in _context.UserTasks
                                  join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                                  where role.UserId == UserId
                                          && task.IsDeleted == false
                                          && task.Status != 3
                                          && task.isProject
                                  select task
                                  );

            var resultUnion = await taskList.Union(tasklistByRole).Distinct().Select(
                 p => new UserTaskModeList()
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     CreatedDate = p.CreatedDate,
                     DueDate = p.DueDate,
                     ParentId = p.ParentId,
                     Status = p.Status,
                     UserCreated = p.UserCreated,
                     ViewAll = p.ViewAll,
                     IsDeleted = p.IsDeleted,
                 }
                 ).ToListAsync();

            return resultUnion;
        }
        catch
        {
            return new List<UserTaskModeList>();
        }
    }

    public async Task<UserTaskModel> Edit(UserTask entity, List<UserTaskCheckList> entityCheckList,
         List<UserTaskRoleDetails> entityTaskRole, List<UserTaskFileModel> FileLink)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            UserTask userTask = await _context.UserTasks.FindAsync(entity.Id);
            if (userTask != null)
            {
                userTask.CreatedDate = entity.CreatedDate ?? userTask.CreatedDate;
                userTask.Description = entity.Description;
                userTask.CustomerId = entity.CustomerId;
                userTask.DueDate = entity.DueDate;
                userTask.Name = entity.Name;
                userTask.ParentId = entity.ParentId;
                userTask.FileLink = entity.FileLink != null && FileLink.Count > 0 ? JsonSerializer.Serialize(FileLink) : "";
                userTask.UserCreated = entity.UserCreated;
                userTask.ViewAll = entity.ViewAll;
                userTask.DepartmentId = entity.DepartmentId;
                userTask.TypeWorkId = entity.TypeWorkId;
                userTask.Point = entity.Point;
                if(userTask.isProject && !entity.isProject)
                {
                    throw new ErrorException("Không thể bỏ check update dự án");
                }
                userTask.isProject = entity.isProject;
                _context.UserTasks.Update(userTask);

                List<UserTaskCheckList> checkList = _context.UserTaskCheckLists.Where(x => x.UserTaskId == entity.Id).ToList();
                _context.UserTaskCheckLists.RemoveRange(checkList);

                List<UserTaskRoleDetails> taskRole = _context.UserTaskRoleDetails.Where(x => x.UserTaskId == entity.Id).ToList();
                _context.UserTaskRoleDetails.RemoveRange(taskRole);

                // Tạo danh sách kiểm tra
                if (entityCheckList.Count > 0)
                {
                    foreach (var item in entityCheckList)
                    {
                        item.UserTaskId = entity.Id;
                        _context.UserTaskCheckLists.Add(item);
                    }
                }
                // Tạo Task role
                if (entityTaskRole.Count > 0)
                {
                    foreach (var item in entityTaskRole)
                    {
                        item.UserTaskId = entity.Id;
                        _context.UserTaskRoleDetails.Add(item);
                    }
                }

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return await GetById(entity.Id);
            }
            else
            {
                throw new ErrorException("Id công việc không tồn tại trong hệ thống.");
            }
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
        throw new NotImplementedException();
    }

    public async Task<UserTaskModel> Copy(int id)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            UserTask userTask = await _context.UserTasks.FindAsync(id);
            if (userTask != null)
            {
                userTask.Id = 0;
                userTask.Name = userTask.Name + "- Copy";
                await _context.UserTasks.AddAsync(userTask);
                await _context.SaveChangesAsync();

                List<UserTaskCheckList> checkList = _context.UserTaskCheckLists.Where(x => x.UserTaskId == id).ToList();
                List<UserTaskRoleDetails> taskRole = _context.UserTaskRoleDetails.Where(x => x.UserTaskId == id).ToList();

                // Tạo danh sách kiểm tra
                if (checkList.Count > 0)
                {
                    foreach (var item in checkList)
                    {
                        item.Id = 0;
                        item.UserTaskId = userTask.Id;
                        _context.UserTaskCheckLists.Add(item);
                    }
                }
                // Tạo Task role
                if (taskRole.Count > 0)
                {
                    foreach (var item in taskRole)
                    {
                        item.Id = 0;
                        item.UserTaskId = userTask.Id;
                        _context.UserTaskRoleDetails.Add(item);
                    }
                }

                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return await GetById(userTask.Id);
            }
            else
            {
                return null;
            }
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<UserTask> Delete(int id)
    {
        UserTask userTask = await _context.UserTasks.FindAsync(id);
        if (userTask != null)
        {
            userTask.IsDeleted = true;
            _context.UserTasks.Update(userTask);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ErrorException("Id công việc không tồn tại trong hệ thống.");
        }

        return userTask;
    }

    private async Task<UserTaskModel> GetById(int id)
    {
        UserTaskModel userTaskModel = new UserTaskModel();
        UserTask userTask = await _context.UserTasks.FindAsync(id);
        if (userTask != null)
        {
            var userCheckList = await _context.UserTaskCheckLists.Where(o => o.UserTaskId == id).Select(x => _mapper.Map<UserTaskCheckListModel>(x)).ToListAsync();
            var userTaskRole = await _context.UserTaskRoleDetails.Where(o => o.UserTaskId == id).Select(x => _mapper.Map<UserTaskRoleDetailsModel>(x)).ToListAsync();
            var childTask = await _context.UserTasks.Where(o => o.ParentId == id).ToListAsync();
            userTaskModel.CreatedDate = userTask.CreatedDate;
            userTaskModel.Description = userTask.Description;
            userTaskModel.DueDate = userTask.DueDate;
            userTaskModel.Id = userTask.Id;
            userTaskModel.IsDeleted = userTask.IsDeleted ?? false;
            userTaskModel.Name = userTask.Name;
            userTaskModel.ParentId = userTask.ParentId;
            userTaskModel.Status = userTask.Status;
            userTaskModel.UserCreated = userTask.UserCreated;
            userTaskModel.ViewAll = userTask.ViewAll;
            userTaskModel.CustomerId = userTask.CustomerId;
            userTaskModel.DepartmentId = userTask.DepartmentId;
            userTaskModel.TypeWorkId = userTask.TypeWorkId;
            userTaskModel.Point = userTask.Point;
            userTaskModel.isProject = userTask.isProject;
            userTaskModel.FileLink = userTask.FileLink != null
                && userTask.FileLink != "" ? JsonSerializer.Deserialize<List<UserTaskFileModel>>(userTask.FileLink) : new List<UserTaskFileModel>();
            userTaskModel.CheckList = userCheckList;
            userTaskModel.MinCheckList = userCheckList
                .Count(x => x.Status == true);
            userTaskModel.MaxCheckList = userCheckList.Count;
            userTaskModel.TaskRole = userTaskRole;
            userTaskModel.ChildTask = childTask;

            // add responsiblePerson
            userTaskModel.ResponsiblePerson = await GetResponsiblePerson(userTask.Id);
            userTaskModel.SupervisorPersons = await GetResponsiblePerson(userTask.Id, UserTaskRoleConst.Supervisor);
            userTaskModel.ParticipantPersons = await GetResponsiblePerson(userTask.Id, UserTaskRoleConst.Participant);
            userTaskModel.ResponsibleUserCreated = await GetResponsibleUser(userTask.UserCreated ?? 0);
        }
        else
        {
            return null;
        }
        return userTaskModel;
    }

    private async Task<UserTaskComment> CreateUserTaskComment(int UserTaskId, int UserId)
    {
        try
        {
            UserTaskComment taskCommnet = new UserTaskComment()
            {
                Id = 0,
                UserId = UserId,
                UserTaskId = UserTaskId,
                CreatedDate = DateTime.Now,
                Type = "view",
                Comment = "",
                ParentId = 0
            };

            _context.Database.BeginTransaction();
            await _context.UserTaskComments.AddAsync(taskCommnet);

            UserTask task = await _context.UserTasks.Where(o => o.Id == UserTaskId).FirstOrDefaultAsync();
            if (task is null)
            {
                _context.Database.RollbackTransaction();
                return null;
            }
            task.Viewer = task.Viewer + 1;
            _context.UserTasks.Update(task);

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return taskCommnet;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<PagingResult<UserTaskModeList>> GetListMode(UserTaskRequestModel request, int userId)
    {
        try
        {
            DateTime today = DateTime.Now.Date;
            DateTime firstDayThisWeek = FirstDayOfWeek(today).Date;
            DateTime lastDayThisWeek = LastDayOfWeek(today).Date;
            DateTime firstDayNextWeek = firstDayThisWeek.AddDays(7).Date;
            DateTime lastDayNextWeek = lastDayThisWeek.AddDays(7).Date;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var dateFrom = request.StartDate ?? firstDayOfMonth;
            var dateTo = request.EndDate ?? lastDayOfMonth;

            var taskRoleUserQueryable = await _context.UserTaskRoleDetails
                .Where(x => x.UserId == userId)
                .Select(x => x.UserTaskId).ToListAsync();

            var superVisorRoleIds = await _context.UserTaskRoleDetails
                .Where(x => x.UserId == userId && x.UserTaskRoleId == UserTaskRoleConst.Supervisor)
                .Select(x => x.UserTaskId)
                .ToListAsync();

            var isExistTask = await _context.UserTasks.Where(o => o.UserCreated == userId && o.Status == (int)TaskStatusEnum.Processing && o.IsDeleted == false).AnyAsync();

            var tasksQueryable = (from task in _context.UserTasks
                                  join user in _context.Users on task.UserCreated equals user.Id
                                  join parent in _context.UserTasks on task.ParentId equals parent.Id into tmpParent
                                  from qParent in tmpParent.DefaultIfEmpty()
                                  join pin in _context.UserTaskPins on new { TaskId = task.Id, UserId = userId } equals new
                                  { TaskId = pin.UserTaskId, UserId = pin.UserId } into tmpPin
                                  from qPin in tmpPin.DefaultIfEmpty()
                                  where (task.UserCreated == userId || task.ViewAll == true || taskRoleUserQueryable.Contains(task.Id)) &&
                                        task.IsDeleted == false &&
                                        (task.CreatedDate.Value.Date >= dateFrom.Date || task.Status == (int)TaskStatusEnum.Processing) &&
                                        task.CreatedDate.Value.Date <= dateTo.Date &&
                                        (request.Statuses == null || !request.Statuses.Any() || request.Statuses.Contains(task.Status ?? 0)) &&
                                        (request.DepartmentId == null || request.DepartmentId == 0 || task.DepartmentId == request.DepartmentId) &&
                                        (request.ParentProjectId == null || request.ParentProjectId == 0 || task.ParentId == request.ParentProjectId) &&
                                        (request.UserCreatedId == null || request.UserCreatedId == 0 || task.UserCreated == request.UserCreatedId) &&
                                        (request.CustomerId == null || request.CustomerId == 0 || task.CustomerId == request.CustomerId) &&
                                        (!request.IsExpired || task.Status != (int)TaskStatusEnum.Done) &&
                                        (string.IsNullOrEmpty(request.SearchText) || EF.Functions.Like(task.Name, $"%{request.SearchText}%") || EF.Functions.Like(task.Id.ToString(), $"%{request.SearchText}%")) &&
                                        (request.TaskType == TaskTypeEnum.All || (request.TaskType == TaskTypeEnum.Project && task.isProject) || (request.TaskType == TaskTypeEnum.Task && !task.isProject))
                                  orderby qPin.Id descending, task.CreatedDate descending
                                  select new UserTaskModeList
                                  {
                                      Id = task.Id,
                                      Name = task.Name,
                                      Status = task.Status,
                                      UserCreated = task.UserCreated,
                                      ViewAll = task.ViewAll,
                                      DueDate = task.DueDate,
                                      Description = task.Description,
                                      IsDeleted = task.IsDeleted,
                                      CreatedDate = task.CreatedDate,
                                      ParentId = task.ParentId,
                                      Project = qParent.Name,
                                      CreatePerson = user.FullName,
                                      OrderNo = qPin.Id,
                                      DueDateMode = "",
                                      Activity = "",
                                      ResponsiblePerson = new List<ResponsiblePerson>(),
                                      ResponsibleUserCreated = new ResponsiblePerson(),
                                      Viewer = task.Viewer,
                                      IsStatusForManager = task.IsStatusForManager,
                                      IsSupervisor = superVisorRoleIds.Contains(task.Id),
                                      ActualHours = 0, //qActualHour.TotalHours
                                      MinCheckList = 0,
                                      MaxCheckList = 0,
                                      IsExistTask = isExistTask
                                  }
                );

            var data = request.Page > 0 ? tasksQueryable.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
                : tasksQueryable.ToList();

            foreach (var item in data)
            {
                item.MinCheckList = _context.UserTaskCheckLists.Where(x => x.UserTaskId == item.Id && x.Status == true).Count();
                item.MaxCheckList = _context.UserTaskCheckLists.Where(x => x.UserTaskId == item.Id).Count();
                // role task: 1 trách nhiệm, 2 Tham gia, 3 quan sat
                item.ResponsiblePerson = await GetResponsiblePerson(item.Id);
                item.SupervisorPersons = await GetResponsiblePerson(item.Id, UserTaskRoleConst.Supervisor);
                item.ParticipantPersons = await GetResponsiblePerson(item.Id, UserTaskRoleConst.Participant);
                item.ResponsibleUserCreated = await GetResponsibleUser(item.UserCreated ?? 0);
                item.ActualHours = Convert.ToDouble(String.Format("{0:0.0000}", getActualHours(item.Id)));
                item.Activity = GetActivate(Convert.ToDateTime(item.CreatedDate));

                if (item.DueDate.HasValue)
                {
                    DateTime _dueDate = Convert.ToDateTime(item.DueDate).Date;
                    if (_dueDate < today)
                    {
                        item.DueDateMode = "overdue";
                    }
                    else if (_dueDate == today)
                    {
                        item.DueDateMode = "today";
                    }
                    else if (_dueDate > today && _dueDate <= lastDayThisWeek)
                    {
                        item.DueDateMode = "thisweek";
                    }
                    else if (_dueDate >= firstDayNextWeek && _dueDate <= lastDayNextWeek)
                    {
                        item.DueDateMode = "nextweek";
                    }
                }
                else
                {
                    item.DueDateMode = "Indefinite-term";
                }
            }

            return new PagingResult<UserTaskModeList>
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = await tasksQueryable.CountAsync(),
                Data = data
            };
        }
        catch
        {
            return new PagingResult<UserTaskModeList>
            {
                CurrentPage = 0,
                PageSize = 0,
                TotalItems = 0,
                Data = new List<UserTaskModeList>()
            };
        }
    }

    public async Task<PagingResult<UserTaskModeList>> GetDueDateMode(UserTaskRequestModel request, int UserId)
    {
        try
        {
            DateTime today = DateTime.Now.Date;
            DateTime FirstDayThisWeek = FirstDayOfWeek(today).Date;
            DateTime LastDayThisWeek = LastDayOfWeek(today).Date;
            DateTime FirstDayNextWeek = FirstDayThisWeek.AddDays(7).Date;
            DateTime LastDayNextWeek = LastDayThisWeek.AddDays(7).Date;

            // Qua han
            var taskListByIdOverDue = _context.UserTasks.Where(
                o => (o.UserCreated == UserId || o.ViewAll == true)
                && ((DateTime)o.DueDate).Date < today
                && o.IsDeleted == false
                && o.Status != 3);

            var tasklistByRoleOverDue = (from task in _context.UserTasks
                                         join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                                         where role.UserId == UserId
                                         && ((DateTime)task.DueDate).Date < today
                                         && task.IsDeleted == false
                                         && task.Status != 3
                                         select task
                                  );

            var resulOverDue = await taskListByIdOverDue.Union(tasklistByRoleOverDue).Distinct().Select(
                 p => new UserTaskModeList()
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     CreatedDate = p.CreatedDate,
                     DueDate = p.DueDate,
                     ParentId = p.ParentId,
                     Status = p.Status,
                     UserCreated = p.UserCreated,
                     ViewAll = p.ViewAll,
                     IsDeleted = p.IsDeleted,
                     DueDateMode = "overdue"
                 }
                 ).ToListAsync();

            // Hom nay
            var taskListByIdToDay = _context.UserTasks.Where(
               o => (o.UserCreated == UserId || o.ViewAll == true)
               && ((DateTime)o.DueDate).Date == today
               && o.IsDeleted == false
               && o.Status != 3);

            var tasklistByRoleToDay = (from task in _context.UserTasks
                                       join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                                       where role.UserId == UserId
                                       && ((DateTime)task.DueDate).Date == today
                                       && task.IsDeleted == false
                                       && task.Status != 3
                                       select task
                                  );

            var resulToDay = await taskListByIdToDay.Union(tasklistByRoleToDay).Distinct().Select(
                 p => new UserTaskModeList()
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     CreatedDate = p.CreatedDate,
                     DueDate = p.DueDate,
                     ParentId = p.ParentId,
                     Status = p.Status,
                     UserCreated = p.UserCreated,
                     ViewAll = p.ViewAll,
                     IsDeleted = p.IsDeleted,
                     DueDateMode = "today"
                 }
                 ).ToListAsync();

            // trong tuan
            var taskListByIdThisWeek = _context.UserTasks.Where(
              o => (o.UserCreated == UserId || o.ViewAll == true)
              && ((DateTime)o.DueDate).Date > today && ((DateTime)o.DueDate).Date <= LastDayThisWeek
              && o.IsDeleted == false
              && o.Status != 3);

            var tasklistByRoleThisWeek = (from task in _context.UserTasks
                                          join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                                          where role.UserId == UserId
                                          && ((DateTime)task.DueDate).Date > today && ((DateTime)task.DueDate).Date <= LastDayThisWeek
                                          && task.IsDeleted == false
                                          && task.Status != 3
                                          select task
                                  );

            var resulThisWeek = await taskListByIdThisWeek.Union(tasklistByRoleThisWeek).Distinct().Select(
                 p => new UserTaskModeList()
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     CreatedDate = p.CreatedDate,
                     DueDate = p.DueDate,
                     ParentId = p.ParentId,
                     Status = p.Status,
                     UserCreated = p.UserCreated,
                     ViewAll = p.ViewAll,
                     IsDeleted = p.IsDeleted,
                     DueDateMode = "thisweek"
                 }
                 ).ToListAsync();

            // Tuan toi
            var taskListByIdNextWeek = _context.UserTasks.Where(
              o => (o.UserCreated == UserId || o.ViewAll == true)
              && ((DateTime)o.DueDate).Date >= FirstDayNextWeek && ((DateTime)o.DueDate).Date <= LastDayNextWeek
              && o.IsDeleted == false
              && o.Status != 3);

            var tasklistByRoleNextWeek = (from task in _context.UserTasks
                                          join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                                          where role.UserId == UserId
                                          && ((DateTime)task.DueDate).Date >= FirstDayNextWeek && ((DateTime)task.DueDate).Date <= LastDayNextWeek
                                          && task.IsDeleted == false
                                          && task.Status != 3
                                          select task
                                  );

            var resulNextWeek = await taskListByIdNextWeek.Union(tasklistByRoleNextWeek).Distinct().Select(
                 p => new UserTaskModeList()
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     CreatedDate = p.CreatedDate,
                     DueDate = p.DueDate,
                     ParentId = p.ParentId,
                     Status = p.Status,
                     UserCreated = p.UserCreated,
                     ViewAll = p.ViewAll,
                     IsDeleted = p.IsDeleted,
                     DueDateMode = "nextweek"
                 }
                 ).ToListAsync();

            // Tuan toi
            var taskListByIdIndefiniteTerm = _context.UserTasks.Where(
              o => (o.UserCreated == UserId || o.ViewAll == true)
              && o.IsDeleted == false
              && (o.Status ?? 0) == 0);

            var tasklistByRoleIndefiniteTerm = (from task in _context.UserTasks
                                                join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                                                where role.UserId == UserId
                                                && task.IsDeleted == false
                                                && (task.Status ?? 0) == 0
                                                select task
                                  );

            var resultIndefiniteTerm = await taskListByIdIndefiniteTerm.Union(tasklistByRoleIndefiniteTerm).Distinct().Select(
                 p => new UserTaskModeList()
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Description = p.Description,
                     CreatedDate = p.CreatedDate,
                     DueDate = p.DueDate,
                     ParentId = p.ParentId,
                     Status = p.Status,
                     UserCreated = p.UserCreated,
                     ViewAll = p.ViewAll,
                     IsDeleted = p.IsDeleted,
                     DueDateMode = "Indefinite-term"
                 }
                 ).ToListAsync();

            // tong hop

            List<UserTaskModeList> result = resulOverDue.Union(resulToDay)
                .Union(resulThisWeek)
                .Union(resulNextWeek)
                .Union(resultIndefiniteTerm)
                .Distinct()
                .OrderBy(o => o.DueDateMode)
                .ThenBy(o => o.CreatedDate)
                .ToList();
            return new PagingResult<UserTaskModeList>()
            {
                CurrentPage = request.Page,
                PageSize = request.PageSize,
                TotalItems = result.Count,
                Data = result
            };
        }
        catch
        {
            return new PagingResult<UserTaskModeList>()
            {
                CurrentPage = 0,
                PageSize = 0,
                TotalItems = 0,
                Data = new List<UserTaskModeList>()
            };
        }
    }

    public async Task PinTask(int UserTaskId, int UserId)
    {
        UserTask uTask = _context.UserTasks.Find(UserTaskId);
        if (uTask == null)
        {
            throw new ErrorException("Id công việc không tồn tại");
        }

        UserTaskPin userTask = _context.UserTaskPins.Where(o => o.UserId == UserId && o.UserTaskId == UserTaskId).FirstOrDefault();
        if (userTask == null)
        {
            UserTaskPin uTaskPin = new UserTaskPin()
            {
                Id = 0,
                UserId = UserId,
                UserTaskId = UserTaskId,
                OrderNo = 0
            };
            await _context.UserTaskPins.AddAsync(uTaskPin);
        }
        else
        {
            _context.UserTaskPins.Remove(userTask);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<UserTask> StatusTask(UserTaskStatusModel status, int UserId)
    {
        UserTask userTask = await _context.UserTasks.FirstOrDefaultAsync(o => o.Id == status.UserTaskId);

        if (userTask == null)
        {
            throw new ErrorException("Id công việc không tồn tại");
        }
        else
        {
            if (status.Status == 1)
            {
                var isExistTask = await _context.UserTasks.Where(o => o.UserCreated == UserId && o.Status == status.Status && o.IsDeleted == false && o.Id != status.UserTaskId).AnyAsync();
                if (isExistTask)
                    throw new ErrorException("Đang tồn tại công việc đang bắt đầu");
                userTask.IsStatusForManager = 0;
            }

            if (status.Status > 0)
            {
                await TrackingTask(status.UserTaskId ?? 0, status.Status ?? 0, UserId, userTask.Status);
            }

            userTask.Status = status.Status;
            _context.UserTasks.Update(userTask);
            await _context.SaveChangesAsync();
        }
        return userTask;
    }

    public async Task<string> TrackingTask(int UserTaskId, int IsStart, int UserId, int? currentStatus)
    {
        string result = "";
        try
        {
            await _context.Database.BeginTransactionAsync();

            if (IsStart == 2)
            {
                UserTaskTracking taskTracking = await _context.UserTaskTrackings.Where(o => o.UserTaskId == UserTaskId && o.EndDate == null).OrderByDescending(o => o.CreatedDate).Take(1).FirstOrDefaultAsync();
                if (currentStatus == (int)TaskStatusEnum.Reviewing)
                {
                    if (taskTracking is null)
                        taskTracking = new UserTaskTracking();
                }
                else if (taskTracking == null)
                {
                    throw new ErrorException("Công việc chưa bắt đầu. Không thể cập nhật thời gian hoãn lại");
                }

                taskTracking.EndDate = DateTime.Now;
                taskTracking.UserUpdated = UserId;
                taskTracking.UpdateDate = DateTime.Now;
                _context.Entry(taskTracking).Property(x => x.ActualHours).IsModified = false;
                _context.UserTaskTrackings.Update(taskTracking);
                result = "Trạng thái hoãn lại công việc cập nhật thành công.";
            }
            else if (IsStart == 1)
            {
                UserTaskTracking taskTracking = await _context.UserTaskTrackings.Where(o => o.UserTaskId == UserTaskId).OrderByDescending(o => o.CreatedDate).Take(1).FirstOrDefaultAsync();
                if (taskTracking == null || taskTracking.EndDate != null)
                {
                    taskTracking = new UserTaskTracking()
                    {
                        StartDate = DateTime.Now,
                        EndDate = null,
                        UserTaskId = UserTaskId,
                        UserCreated = UserId,
                        CreatedDate = DateTime.Now,
                        UpdateDate = null,
                        UserUpdated = null
                    };
                    _context.Entry(taskTracking).Property(x => x.ActualHours).IsModified = false;
                    _context.UserTaskTrackings.Add(taskTracking);
                    result = "Trạng thái bắt đầu công việc cập nhật thành công.";
                }
                else if (taskTracking.EndDate == null)
                {
                    throw new ErrorException("Công việc đã ở trạng thái bắt đầu.");
                }
            }
            else
            {
                UserTaskTracking taskTracking = await _context.UserTaskTrackings.Where(o => o.UserTaskId == UserTaskId && o.EndDate == null).OrderByDescending(o => o.CreatedDate).Take(1).FirstOrDefaultAsync();
                if (taskTracking == null)
                {
                    DateTime dateHoanThanh = DateTime.Now;

                    taskTracking = new UserTaskTracking()
                    {
                        StartDate = dateHoanThanh,
                        EndDate = dateHoanThanh,
                        UserTaskId = UserTaskId,
                        UserCreated = UserId,
                        CreatedDate = DateTime.Now,
                        UpdateDate = null,
                        UserUpdated = null
                    };
                    _context.Entry(taskTracking).Property(x => x.ActualHours).IsModified = false;
                    _context.UserTaskTrackings.Update(taskTracking);
                    result = "Trạng thái hoàn thành công việc cập nhật thành công.";
                }
                else
                {
                    taskTracking.EndDate = DateTime.Now;
                    taskTracking.UserUpdated = UserId;
                    taskTracking.UpdateDate = DateTime.Now;
                    _context.Entry(taskTracking).Property(x => x.ActualHours).IsModified = false;
                    _context.UserTaskTrackings.Update(taskTracking);

                    result = "Trạng thái hoàn thành công việc cập nhật thành công.";
                }
            }
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return result;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }

    #region Private

    private static DateTime FirstDayOfWeek(DateTime date)
    {
        DayOfWeek fdow = DayOfWeek.Monday;
        int offset = fdow - date.DayOfWeek;
        DateTime fdowDate = date.AddDays(offset);
        return fdowDate;
    }

    private static DateTime LastDayOfWeek(DateTime date)
    {
        DateTime ldowDate = FirstDayOfWeek(date).AddDays(6);
        return ldowDate;
    }

    private async Task<List<ResponsiblePerson>> GetResponsiblePerson(int UserTaskId, int userTaskRole = UserTaskRoleConst.Responsible)
    {
        var result = await (from role in _context.UserTaskRoleDetails
                            join user in _context.Users on role.UserId equals user.Id
                            where role.UserTaskId == UserTaskId && role.UserTaskRoleId == userTaskRole
                            select new ResponsiblePerson { FullName = user.FullName, Avatar = user.Avatar, UserId = user.Id }).ToListAsync();
        return result;
    }

    private async Task<ResponsiblePerson> GetResponsibleUser(int UserId)
    {
        var result = await (from user in _context.Users
                            where user.Id == UserId
                            select new ResponsiblePerson { FullName = user.FullName, Avatar = user.Avatar }).FirstOrDefaultAsync();
        return result;
    }

    private double getActualHours(int UserTaskId)
    {
        var result = _context.UserTaskTrackings.Where(o => o.UserTaskId == UserTaskId).Sum(o => o.ActualHours ?? 0);

        return result;
    }

    private string GetActivate(DateTime createDate)
    {
        string buoi = "sáng";
        if (createDate.Hour > 12 && createDate.Hour <= 18)
        {
            buoi = "chiều";
        }
        else if (createDate.Hour > 18)
        {
            buoi = "tối";
        }

        return String.Format("{0} tháng {1}, {2}:{3} {4}", createDate.Day, createDate.Month, createDate.Hour, createDate.Minute, buoi);
    }

    #endregion Private

    public Task<PagingResult<UserTask>> GetListTaskProject(UserTaskRequestModel request, int userId)
    {
        var listTask = _context.UserTasks.Where(x => x.isProject && x.Status != 3
        && (x.UserCreated == userId || x.ViewAll == true) && x.IsDeleted == false);

        var tasklistByRole = (from task in _context.UserTasks
                              join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                              where role.UserId == userId
                              && task.IsDeleted == false
                              && task.isProject
                              && task.Status != 3
                              select task);

        var resultUnion = listTask.Union(tasklistByRole).Distinct();

        return Task.FromResult(new PagingResult<UserTask>
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalItems = resultUnion.Count(),
            Data = request.PageSize > 0 ? resultUnion.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList()
               : resultUnion.ToList()
        });
    }

    public async Task<PagingResult<UserTaskModeList>> GetTaskProjectParent(UserTaskRequestModel request, int userId)
    {

        var taskListByUserId = (from task in _context.UserTasks
                                join user in _context.Users on task.UserCreated equals user.Id
                                join parent in _context.UserTasks on task.ParentId equals parent.Id into tmpParent
                                from qParent in tmpParent.DefaultIfEmpty()
                                join pin in _context.UserTaskPins on new { TaskId = task.Id, UserId = userId } equals new { TaskId = pin.UserTaskId, UserId = pin.UserId } into tmpPin
                                from qPin in tmpPin.DefaultIfEmpty()
                                where
                                    task.isProject &&
                                    (task.UserCreated == userId || task.ViewAll == true) &&
                                    task.IsDeleted == false &&
                                    (task.ParentId == null || task.ParentId == 0) &&

                                   (request.Statuses == null || !request.Statuses.Any() || request.Statuses.Contains(task.Status ?? 0)) &&
                                   (request.DepartmentId == null || request.DepartmentId == 0 || task.DepartmentId == request.DepartmentId) &&
                                   (request.ParentProjectId == null || request.ParentProjectId == 0 || task.ParentId == request.ParentProjectId) &&
                                   (request.UserCreatedId == null || request.UserCreatedId == 0 || task.UserCreated == request.UserCreatedId) &&
                                   (request.CustomerId == null || request.CustomerId == 0 || task.CustomerId == request.CustomerId) &&
                                   (string.IsNullOrEmpty(request.SearchText) || EF.Functions.Like(task.Name, $"%{request.SearchText}%") || EF.Functions.Like(task.Id.ToString(), $"%{request.SearchText}%"))

                                select new UserTaskModeList
                                {
                                    Id = task.Id,
                                    Name = task.Name,
                                    Status = task.Status,
                                    UserCreated = task.UserCreated,
                                    ViewAll = task.ViewAll,
                                    DueDate = task.DueDate,
                                    Description = task.Description,
                                    IsDeleted = task.IsDeleted,
                                    CreatedDate = task.CreatedDate,
                                    ParentId = task.ParentId,
                                    Project = qParent.Name,
                                    CreatePerson = user.FullName,
                                    OrderNo = qPin.Id == 0 ? 99999 : qPin.Id,
                                    DueDateMode = "",
                                    Activity = "",
                                    ResponsiblePerson = new List<ResponsiblePerson>(),
                                    ResponsibleUserCreated = new ResponsiblePerson(),
                                    Viewer = task.Viewer,
                                    IsStatusForManager = task.IsStatusForManager,
                                    ActualHours = 0//qActualHour.TotalHours
                                });

        var tasklistByRole = (from task in _context.UserTasks
                              join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                              join user in _context.Users on task.UserCreated equals user.Id
                              join parent in _context.UserTasks on task.ParentId equals parent.Id into tmpParent
                              from qParent in tmpParent.DefaultIfEmpty()
                              join pin in _context.UserTaskPins on new { TaskId = task.Id, UserId = userId } equals new { TaskId = pin.UserTaskId, UserId = pin.UserId } into tmpPin
                              from qPin in tmpPin.DefaultIfEmpty()
                              where role.UserId == userId
                                && task.IsDeleted == false
                                && task.isProject
                                && (task.ParentId == null || task.ParentId == 0)

                               && (request.Statuses == null || !request.Statuses.Any() || request.Statuses.Contains(task.Status ?? 0)) &&
                                   (request.DepartmentId == null || request.DepartmentId == 0 || task.DepartmentId == request.DepartmentId) &&
                                   (request.ParentProjectId == null || request.ParentProjectId == 0 || task.ParentId == request.ParentProjectId) &&
                                   (request.UserCreatedId == null || request.UserCreatedId == 0 || task.UserCreated == request.UserCreatedId) &&
                                   (request.CustomerId == null || request.CustomerId == 0 || task.CustomerId == request.CustomerId) &&
                                   (string.IsNullOrEmpty(request.SearchText) || EF.Functions.Like(task.Name, $"%{request.SearchText}%") || EF.Functions.Like(task.Id.ToString(), $"%{request.SearchText}%"))


                              select new UserTaskModeList
                              {
                                  Id = task.Id,
                                  Name = task.Name,
                                  Status = task.Status,
                                  UserCreated = task.UserCreated,
                                  ViewAll = task.ViewAll,
                                  DueDate = task.DueDate,
                                  Description = task.Description,
                                  IsDeleted = task.IsDeleted,
                                  CreatedDate = task.CreatedDate,
                                  ParentId = task.ParentId,
                                  Project = qParent.Name,
                                  CreatePerson = user.FullName,
                                  OrderNo = qPin.Id == 0 ? 99999 : qPin.Id,
                                  DueDateMode = "",
                                  Activity = "",
                                  ParticipantPersons = new List<ResponsiblePerson>(),
                                  ResponsiblePerson = new List<ResponsiblePerson>(),
                                  ResponsibleUserCreated = new ResponsiblePerson(),
                                  Viewer = task.Viewer,
                                  IsStatusForManager = task.IsStatusForManager,

                                  ActualHours = 0 //qActualHour.TotalHours
                              });

        var resultUnion = taskListByUserId.Union(tasklistByRole).Distinct();
        var listOut = await resultUnion.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        foreach (var item in listOut)
        {
            item.ParticipantPersons = await GetResponsiblePerson(item.Id, UserTaskRoleConst.Participant);
            item.ResponsiblePerson = await GetResponsiblePerson(item.Id);
            item.ResponsibleUserCreated = await GetResponsibleUser(item.UserCreated ?? 0);
            item.SupervisorPersons = await GetResponsiblePerson(item.Id, UserTaskRoleConst.Supervisor);

            item.ActualHours = Convert.ToDouble(String.Format("{0:0.0000}", getActualHours(item.Id)));
            item.Activity = GetActivate(Convert.ToDateTime(item.CreatedDate));
            item.IsChildren = await _context.UserTasks.AnyAsync(x => x.ParentId == item.Id); // Co con hay khong
        }

        return new PagingResult<UserTaskModeList>
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
            TotalItems = resultUnion.Count(),
            Data = listOut
        };
    }

    public async Task<List<UserTaskModeList>> GetTaskProjectChildren(int userId, int parentId, UserTaskRequestModel request)
    {
        var taskListByUserId = (from task in _context.UserTasks
                                join user in _context.Users on task.UserCreated equals user.Id
                                join parent in _context.UserTasks on task.ParentId equals parent.Id into tmpParent
                                from qParent in tmpParent.DefaultIfEmpty()
                                join pin in _context.UserTaskPins on new { TaskId = task.Id, UserId = userId } equals new { TaskId = pin.UserTaskId, UserId = pin.UserId } into tmpPin
                                from qPin in tmpPin.DefaultIfEmpty()
                                where
                                    (task.UserCreated == userId || task.ViewAll == true) &&
                                    task.IsDeleted == false &&
                                    task.ParentId == parentId &&

                                    (request.Statuses == null || !request.Statuses.Any() || request.Statuses.Contains(task.Status ?? 0)) &&
                                   (request.DepartmentId == null || request.DepartmentId == 0 || task.DepartmentId == request.DepartmentId) &&
                                   (request.ParentProjectId == null || request.ParentProjectId == 0 || task.ParentId == request.ParentProjectId) &&
                                   (request.UserCreatedId == null || request.UserCreatedId == 0 || task.UserCreated == request.UserCreatedId) &&
                                   (request.CustomerId == null || request.CustomerId == 0 || task.CustomerId == request.CustomerId) &&
                                   (string.IsNullOrEmpty(request.SearchText) || EF.Functions.Like(task.Name, $"%{request.SearchText}%") || EF.Functions.Like(task.Id.ToString(), $"%{request.SearchText}%"))

                                select new UserTaskModeList
                                {
                                    Id = task.Id,
                                    Name = task.Name,
                                    Status = task.Status,
                                    UserCreated = task.UserCreated,
                                    ViewAll = task.ViewAll,
                                    DueDate = task.DueDate,
                                    Description = task.Description,
                                    IsDeleted = task.IsDeleted,
                                    CreatedDate = task.CreatedDate,
                                    ParentId = task.ParentId,
                                    Project = qParent.Name,
                                    CreatePerson = user.FullName,
                                    OrderNo = qPin.Id == 0 ? 99999 : qPin.Id,
                                    DueDateMode = "",
                                    Activity = "",
                                    ResponsiblePerson = new List<ResponsiblePerson>(),
                                    ResponsibleUserCreated = new ResponsiblePerson(),
                                    Viewer = task.Viewer,
                                    IsStatusForManager = task.IsStatusForManager,

                                    ActualHours = 0//qActualHour.TotalHours
                                }
                                                );

        var tasklistByRole = (from task in _context.UserTasks
                              join role in _context.UserTaskRoleDetails on task.Id equals role.UserTaskId
                              join user in _context.Users on task.UserCreated equals user.Id
                              join parent in _context.UserTasks on task.ParentId equals parent.Id into tmpParent
                              from qParent in tmpParent.DefaultIfEmpty()
                              join pin in _context.UserTaskPins on new { TaskId = task.Id, UserId = userId } equals new { TaskId = pin.UserTaskId, UserId = pin.UserId } into tmpPin
                              from qPin in tmpPin.DefaultIfEmpty()
                              where role.UserId == userId
                                && task.IsDeleted == false
                                && task.Status != 3
                                && task.ParentId == parentId &&


                                 (request.Statuses == null || !request.Statuses.Any() || request.Statuses.Contains(task.Status ?? 0)) &&
                                   (request.DepartmentId == null || request.DepartmentId == 0 || task.DepartmentId == request.DepartmentId) &&
                                   (request.ParentProjectId == null || request.ParentProjectId == 0 || task.ParentId == request.ParentProjectId) &&
                                   (request.UserCreatedId == null || request.UserCreatedId == 0 || task.UserCreated == request.UserCreatedId) &&
                                   (request.CustomerId == null || request.CustomerId == 0 || task.CustomerId == request.CustomerId) &&
                                   (string.IsNullOrEmpty(request.SearchText) || EF.Functions.Like(task.Name, $"%{request.SearchText}%") || EF.Functions.Like(task.Id.ToString(), $"%{request.SearchText}%"))


                              select new UserTaskModeList
                              {
                                  Id = task.Id,
                                  Name = task.Name,
                                  Status = task.Status,
                                  UserCreated = task.UserCreated,
                                  ViewAll = task.ViewAll,
                                  DueDate = task.DueDate,
                                  Description = task.Description,
                                  IsDeleted = task.IsDeleted,
                                  CreatedDate = task.CreatedDate,
                                  ParentId = task.ParentId,
                                  Project = qParent.Name,
                                  CreatePerson = user.FullName,
                                  OrderNo = qPin.Id == 0 ? 99999 : qPin.Id,
                                  DueDateMode = "",
                                  Activity = "",
                                  ResponsiblePerson = new List<ResponsiblePerson>(),
                                  ResponsibleUserCreated = new ResponsiblePerson(),
                                  Viewer = task.Viewer,
                                  IsStatusForManager = task.IsStatusForManager,

                                  ActualHours = 0//qActualHour.TotalHours
                              });

        var resultUnion = taskListByUserId.Union(tasklistByRole).Distinct().ToList();
        foreach (var item in resultUnion)
        {
            item.SupervisorPersons = await GetResponsiblePerson(item.Id, UserTaskRoleConst.Supervisor);
            item.ParticipantPersons = await GetResponsiblePerson(item.Id, UserTaskRoleConst.Participant);
            item.ResponsiblePerson = await GetResponsiblePerson(item.Id);
            item.ResponsibleUserCreated = await GetResponsibleUser(item.UserCreated ?? 0);
            item.ActualHours = Convert.ToDouble(String.Format("{0:0.0000}", getActualHours(item.Id)));
            item.Activity = GetActivate(Convert.ToDateTime(item.CreatedDate));
            item.IsChildren = await _context.UserTasks.AnyAsync(x => x.ParentId == item.Id);
        }

        return resultUnion;
    }

    public async Task ChangeStatusTaskForManager(int taskId, int statusForManager, int userId)
    {
        var task = await _context.UserTasks.FindAsync(taskId);
        if (task == null)
        {
            return;
        }
        task.UserManagerId = userId;
        task.ManagerUpdateDate = DateTime.Now;
        task.IsStatusForManager = statusForManager;
        _context.UserTasks.Update(task);
        var status = await _context.Status.FindAsync(statusForManager);
        var user = await _context.Users.FindAsync(userId);
        // add comment
        UserTaskCommentModel comment = new UserTaskCommentModel
        {
            Comment = $"<p>{status?.Name}</p>",
            CreatedDate = DateTime.Now,
            FileLink = new List<UserTaskFileModel>(),
            TaskRole = new List<UserTaskRoleDetailsModel>(),
            Type = "edit",
            UserTaskId = taskId,
            UserId = userId,
            IsAllowEdit = true,
            NameOfUser = user?.FullName,
            ParentId = 0
        };
        List<UserTaskFileModel> file = new List<UserTaskFileModel>();

        await _userTaskCommentService.Add(comment, file);
        await _context.SaveChangesAsync();
    }
}