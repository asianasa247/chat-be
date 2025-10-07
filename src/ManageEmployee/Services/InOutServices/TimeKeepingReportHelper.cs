using Common.Extensions;
using Common.Helpers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.TimeKeeperModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.InOutServices;

public class TimeKeepingReportHelper : ITimeKeepingReportHelper
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public TimeKeepingReportHelper(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<TimeKeepingReportReponse> TimeKeepingReportV2(TimeKeepViewModel param, int userId, string roles)
    {
        var dateModel = GetDates(param.FromDate, param.ToDate);

        var (totalReport, reports) = await GetReports(userId, roles, param);

        // Get histories by users paging above
        var userIds = reports.Select(x => x.UserId).ToList();

        var inOutHistories = await GetInOutHistories(
            dateModel.FromDate, 
            dateModel.ToDate, 
            userIds
        );

        var overTimeHistories = await GetOverTimeHistories(
            dateModel.FromDate, 
            dateModel.ToDate
        );

        var leaveResult = await GetLeaveReport(
            dateModel.LeaveFromDate, 
            dateModel.LeaveToDate, 
            userIds
        );

        var result = new List<TimeKeepingReportV2Model>();
        foreach (var user in reports)
        {
            var currentLeave = leaveResult
                                .Where(x => x.UserId == user.UserId && x.IsFinish)
                                .ToList();

            var currentInOutHistories = inOutHistories
                                            .Where(x => x.UserId == user.UserId)
                                            .ToList();

            var currentOverTimeHistories = overTimeHistories
                                            .Where(x => x.UserIds.Contains(user.UserId))
                                            .ToList();

            var report = ConvertToTimeKeepingReport(
                currentInOutHistories,
                currentOverTimeHistories,
                currentLeave,
                dateModel.Dates,
                user
            );

            result.Add(report);
        }
        return new TimeKeepingReportReponse()
        {
            TotalItems = totalReport,
            PageSize = param.Page,
            Data = result,
            Date = dateModel
        };
    }

    private async Task<List<LeaveReportModel>> GetLeaveReport(DateTime leaveFromDate, DateTime leaveToDate, List<int> userIds)
    {
        return await (from leave in _context.P_Leave
                      join leave_item in _context.P_Leave_Items
                            on leave.Id equals leave_item.LeaveId
                      join symbol in _context.Symbols
                            on leave_item.SymbolCode equals symbol.Code
                      where userIds.Contains(leave.UserId)
                            && leave.Fromdt >= leaveFromDate
                            && leave.Todt <= leaveToDate
                      select new LeaveReportModel
                      {
                          UserId = leave.UserId,
                          Date = leave_item.Date.AddHours(7),
                          SymbolCode = leave_item.SymbolCode,
                          ToDate = leave.Todt.AddHours(7),
                          FromDate = leave.Fromdt.AddHours(7),
                          TotalHours = (leave.Todt - leave.Fromdt).TotalHours,
                          SymbolHours = symbol.TimeTotal,
                          IsFinish = leave.IsFinished,
                          IsLicensed = leave_item.IsLicensedItem,
                      }).ToListAsync();
    }

    private static TimeKeepingReportDateModel GetDates(DateTime? fromDate, DateTime? toDate)
    {
        var currentFromDate = fromDate ?? DateTime.Today;
        var currentToDate = toDate ?? currentFromDate.AddDays(1);
        var dates = new List<DateTime>();

        for (var date = currentFromDate; date <= currentToDate; date = date.AddDays(1))
        {
            dates.Add(date.Date);
        }

        var leaveFromDate = currentFromDate.Date.AddHours(-7);
        var leaveToDate = currentToDate.Date.AddHours(-7);

        return new TimeKeepingReportDateModel
        {
            FromDate = currentFromDate,
            ToDate = currentToDate,
            LeaveFromDate = leaveFromDate,
            LeaveToDate = leaveToDate,
            Dates = dates
        };
    }

    private async Task<List<OverTimeHistoryModel>> GetOverTimeHistories(DateTime fromDate, DateTime toDate)
    {
        var procedureRequestOvertimes = await _context.ProcedureRequestOvertimes
                    .Where(x => x.IsFinished
                        && x.FromAt.Date >= fromDate.Date
                        && x.ToAt <= toDate.Date)
                    .ToListAsync();
        if (!procedureRequestOvertimes.Any()) 
        {
            return new List<OverTimeHistoryModel>();
        }

        return procedureRequestOvertimes
                     .Select(x => new OverTimeHistoryModel
                     {
                         TotalHours = (x.ToAt - x.FromAt).Hours * x.Coefficient,
                         Date = x.FromAt.Date,
                         UserIds = x.UserIdStr.Deserialize<List<int>>(),
                     })
                    .ToList();
    }

    private async Task<List<InOutSmallPeriodModel>> GetInOutHistories(DateTime fromDate, DateTime toDate, List<int> userIds)
    {
        return await (from history in _context.InOutHistories
                      join symbol in _context.Symbols
                           on history.SymbolId equals symbol.Id into symbolGroup
                      from symbol in symbolGroup.DefaultIfEmpty()
                      where history.TargetDate.Date != DateTime.MinValue &&
                            history.TimeIn >= fromDate.Date &&
                            history.TimeIn <= toDate.Date &&
                            userIds.Contains(history.UserId)
                      select new InOutSmallPeriodModel
                      {
                          UserId = history.UserId,
                          Date = history.TargetDate.Date,
                          TimeIn = history.TimeIn,
                          TimeOut = history.TimeOut,
                          SymbolHours = symbol.TimeTotal,
                          SymbolCode = symbol.Code,
                          TimeFrameFrom = history.TimeFrameFrom,
                          TimeFrameTo = history.TimeFrameTo,
                          WorkType = history.IsOverTime
                      })
                      .ToListAsync();
    }

    private TimeKeepingReportV2Model ConvertToTimeKeepingReport(
        List<InOutSmallPeriodModel> inOutHistories,
        List<OverTimeHistoryModel> overTimeHistories,
        List<LeaveReportModel> currentLeave,
        List<DateTime> dates,
        Report user
        )
    {
        var userOvertimes = ToWorkingTimeModel(overTimeHistories);

        var userInOut = ToWorkingTimeModel(inOutHistories);

        var workingDay = GetWorkingDay(inOutHistories);

        var workMissingDates = userInOut.GroupBy(x => x.Date).Select(x => x.Key).ToList();

        var groupDates = GetGroupDates(userOvertimes, userInOut, workMissingDates);

        var missingDates = GetMissingDates(dates, groupDates);

        var leaveGroupByDate = currentLeave.GroupBy(x => x.DateOnly)
                                           .ToDictionary(x => x.Key, x => x.ToList());

        var symbolHours = GetSymbolHours(userInOut);

        var (totalOverTimeDay, totalOverTimeHours) = CalOverTimes(userOvertimes, symbolHours);
        var (totalWorkingDay, totalWorkingHours) = CalWorkingDay(workingDay);
        var (totalPaidLeave, totalUnPaidLeave) = CalLeaveDay(currentLeave, symbolHours);

        return new TimeKeepingReportV2Model
        {
            UserId = user.UserId,
            DepartmentName = user.DepartmentName,
            FullName = user.FullName,
            Histories = GetHistories(groupDates, missingDates, leaveGroupByDate),
            TotalWorkingDay = totalWorkingDay,
            TotalWorkingHours = totalWorkingHours,
            TotalOverTimeWorkingHours = totalOverTimeHours,
            TotalOverTimeDay = totalOverTimeDay,
            TotalPaidLeave = totalPaidLeave,
            TotalUnPaidLeave = totalUnPaidLeave,
            TotalUnLicensed = currentLeave.Where(x => !x.IsLicensed)
                                          .Select(x => x.DateOnly)
                                          .Distinct()
                                          .Count(),
        };
    }

    private static List<TimeKeepingHistoryByDateModel> GetGroupDates(List<WorkingTimeModel> userOvertimes, List<WorkingTimeModel> userInOut, List<DateTime> workMissingDates)
    {
        return userOvertimes.Concat(userInOut)
                    .GroupBy(x => x.Date)
                    .Select(gr => ToTimeKeepingHistoryByDateModel(gr, workMissingDates)).ToList();
    }

    private static List<TimeKeepingHistoryByDateModel> GetMissingDates(List<DateTime> dates, List<TimeKeepingHistoryByDateModel> groupDates)
    {
        return dates.Where(x => groupDates.All(p => p.Date != x))
                    .Select(date => new TimeKeepingHistoryByDateModel(date))
                    .ToList();
    }

    private (double totalPaidLeave, double totalUnPaidLeave) CalLeaveDay(List<LeaveReportModel> currentLeave, double symbolHours)
    {
        var licensedLeave = currentLeave.Where(x => x.IsLicensed);

        var totalPaidLeave = (licensedLeave
            .Where(x => x.SymbolHours > 0)
            .Sum(x => Math.Min(x.TotalHours, x.SymbolHours)) / symbolHours
        ).Round();

        var totalUnPaidLeave = (licensedLeave
            .Where(x => x.SymbolHours == 0)
            .Sum(x => Math.Min(x.TotalHours, symbolHours)) / symbolHours
        ).Round();

        return (totalPaidLeave, totalUnPaidLeave);
    }

    private (double totalWorkingDay, double totalWorkingHours) CalWorkingDay(List<WorkingDayModel> workingDay)
    {
        var totalWorkingDay = workingDay.Sum(s => s.TotalDays).Round();
        var totalWorkingHours = workingDay.Sum(x => x.TotalHours).Round();

        return (totalWorkingDay, totalWorkingHours);
    }

    private (double totalOverTimeDay, double totalOverTimeHours) CalOverTimes(List<WorkingTimeModel> userOvertimes, double symbolHours)
    {
        var totalHours = userOvertimes.Sum(x => x.TotalHours);

        var totalOverTimeWorkingHours = totalHours.Round();

        var totalOverTimeDay = (totalHours / symbolHours).Round();

        return (totalOverTimeDay, totalOverTimeWorkingHours);
    }

    private static List<TimeKeepingHistoryByDateModel> GetHistories(
        List<TimeKeepingHistoryByDateModel> groupDates,
        IEnumerable<TimeKeepingHistoryByDateModel> missingDates,
        Dictionary<DateOnly, List<LeaveReportModel>> leaveGroupByDate)
    {
        return groupDates.Concat(missingDates)
                        .OrderBy(x => x.Date)
                        .Select(s =>
                        {
                            if (leaveGroupByDate.TryGetValue(s.DateOnly, out var leaveReports))
                            {
                                var symbolCodes = leaveReports.Select(s => s.SymbolCode).ToList();

                                symbolCodes.Insert(0, s.SymbolCode);
                                s.SymbolCode = symbolCodes.JoinString(", ");
                            }

                            s.SymbolCode = s.SymbolCode
                                            .DefaultIfEmpty(
                                                    s.WorkingHours
                                                        .Round()
                                                        .ToString()
                                            );
                            return s;
                        }).ToList();
    }

    private static List<WorkingDayModel> GetWorkingDay(List<InOutSmallPeriodModel> inOutHistories)
    {
        var workTypeGrHistories = inOutHistories
           .GroupBy(x => new { x.WorkType, x.Date })
           .Select(s => new
           {
               Date = s.Key.Date,
               WorkType = s.Key.WorkType,
               SymbolCode = s.First().SymbolCode,
               WorkingTime = s.ToList()
           })
           .ToList();

        return workTypeGrHistories
                    .Select(s =>
                    {
                        var tempTotalHours = s.WorkingTime.Sum(s => s.TotalHours);
                        var symbolHours = s.WorkingTime.FirstOrDefault()?.SymbolHours ?? 1;

                        var totalHours = tempTotalHours > symbolHours
                                        ? symbolHours
                                        : tempTotalHours;

                        return new WorkingDayModel
                        {
                            TotalHours = totalHours,
                            TotalDays = totalHours / symbolHours
                        };
                    }).ToList();
    }

    private static List<WorkingTimeModel> ToWorkingTimeModel(List<InOutSmallPeriodModel> inOutHistories)
    {
        return inOutHistories
                    .Select(x => new WorkingTimeModel
                    {
                        TotalHours = x.TotalHours == 0 ? x.SymbolHours : x.TotalHours,
                        Date = x.Date,
                        IsOvertime = false,
                        IsMissingInOut = x.IsMissingInOut,
                        SymbolCode = x.SymbolCode,
                        SymbolHours = x.SymbolHours,
                        WorkType = x.WorkType
                    }).ToList();
    }

    private static List<WorkingTimeModel> ToWorkingTimeModel(List<OverTimeHistoryModel> overTimeHistories)
    {
        return overTimeHistories
                                .Select(x => new WorkingTimeModel
                                {
                                    TotalHours = x.TotalHours,
                                    Date = x.Date,
                                    IsOvertime = true,
                                    IsMissingInOut = false,
                                    SymbolCode = string.Empty,
                                    SymbolHours = 0d,
                                    WorkType = (int)WorkType.Overtime
                                }).ToList();
    }

    private static TimeKeepingHistoryByDateModel ToTimeKeepingHistoryByDateModel(IGrouping<DateTime, WorkingTimeModel> gr, List<DateTime> workMissingDates)
    {
        var overTime = gr.Where(x => x.IsOvertime).ToList();
        var workingTime = gr.Where(x => !x.IsOvertime).ToList();

        var workingTimeSymbolCode = workingTime.FirstOrDefault()?.SymbolCode ?? string.Empty;
        var workingTimeSymbolHours = workingTime.FirstOrDefault()?.SymbolHours ?? 0;

        var totalWorkingTime = workingTime.Sum(x => x.TotalHours);

        var totalOverTime = Math.Round(overTime.Sum(x => x.TotalHours), 2);

        var model = new TimeKeepingHistoryByDateModel(
            date: gr.Key,
            isMissingInOut: !workMissingDates.Contains(gr.Key) || gr.Any(x => x.IsMissingInOut),
            workingHours: totalWorkingTime,
            overtimeHours: totalOverTime,
            symbolCode: workingTimeSymbolCode,
            symbolHours: workingTimeSymbolHours
        );

        var listCode = new List<string>();

        if (totalWorkingTime >= workingTimeSymbolHours)
        {
            listCode.Add(workingTimeSymbolCode);
        }
        else if(totalWorkingTime > 0)
        {
            listCode.Add(totalWorkingTime.Round().ToString());
        }

        if (totalOverTime > 0)
        {
            listCode.Add(totalOverTime.ToString());
        }

        model.SymbolCode = listCode.JoinString(", ");
        return model;
    }

    private double GetSymbolHours(List<WorkingTimeModel> userInOut)
    {
        var symbolHours = userInOut.FirstOrDefault()?.SymbolHours;
        if (symbolHours != null)
        {
            return symbolHours.Value;
        }
        return _context.Symbols.FirstOrDefault(x => x.Code == Constants.Constants.SymbolWorkingDefault)?.TimeTotal ?? 8;
    }

    private IQueryable<User> GetUserQuery(int userId, string roles, TimeKeepViewModel param)
    {
        Expression<Func<User, bool>> predicate = x => !x.IsDelete &&
                                (string.IsNullOrEmpty(param.SearchText)
                                || x.Username.Trim().Contains(param.SearchText)
                                || x.FullName.Trim().Contains(param.SearchText));

        if (param.DepartmentId != null)
        {
            predicate = predicate.And(x => x.DepartmentId == param.DepartmentId);
        }

        if (param.TargetId != null)
        {
            predicate = predicate.And(x => x.TargetId == param.TargetId);
        }

        if (userId == 0)
        {
            predicate = predicate.And(x => !x.Quit);
            return _context.Users.Where(predicate);
        }

        // Get Users by role
        var userRoles = roles.Deserialize<List<string>>();

        return _userService.QueryUserForPermission(userId, userRoles)
                           .Where(predicate);
    }

    private async Task<(int TotalItems, List<Report> Items)> GetReports(
        int userId,
        string roles,
        TimeKeepViewModel param)
    {
        var userByRoleList = GetUserQuery(userId, roles, param);

        var report = (from p in userByRoleList
                      join dp in _context.Departments
                          on p.DepartmentId equals dp.Id into dpGroup
                      from department in dpGroup.DefaultIfEmpty()
                      select new Report
                      {
                          UserId = p.Id,
                          FullName = p.FullName,
                          DepartmentName = department != null ? department.Name : "",
                      });


        var rowCount = await report.CountAsync();

        var pageIndex = param.PageSize * (param.Page > 0 ? param.Page - 1 : param.Page);

        var reports = await report
                            .Skip(pageIndex)
                            .Take(param.PageSize)
                            .ToListAsync();

        return (rowCount, reports);
    }

    private record class WorkingTimeModel
    {
        public double TotalHours { get; internal set; }
        public DateTime Date { get; internal set; }
        public bool IsOvertime { get; internal set; }
        public bool IsMissingInOut { get; internal set; }
        public string SymbolCode { get; internal set; }
        public double SymbolHours { get; internal set; }
        public int WorkType { get; internal set; }
    }

    private record class WorkingDayModel
    {
        public double TotalHours { get; internal set; }
        public double TotalDays { get; internal set; }
    }
}
