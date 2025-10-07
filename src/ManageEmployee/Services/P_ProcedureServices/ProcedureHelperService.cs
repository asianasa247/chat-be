using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Entities.ProcedureEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;

public class ProcedureHelperService : IProcedureHelperService
{
    private readonly ApplicationDbContext _context;

    public ProcedureHelperService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<int?>> GetProcedureStatusIds(int userId, ProduceProductStatusTab statusTab, string produceCode)
    {
        // check role procedure
        // check tab
        // find status for current user
        var user = await _context.Users.FindAsync(userId);
        var userRoleIds = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();
        var produce = await _context.P_Procedure.FirstOrDefaultAsync(x => x.Code == produceCode);
        if (produce is null)
        {
            throw new ErrorException(string.Format("Vui lòng liên hệ quản tri để thêm mã quy trình {0}", produceCode));
        }
        var statusIds = await _context.P_ProcedureStatusRole.Where(x => x.P_ProcedureId == produce.Id && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0))).Select(x => x.P_ProcedureStatusId).ToListAsync();
        var procedureStatusIdFroms = new List<int?>();
        var procedureStatusIdTos = new List<int?>();

        if (statusTab == ProduceProductStatusTab.Pending || statusTab == ProduceProductStatusTab.All)
        {
            procedureStatusIdFroms = await _context.P_ProcedureStatusSteps.Where(x => statusIds.Contains(x.ProcedureStatusIdFrom ?? 0)).Select(x => x.ProcedureStatusIdFrom).ToListAsync();
        }
        if (statusTab != ProduceProductStatusTab.Pending)
        {
            int? minOrderStaus = await _context.P_ProcedureStatusSteps.Where(x => statusIds.Contains(x.ProcedureStatusIdFrom ?? 0)).Select(x => x.Order).OrderBy(x => x).FirstOrDefaultAsync();
            procedureStatusIdTos = await _context.P_ProcedureStatusSteps.Where(x => x.P_ProcedureId == produce.Id && x.Order >= minOrderStaus).Select(x => x.ProcedureStatusIdTo).ToListAsync();
        }

        return procedureStatusIdFroms.Concat(procedureStatusIdTos);
    }

    public async Task<bool> CheckPermissionAddProcedure(string procedureCode, int userId)
    {
        var procedure = await _context.P_Procedure.FirstOrDefaultAsync(x => x.Code == procedureCode);
        if (procedure is null)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFound);
        }
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            throw new ErrorException(ErrorMessages.UserNotFound);
        }
        var stepFirst = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.P_ProcedureId == procedure.Id && x.IsInit);
        if (stepFirst is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var userRoleIds = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();

        var isAdd = await _context.P_ProcedureStatusRole
                    .Where(x => x.P_ProcedureId == procedure.Id
                                && x.P_ProcedureStatusId == stepFirst.ProcedureStatusIdFrom
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)))
                    .AnyAsync();

        return isAdd;
    }

    public async Task<ProcedureStatusModelResponse> GetStatusInit(string procedureCode)
    {
        var procedure = await _context.P_Procedure.FirstOrDefaultAsync(x => x.Code == procedureCode);
        if (procedure is null)
            return new ProcedureStatusModelResponse();

        var procedureStatusFirst = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.IsInit && x.P_ProcedureId == procedure.Id);
        if (procedureStatusFirst == null)
            return new ProcedureStatusModelResponse();
        var procedureStatus = await _context.P_ProcedureStatus.FirstOrDefaultAsync(x => x.Id == procedureStatusFirst.ProcedureStatusIdFrom);
        if (procedureStatus == null)
            return new ProcedureStatusModelResponse();

        return new ProcedureStatusModelResponse()
        {
            Id = procedureStatus.Id,
            P_StatusName = procedureStatus.Name,
            IsFinish = procedureStatusFirst.IsFinish
        };
    }

    public async Task<ProcedureStatusModelResponse> GetStatusAccept(int? procedureStatusId, int userId, bool isCheckCondition = false)
    {
        var user = await _context.Users.FindAsync(userId);
        var userRoleIds = user.UserRoleIds.Split(",").Select(x => int.Parse(x)).ToList();
        var isExistRole = await _context.P_ProcedureStatusRole.AnyAsync(x => x.P_ProcedureStatusId == procedureStatusId
                                && (x.UserId == userId || userRoleIds.Contains(x.RoleId ?? 0)));

        if (!isExistRole)
        {
            throw new ErrorException("Bạn không có quyền chuyển tiếp quy trình");
        }


        var procedureStatusSteps = await _context.P_ProcedureStatusSteps
                       .Where(x => x.ProcedureStatusIdFrom == procedureStatusId).ToListAsync();
        if (!procedureStatusSteps.Any())
        {
            return new ProcedureStatusModelResponse();
        }

        if (procedureStatusSteps.Count == 1)
        {
            var procedureStatusStepUnique = procedureStatusSteps.First();
            var procedureStatusUnique = await _context.P_ProcedureStatus.FirstOrDefaultAsync(x => x.Id == procedureStatusStepUnique.ProcedureStatusIdTo);

            return new ProcedureStatusModelResponse()
            {
                Id = procedureStatusUnique.Id,
                P_StatusName = procedureStatusUnique.Name,
                IsFinish = procedureStatusStepUnique.IsFinish,
                ProcedureConditionCode = procedureStatusStepUnique.ProcedureConditionCode
            };
        }

        var procedureStatusStepHaveCondition = procedureStatusSteps.FirstOrDefault(x => x.ProcedureConditionId != null && x.ProcedureConditionId > 0);
        if(procedureStatusStepHaveCondition != null && isCheckCondition)
        {
            var procedureStatusHaveCondition = await _context.P_ProcedureStatus.FirstOrDefaultAsync(x => x.Id == procedureStatusStepHaveCondition.ProcedureStatusIdTo);
            if (procedureStatusHaveCondition == null)
                return new ProcedureStatusModelResponse();

            
            return new ProcedureStatusModelResponse()
            {
                Id = procedureStatusHaveCondition.Id,
                P_StatusName = procedureStatusHaveCondition.Name,
                IsFinish = procedureStatusStepHaveCondition.IsFinish,
                ProcedureConditionCode = procedureStatusStepHaveCondition.ProcedureConditionCode
            };
        }

        var procedureStatusStep = procedureStatusSteps.FirstOrDefault(x => x.ProcedureConditionId == null || x.ProcedureConditionId == 0);
        if (procedureStatusStep == null)
            return new ProcedureStatusModelResponse();
        var procedureStatus = await _context.P_ProcedureStatus.FirstOrDefaultAsync(x => x.Id == procedureStatusStep.ProcedureStatusIdTo);
        if (procedureStatus == null)
            return new ProcedureStatusModelResponse();
        
        return new ProcedureStatusModelResponse()
        {
            Id = procedureStatus.Id,
            P_StatusName = procedureStatus.Name,
            IsFinish = procedureStatusStep.IsFinish,
            ProcedureConditionCode = procedureStatusStep.ProcedureConditionCode
        };
    }

    public async Task WriteProcedureLog(string procedureCode, int procedureStatusId, int userId,
        int procedureId,
        string procedureName,
        bool isNotAccept = false)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        if (isNotAccept == true)
        {
            var logNotAccepts = await _context.ProcedureLogs.Where(x => x.ProcedureId == procedureId
                                        && x.ProcedureCode == procedureCode).ToListAsync();
            logNotAccepts = logNotAccepts.ConvertAll(x =>
            {
                x.NotAcceptCount += 1;
                return x;
            });
            _context.ProcedureLogs.UpdateRange(logNotAccepts);
        }
        else
        {
            var log = new ProcedureLog
            {
                UserId = userId,
                ProcedureCode = procedureCode,
                ProcedureStatusId = procedureStatusId,
                ProcedureId = procedureId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                NotificationContent = $"{user.FullName} đã duyệt quy trình {procedureName}"
            };
            await _context.ProcedureLogs.AddAsync(log);
        }
        await _context.SaveChangesAsync();
    }

    public async Task WriteProcedureLogToSendNotification(string procedureCode, int procedureToStatusId, int procedureId, 
        int userId, string procedureName, bool isFinished = false)
    {
        var logDels = await _context.ProcedureLogs.Where(x => x.ProcedureId == procedureId && x.IsSendNotification && x.ProcedureCode == procedureCode).ToListAsync();
        _context.ProcedureLogs.RemoveRange(logDels);
        await _context.SaveChangesAsync();

        if (isFinished)
        {
            return;
        }

        var userName = await _context.Users.Where(x => x.Id == userId).Select(x => x.FullName).FirstOrDefaultAsync();
        var procedureStatus = await _context.P_ProcedureStatusRole.Where(x => x.P_ProcedureStatusId == procedureToStatusId).ToListAsync();
        var userIds = procedureStatus.Select(x => x.UserId).Where(x => x != null).Distinct().ToList();
        var roleIds = procedureStatus.Select(x => x.RoleId).Where(x => x != null).Distinct().ToList();

        
        var log = new ProcedureLog
        {
            ProcedureCode = procedureCode,
            ProcedureStatusId = procedureToStatusId,
            ProcedureId = procedureId,
            UserIds = ";" + string.Join(";", userIds) + ";",
            RoleIds = ";" + string.Join(";", roleIds) + ";",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            NotificationContent = $"{userName} yêu cầu duyệt quy trình {procedureName}",
            IsSendNotification = true
        };
        await _context.ProcedureLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public IQueryable<ProcedureLog> GetProcedureLog(string procedureCode, int procedureId)
    {
           return _context.ProcedureLogs.Where(x => x.ProcedureId == procedureId && !x.IsSendNotification
                                        && x.ProcedureCode == procedureCode && x.NotAcceptCount == 0)
            .OrderBy(x => x.CreatedAt);
           
    }
    public async Task<IEnumerable<int>> GetLogStep(string procedureCode, int procedureId)
    {
        var logs = await GetProcedureLog(procedureCode, procedureId).ToListAsync();
        
        return logs.Select(x => x.UserId);
    }

    public string GetProcedureNumber(string procedureNumber, int length = 7)
    {
        //string dt = DateTime.Now.Year.ToString() + (DateTime.Now.Month > 9 ? DateTime.Now.Month.ToString() : ("0" + DateTime.Now.Month.ToString()));
        string procedureNumberOutDefault = "1";
        while (true)
        {
            if (procedureNumberOutDefault.Length > (length - 1))
            {
                break;
            }
            procedureNumberOutDefault = "0" + procedureNumberOutDefault;
        }

        if (string.IsNullOrEmpty(procedureNumber))
        {
            return procedureNumberOutDefault;
        }
        try
        {
            var procedureNumbers = procedureNumber.Split("-");
            var procedureNumberNumber = int.Parse(procedureNumbers.Last()) + 1;
            var procedureNumberOut = procedureNumberNumber.ToString();
            while (true)
            {
                if (procedureNumberOut.Length > (length - 1))
                {
                    break;
                }
                procedureNumberOut = "0" + procedureNumberOut;
            }
            return procedureNumberOut;
        }
        catch
        {
            return procedureNumberOutDefault;
        }
    }

    public async Task<bool> ExistLog(int procedureId, string procedureCode)
    {
        return await _context.ProcedureLogs.AnyAsync(x => x.ProcedureId == procedureId && x.NotAcceptCount == 0
                                                        && x.ProcedureCode == procedureCode && !x.IsSendNotification);
    }
    public async Task DeleteLog(int procedureId, string procedureCode)
    {
        var logs = await _context.ProcedureLogs.Where(x => x.ProcedureId == procedureId && x.ProcedureCode == procedureCode).ToListAsync();
        _context.ProcedureLogs.RemoveRange(logs);
        await _context.SaveChangesAsync();
    }

    public string GetCode(string codeNumber, string prefix)
    {
        string monthStr = DateTime.Today.Month.ToString();
        while (monthStr.Length < 2)
        {
            monthStr = "0" + monthStr;
        }
        string yearStr = DateTime.Today.Year.ToString().Substring(2, 2);
        
        var procedureNumber = GetProcedureNumber(codeNumber, 4);

        return $"{prefix}{monthStr}-{yearStr}-{procedureNumber}";
    }

    public async Task<IEnumerable<int>> GetUserFinish(int? procedureStatusId)
    {
        var procedureStatusStep = await _context.P_ProcedureStatusSteps.FirstOrDefaultAsync(x => x.ProcedureStatusIdTo == procedureStatusId && x.IsFinish);
        var procedureStatuses = await _context.P_ProcedureStatusRole.Where(x => x.P_ProcedureStatusId == procedureStatusStep.ProcedureStatusIdFrom).ToListAsync();
        var userIds = procedureStatuses.Where(x => x.UserId != null).Select(x => x.UserId ?? 0).ToList();
        var roleIds = procedureStatuses.Where(x => x.RoleId != null).Select(x => x.RoleId);
        foreach(var role in roleIds)
        {
            var userIdRoles = await _context.Users.Where(x => ("," + x.UserRoleIds + ",").Contains("," + role + ",")).Select(x => x.Id).ToListAsync();
            userIds.AddRange(userIdRoles);
        }
        var userIdOuts = userIds.Distinct();
        return userIdOuts;
    }

}