using System.Text.Json;
using AutoMapper;
using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Hubs;
using ManageEmployee.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.Services;
public class UserTaskCommentService : IUserTaskCommentService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

    public UserTaskCommentService(ApplicationDbContext context, IMapper mapper, IHubContext<BroadcastHub, IHubClient> hubContext)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<UserTaskCommentModel> Add(UserTaskCommentModel entity, List<UserTaskFileModel> FileLink)
    {
        try
        {
            // Tạo mới comment
            var listbillTracking = await _context.BillTrackings.Where(x => x.UserIdReceived == entity.UserTaskId && !x.IsRead && x.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION).ToListAsync();

           
            var listbillTrackingDele = new List<BillTracking>();
            var userTask = await _context.UserTasks.FindAsync(entity.UserTaskId);              
            var user = await _context.Users.FindAsync(entity.UserId);
            var userTaskRoles = await _context.UserTaskRoleDetails.Where(x => x.UserTaskId == entity.UserTaskId).ToListAsync();

            userTaskRoles.ForEach(role =>
            {
                var billTracking = listbillTracking.Find(x => x.UserCode == role.UserId.ToString());
                if (billTracking != null)
                {
                    listbillTrackingDele.Add(billTracking);
                }

                if (role.UserId != entity.UserId)
                {
                    BillTracking billTrackingAdd = new BillTracking()
                    {
                        BillId = 0,
                        CustomerName = role.UserId.ToString(),
                        UserCode = role.UserId.ToString(),
                        UserIdReceived = entity.UserTaskId ?? 0,
                        Note = $"<strong>{userTask?.Name}</strong>" +
                               $"<br/>{user?.FullName}" +
                               $"<br/><p style='color:#32CD32'>Comment {DateTime.Now.ToString("dd/MM/yyyy : HH:mm")}:</p> " +
                               $"<br/>{entity.Comment}",
                        TranType = TranTypeConst.SendToStaff,
                        Status = "Success",
                        IsRead = false,
                        DisplayOrder = 0
                    };
                    _context.BillTrackings.Add(billTrackingAdd);
                }
            });

            _context.BillTrackings.RemoveRange(listbillTrackingDele);

            entity.Id = 0;
            var entityUserComment = _mapper.Map<UserTaskComment>(entity);
            entityUserComment.FileLink = entity.FileLink != null && entity.FileLink.Count > 0 ? JsonSerializer.Serialize(entity.FileLink) : "";
            entityUserComment.CreatedDate = DateTime.Now;
            await _context.UserTaskComments.AddAsync(entityUserComment);

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            // return kết quả
            return await GetById(entityUserComment.Id);

        }
        catch(Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }


    public async Task<UserTaskCommentModel> Edit(UserTaskCommentModel entity)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            UserTaskComment userTaskComment = await _context.UserTaskComments.FindAsync(entity.Id);

            if (userTaskComment != null)
            {
                userTaskComment.Comment = entity.Comment;
                userTaskComment.FileLink = entity.FileLink != null && entity.FileLink.Count > 0 ? JsonSerializer.Serialize(entity.FileLink) : "";
                _context.UserTaskComments.Update(userTaskComment);
                
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
            }

            // return kết quả
            return GetById(entity.Id).Result;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public Task<List<UserTaskCommentModel>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<UserTaskCommentModel> GetById(int id)
    {
        UserTaskCommentModel userTaskCommentModel = new UserTaskCommentModel();
        UserTaskComment userTaskComment = await _context.UserTaskComments.FindAsync(id);
        if (userTaskComment != null)
        {
            userTaskCommentModel.Id = userTaskComment.Id;
            userTaskCommentModel.UserTaskId = userTaskComment.UserTaskId;
            userTaskCommentModel.UserId = userTaskComment.UserId;
            userTaskCommentModel.Type = userTaskComment.Type;
            userTaskCommentModel.Comment = userTaskComment.Comment;
            userTaskCommentModel.ParentId = userTaskComment.ParentId;
            userTaskCommentModel.CreatedDate = userTaskComment.CreatedDate;
            userTaskCommentModel.FileLink = userTaskComment.FileLink != null && userTaskComment.FileLink != "" ? JsonSerializer.Deserialize<List<UserTaskFileModel>>(userTaskComment.FileLink) : new List<UserTaskFileModel>();
            userTaskCommentModel.NameOfUser = _context.Users.FirstOrDefault(x => x.Id == userTaskComment.UserId)?.FullName;
        }
        else
        {
            return null;
        }
        return userTaskCommentModel;
    }
    public async Task<List<UserTaskCommentModel>> GetByTaskId(int taskId, int userId)
    {

        var commnets = await _context.UserTaskComments
            .GroupJoin(_context.Users,
            task => task.UserId,
            user => user.Id,
            (t, u) => new { task = t, user = u })
            .SelectMany(x => x.user.DefaultIfEmpty(), (t, u) => new { task = t.task, user = u })
            .Where(t => t.task.UserTaskId == taskId && t.task.Type == "edit").Select(t => new UserTaskCommentModelFileLink
            {
                Comment = t.task.Comment,
                FileLink = t.task.FileLink,
                CreatedDate = t.task.CreatedDate,
                Id = t.task.Id,
                ParentId = t.task.ParentId,
                Type = t.task.Type,
                UserId = t.task.UserId,
                UserTaskId = t.task.UserTaskId,
                NameOfUser = t.user.FullName,
                Avatar = t.user.Avatar,
                IsAllowEdit = t.task.UserId == userId
            }).OrderByDescending(t => t.CreatedDate).ToListAsync();


        return commnets.ConvertAll(t => new UserTaskCommentModel
        {
            Comment = t.Comment,
            CreatedDate = t.CreatedDate,
            Id = t.Id,
            ParentId = t.ParentId,
            Type = t.Type,
            UserId = t.UserId,
            UserTaskId = t.UserTaskId,
            NameOfUser = t.NameOfUser,
            Avatar = t.Avatar,
            IsAllowEdit = t.UserId == userId,
            FileLink = string.IsNullOrEmpty(t.FileLink) ? null :JsonSerializer.Deserialize<List<UserTaskFileModel>>(t.FileLink)
        });
    }
}