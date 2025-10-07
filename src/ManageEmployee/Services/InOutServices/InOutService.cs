using AutoMapper.Internal;
using Common.Constants;
using Common.Extensions;
using Common.Helpers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.InOut;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Timekeep;
using ManageEmployee.DataTransferObject.TimeKeeperModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq.Expressions;
using GetForReport = ManageEmployee.DataTransferObject.TimeKeeperModels.GetForReport;
using Report = ManageEmployee.DataTransferObject.TimeKeeperModels.Report;

namespace ManageEmployee.Services.InOutServices;

public class InOutService : IInOutService
{
    private readonly ApplicationDbContext _context;
    private readonly IInOutTimeExtend _inOutTimeExtend;
    private readonly IUserService _userService;
    private readonly ITimeKeepingReportHelper _timeKeepingReportHelper;
    public InOutService(ApplicationDbContext context,
                        IInOutTimeExtend inOutTimeExtend,
                        IUserService userService,
                        ITimeKeepingReportHelper timeKeepingReportHelper)
    {
        _context = context;
        _inOutTimeExtend = inOutTimeExtend;
        _userService = userService;
        _timeKeepingReportHelper = timeKeepingReportHelper;
    }

    public async Task<InOutHistory> Create(InOutHistory param)
    {
        param.Checked = true;
        if (param.TimeIn != null)
        {
            param.TargetDate = new DateTime(param.TimeIn.Value.Year, param.TimeIn.Value.Month, param.TimeIn.Value.Day);
        }
        var symbol = new Symbol();
        symbol = await _context.Symbols.FindAsync(param.SymbolId);

        var shift = await _context.Shifts.Where(x => x.SymbolId == param.SymbolId
           && (param.TimeIn.Value.Hour > (x.TimeIn.Hours - 2))
           && (param.TimeIn.Value.Hour < (x.TimeIn.Hours + 2))).FirstOrDefaultAsync();

        if (param.TimeIn == param.TimeOut)
        {
            var timeFromTime = param.TimeIn;
            var timeToTime = param.TimeOut;

            var v1 = symbol.ShiftStartAt.TimeOfDay;
            var v2 = symbol.ShiftEndAt.TimeOfDay;

            param.TimeFrameFrom = timeFromTime.Value.Add(v1);
            param.TimeFrameTo = timeToTime.Value.Add(v2);
         
        }
        else
        {
            var timeFromTime = shift.TimeIn;
            var timeToTime = shift.TimeOut;
            param.TimeFrameFrom = param.TimeIn.Value.Date.Add(timeFromTime);
            param.TimeFrameTo = param.TimeOut.Value.Date.Add(timeToTime);
            

        }





        //var timeFromTime = symbol.ShiftStartAt.AddMinutes(-symbol.CheckInTimeThreshold).TimeOfDay;
        //var timeToTime = symbol.ShiftEndAt.AddMinutes(symbol.CheckOutTimeThreshold).TimeOfDay;

        //var timeInDate = param.TimeIn.Value.Date;
        //var timeOutDate = param.TimeOut.Value.Date;
        //var Test = timeInDate + timeFromTime;




        await _context.InOutHistories.AddAsync(param);
        if (param.TimeIn != null)
        {
            var date = new DateTime(param.TimeIn.Value.Year, param.TimeIn.Value.Month, param.TimeIn.Value.Day);
            var timeType = param.TimeIn.Value.Hour < 12 ? "morning" : "afternoon";
            var meal = await _context.NumberOfMeals.FirstOrDefaultAsync(x => x.Date == date && x.TimeType == timeType);
            if (meal is null)
            {
                meal = new NumberOfMeal
                {
                    TimeType = timeType,
                    Date = date,
                };
            }
            meal.QuantityFromInOut += 1;
            _context.NumberOfMeals.Update(meal);
        }
        await _context.SaveChangesAsync();

        return param;
    }

    public async Task Delete(int id)
    {
        var data = await _context.InOutHistories.FindAsync(id);
        if (data != null)
        {
            _context.InOutHistories.Remove(data);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<InOutHistory>> GetAll()
    {
        return await _context.InOutHistories.Where(x => x.TimeIn.Value.Date == DateTime.Now.Date).GroupBy(x => x.UserId).Select(ab => ab.FirstOrDefault()).ToListAsync();
    }

    public bool HasPreviousCheckInWithAnotherSymbolId(int userId, DateTime? timeIn, int symbolId)
    {
        return _context.InOutHistories
            .Where(x => x.UserId == userId && x.TimeIn.Value == timeIn.Value && x.SymbolId != symbolId)
            .Any();
    }

    public async Task<PagingResult<InOutHistoryViewModel>> GetPaging(InOutHistoryFilterDateParams param, int userId, string listRole)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page < 0)
            param.Page = 1;
        var fromDt = new DateTime(param.FromDate.Year, param.FromDate.Month, param.FromDate.Day);
        var toDt = new DateTime(param.ToDate.Year, param.ToDate.Month, param.ToDate.Day);
        toDt = toDt.AddDays(1);

        var result = from p in _context.Users

                     join Department in _context.Departments on p.DepartmentId equals Department.Id into temp_dpt
                     from dpt in temp_dpt.DefaultIfEmpty()

                     join tmk in _context.InOutHistories on new { A = p.Id } equals new { A = tmk.UserId } into temp_tk
                     from tk in temp_tk.DefaultIfEmpty()

                     join Target in _context.Targets on p.TargetId equals Target.Id into temp_tgt
                     from tg in temp_tgt.DefaultIfEmpty()

                     join Symbol in _context.Symbols on tk.SymbolId equals Symbol.Id into temp_symbol
                     from s in temp_symbol.DefaultIfEmpty()
                     where !p.IsDelete
                           && tk.TargetDate.Date >= fromDt && tk.TargetDate.Date < toDt
                           && (tg != null && param.TargetId > 0 ? p.TargetId == param.TargetId : p.Id != 0)
                           && (string.IsNullOrEmpty(param.SearchText) ||
                               p.Username.Trim().Contains(param.SearchText) ||
                               p.FullName.Trim().Contains(param.SearchText))
                     select new InOutHistoryViewModel
                     {
                         Id = tk.Id,
                         TimeIn = tk.TimeIn,
                         TimeOut = tk.TimeOut,
                         TargetId = tg.Id,
                         TargetCode = tg != null ? tg.Code : string.Empty,
                         TargetName = tg != null ? tg.Name : string.Empty,
                         UserId = p.Id,
                         UserFullName = p != null ? p.FullName : string.Empty,
                         SymbolId = s.Id,
                         SymbolCode = s != null ? s.Code : string.Empty,
                         SymbolName = s != null ? s.Name : string.Empty,
                         Checked = tk.Checked,
                         CheckInMethod = tk.CheckInMethod,
                         IsOverTime = tk.IsOverTime,
                         BranchId = p.BranchId,
                         DepartmentId = p.DepartmentId,
                         TimeFrameFrom = tk.TimeFrameFrom,
                         TimeFrameTo = tk.TimeFrameTo
                     };

        // check permision
        List<string> roles = JsonConvert.DeserializeObject<List<string>>(listRole);
        if (!roles.Contains(UserRoleConst.SuperAdmin))
        {
            if (roles.Contains(UserRoleConst.AdminBranch) || roles.Contains(UserRoleConst.KeToanTruong))
            {
                var branchId = await _context.Users.Where(x => x.Id == userId).Select(x => x.BranchId).FirstOrDefaultAsync();
                result = result.Where(x => x.BranchId == branchId);
            }
            else if (roles.Contains(UserRoleConst.TruongPhong))
            {
                var departmentId = await _context.Users.Where(x => x.Id == userId).Select(x => x.DepartmentId).FirstOrDefaultAsync();
                result = result.Where(x => x.DepartmentId == departmentId);
            }
            else if (roles.Contains(UserRoleConst.NguoiChamCong))
            {
                var targetId = await _context.Users.Where(x => x.Id == userId).Select(x => x.TargetId).FirstOrDefaultAsync();
                result = result.Where(x => x.TargetId == targetId);
            }
            else
            {
                result = result.Where(x => x.UserId == userId);
            }
        }
        _inOutTimeExtend.InsertDataFromRemote(userId, roles);
        var test = await result.ToListAsync();

        var data = await result.OrderBy(x => x.Checked).Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
        foreach (var item in data)
        {
            if (item.TimeIn != null && item.TimeOut != null)
            {
                if (item.TimeOut == item.TimeIn)
                {
                    item.SymbolCode = item.SymbolCode;
                    item.TotalTime = 0;
                    
                }
                else
                {
                    var acceptedTimeIn = CommonHelper.GetMaxValue(item.TimeIn.Value, item.TimeFrameFrom);
                    var acceptedTimeOut = CommonHelper.GetMinValue(item.TimeOut.Value, item.TimeFrameTo);
                    item.TotalTime = Math.Round((acceptedTimeOut - acceptedTimeIn).TotalHours, 2);
                }
               
            }
        }
        return new PagingResult<InOutHistoryViewModel>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await result.CountAsync(),
            Data = data
        };
    }

    public async Task Update(InOutHistory param)
    {
        var inOutHistory = await _context.InOutHistories.FindAsync(param.Id);

        if (inOutHistory == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        inOutHistory.UserId = param.UserId;
        inOutHistory.TargetId = param.TargetId;
        // Can't change checkin time and method while checkin by machine
        //if (param.CheckInMethod != 1) // if not checkin by machine
        //{
        inOutHistory.TimeIn = param.TimeIn;
        inOutHistory.CheckInMethod = param.CheckInMethod;
        //}

        inOutHistory.TimeOut = param.TimeOut;
        inOutHistory.SymbolId = param.SymbolId;
        if (param.TimeIn != null && param.TimeOut != null)
        {
            inOutHistory.Checked = true;
        }

        param.Checked = true;
        if (param.TimeIn != null)
        {
            param.TargetDate = new DateTime(param.TimeIn.Value.Year, param.TimeIn.Value.Month, param.TimeIn.Value.Day);
        }
        var symbol = new Symbol();
        symbol = await _context.Symbols.FindAsync(param.SymbolId);

        var shift = await _context.Shifts.Where(x => x.SymbolId == param.SymbolId
           && (param.TimeIn.Value.Hour > (x.TimeIn.Hours - 2))
           && (param.TimeIn.Value.Hour < (x.TimeIn.Hours + 2))).FirstOrDefaultAsync();

        if (param.TimeIn == param.TimeOut)
        {
            var timeFromTime = symbol.ShiftStartAt;
            var timeToTime = symbol.ShiftEndAt;
            param.TimeFrameFrom = timeFromTime;
            param.TimeFrameTo = timeToTime;
            inOutHistory.TimeFrameFrom = param.TimeFrameFrom;
            inOutHistory.TimeFrameTo = param.TimeFrameTo;
        }
        else
        {
            var timeFromTime = shift.TimeIn;
            var timeToTime = shift.TimeOut;
            param.TimeFrameFrom = param.TimeIn.Value.Date.Add(timeFromTime);
            param.TimeFrameTo = param.TimeOut.Value.Date.Add(timeToTime);
            inOutHistory.TimeFrameFrom = param.TimeFrameFrom;
            inOutHistory.TimeFrameTo = param.TimeFrameTo;

        }




        _context.InOutHistories.Update(inOutHistory);
        await _context.SaveChangesAsync();
    }

    public async Task<PagingResult<InOutHistoryViewModel>> GetAllByUserId(string keyword, int? departmentId, int targetId, DateTime fromDate, DateTime toDate, int pageIndex, int pageSize, int userId)
    {
        if (pageSize <= 0)
            pageSize = 20;

        if (pageIndex < 0)
            pageIndex = 1;

        var result = from p in _context.Users

                     join Department in _context.Departments on p.DepartmentId equals Department.Id into temp_dpt
                     from dpt in temp_dpt.DefaultIfEmpty()

                     join tmk in _context.InOutHistories on new { A = p.Id } equals new { A = tmk.UserId } into temp_tk
                     from tk in temp_tk.DefaultIfEmpty()

                     join Target in _context.Targets on p.TargetId equals Target.Id into temp_tgt
                     from tg in temp_tgt.DefaultIfEmpty()

                     join Symbol in _context.Symbols on tk.SymbolId equals Symbol.Id into temp_symbol
                     from s in temp_symbol.DefaultIfEmpty()

                     where !p.IsDelete
                        && tk.TimeIn.Value.Date >= fromDate.AddHours(7).Date && tk.TimeIn.Value.Date <= toDate.AddHours(7).Date
                        && (tg != null && targetId > 0 ? p.TargetId == targetId : p.Id != 0)
                        && (keyword != null && keyword.Length > 0 ?
                        p.Username.Trim().Contains(keyword) || p.Username.Trim().StartsWith(keyword) || p.Username.Trim().EndsWith(keyword) ||
                        p.FullName.Trim().Contains(keyword) || p.FullName.Trim().StartsWith(keyword) || p.FullName.Trim().EndsWith(keyword)
                         : p.Id != 0)
                        && p.Id == userId
                     select new InOutHistoryViewModel
                     {
                         Id = tk.Id,
                         TimeIn = tk.TimeIn,
                         TimeOut = tk.TimeOut,
                         //TargetId = tg.Id,
                         TargetCode = tg != null ? tg.Code : string.Empty,
                         TargetName = tg != null ? tg.Name : string.Empty,
                         UserId = p.Id,
                         UserFullName = p != null ? p.FullName : string.Empty,
                         SymbolId = s.Id,
                         SymbolCode = s != null ? s.Code : string.Empty,
                         SymbolName = s != null ? s.Name : string.Empty,
                         Checked = tk.Checked,
                         CheckInMethod = tk.CheckInMethod,
                         IsOverTime = tk.IsOverTime
                     };

        return new PagingResult<InOutHistoryViewModel>()
        {
            CurrentPage = pageIndex,
            PageSize = pageSize,
            TotalItems = await result.CountAsync(),
            Data = await result.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
        };
    }

    public async Task UpdateCheckedAsync(int id)
    {
        var inOutHistory = await _context.InOutHistories.FindAsync(id);

        if (inOutHistory == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        inOutHistory.Checked = false;
        _context.InOutHistories.Update(inOutHistory);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Lấy danh sách nhân viên chấm công
    /// </summary>
    /// <param name="date"></param>
    /// <param name="targetId"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    public async Task<PagingResult<ManualCheckInViewModel>> GetManualCheckInUsers(ManualCheckInUserRequest request, int userId, string roles)
    {
        var date = request.DateTimeKeep.HasValue ? request.DateTimeKeep.Value : DateTime.Now;
        var currentDate = date.Date;

        int targetId = 0;
        var user = await _userService.GetByIdAsync(userId);
        // Nếu là user thường
        if (user != null && user.Timekeeper != 2)
        {
            targetId = user?.TargetId ?? 0;
        }
        var userQuery = _context.Users
           .Where(x => !x.IsDelete && !x.Quit);

        if (!roles.Contains(UserRoleConst.SuperAdmin))
        {
            if (roles.Contains(UserRoleConst.AdminBranch))
            {
                userQuery = userQuery.Where(x => x.BranchId == user.BranchId);
            }
            else if (roles.Contains(UserRoleConst.NguoiChamCong))
            {
                userQuery = userQuery.Where(x => x.TargetId == user.TargetId);
            }
            else
            {
                userQuery = userQuery.Where(x => x.Id == user.Id);
            }
        }


        if (!string.IsNullOrEmpty(request.SearchText))
        {
            userQuery = userQuery
            .Where(x => x.FullName != null &&
                       x.FullName.Contains(request.SearchText, StringComparison.InvariantCultureIgnoreCase));
        }

        var users = await userQuery.Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : request.Page))
            .Select(x => new
            {
                UserId = x.Id,
                x.Username,
                x.FullName,
                x.TargetId,
                DefaultSymbolId = x.SymbolId
            }).ToListAsync();
        var userIds = users.Select(x => x.UserId);

        var inoutHistories = await _context.InOutHistories
            .Where(t => t.TimeIn.Value.Date == currentDate && userIds.Contains(t.UserId))
            .Select(x => new
            {
                x.UserId,
                x.TimeIn,
                x.SymbolId
            }).ToListAsync();

        var targets = await _context.Targets.Select(x => new
        {
            TargetId = x.Id,
            TargetName = x.Name
        }).ToListAsync();

        var symbols = await _context.Symbols.Select(x => new
        {
            SymbolId = x.Id,
            SymbolCode = x.Code
        }).ToListAsync();

        var result = users.GroupJoin(inoutHistories,
            x => x.UserId,
            y => y.UserId,
            (x, y) => new
            {
                x.UserId,
                x.Username,
                x.FullName,
                x.TargetId,
                x.DefaultSymbolId,
                CheckInCount = y.Count(),
                CheckinHistories = y.Select(c => new CheckInHistory()
                {
                    TimeIn = c.TimeIn,
                    SymbolId = c.SymbolId,
                    SymbolCode = symbols.Where(v => v.SymbolId == c.SymbolId)
                    .SingleOrDefault()?.SymbolCode ?? string.Empty
                }).ToList()
            }).GroupJoin(targets,
        x => x.TargetId,
        y => y.TargetId,
        (x, y) => new
        {
            x.UserId,
            x.Username,
            x.FullName,
            x.TargetId,
            x.DefaultSymbolId,
            x.CheckInCount,
            y.FirstOrDefault()?.TargetName,
            x.CheckinHistories
        }).GroupJoin(symbols,
        x => x.DefaultSymbolId,
        y => y.SymbolId,
        (x, y) => new ManualCheckInViewModel
        {
            UserId = x.UserId,
            Username = x.Username,
            FullName = x.FullName,
            TargetId = x.TargetId ?? 0,
            DefaultSymbolId = x.DefaultSymbolId ?? 0,
            CheckInCount = x.CheckInCount,
            TargetName = x.TargetName,
            DefaultSymbolCode = y.FirstOrDefault()?.SymbolCode,
            CheckInMethod = 1,
            CheckinHistories = x.CheckinHistories
        }).ToList();

        return new PagingResult<ManualCheckInViewModel>
        {
            Data = result,
            PageSize = request.PageSize,
            CurrentPage = request.Page,
            TotalItems = await userQuery.CountAsync()
        };
    }

    ///// <summary>
    ///// Báo cáo
    ///// </summary>
    ///// <param name="currentPage"></param>
    ///// <param name="pageSize"></param>
    ///// <param name="fromDate"></param>
    ///// <param name="toDate"></param>
    ///// <param name="departmentId"></param>
    ///// <param name="targetId"></param>
    ///// <param name="keyword"></param>
    ///// <param name="currentUser"></param>
    ///// <returns></returns>
    public async Task<IEnumerable<Report>> GetReport(TimeKeepViewModel param, int userId, string roles)
    {
        var f = DateTime.Today;
        if (param.FromDate != null)
        {
            f = param.FromDate.Value.Date;
        }
        var t = DateTime.Today;
        if (param.ToDate != null)
        {
            t = param.ToDate.Value.Date.AddDays(1);
        }
        var listHistory = await (from history in _context.InOutHistories
                           .Where(x => x.TimeIn.Value >= f && x.TimeIn.Value < t)
                                     //join tk in _context.TimeKeep on history.TimekeepId equals tk.Id

                                 join tks in _context.Symbols on history.SymbolId equals tks.Id into temp_tks
                                 from timekeepsymbol in temp_tks.DefaultIfEmpty()

                                 select new GetForReport()
                                 {
                                     Id = history.Id,
                                     //TimekeepId = tk.Id,
                                     TimeKeepSymbolId = history.SymbolId,
                                     TimeKeepSymbolName = timekeepsymbol != null ? timekeepsymbol.Name : "",
                                     TimeKeepSymbolCode = timekeepsymbol != null ? timekeepsymbol.Code : "",
                                     CheckInMethod = history.CheckInMethod,
                                     TimeIn = history.TimeIn,
                                     TimeOut = history.TimeOut,
                                     UserId = history.UserId,
                                     IsOverTime = history.IsOverTime,
                                     DateTimeKeep = history.TimeIn.Value.Date,
                                     TargetId = history.TargetId
                                 }).ToListAsync();

        var overTimes = await _context.ProcedureRequestOvertimes.Where(x =>
            x.FromAt.Date >= f.Date && x.ToAt <= t.Date
        ).Select(x => new
        {
            TotalHours = (x.ToAt - x.FromAt).Hours * x.Coefficient,
            UserIdsStr = $",{x.UserIdStr},",
            Date = x.FromAt.Date,
        }).ToListAsync();

        var listRole = JsonConvert.DeserializeObject<List<string>>(roles);
        var listUser = _userService.QueryUserForPermission(userId, listRole);

        if (userId == 0)
        {
            listUser = _context.Users.Where(x => !x.Quit && !x.IsDelete);
        }

        var users = await (
                from p in listUser

                join dp in _context.Departments on p.DepartmentId equals dp.Id into dp_temp
                from department in dp_temp.DefaultIfEmpty()

                where !p.IsDelete
                && (param.DepartmentId != null ? p.DepartmentId == param.DepartmentId : p.Id != 0)
                && (param.TargetId != null ? p.TargetId == param.TargetId : p.Id != 0)
                && (string.IsNullOrEmpty(param.SearchText) || p.Username.Trim().Contains(param.SearchText) || p.FullName.Trim().Contains(param.SearchText))
                select new Report
                {
                    UserId = p.Id,
                    FullName = p.FullName,
                    DepartmentName = department != null ? department.Name : "",
                    //SymbolId = p.TypeOfWork
                }).ToListAsync();

        var targets = await _context.Targets
            .ToDictionaryAsync(x => x.Id);
        // Auto assin symbol
        var timeSymbols = await _context.Symbols
            .ToDictionaryAsync(x => x.Code);

        listHistory.ForEach(item =>
        {
            // Automatic Machine
            if (string.IsNullOrEmpty(item.TimeKeepSymbolCode))
            {
                if (item.TimeIn.HasValue
                    && item.TimeOut.HasValue)
                {
                    var hours = Math.Round((item.TimeOut.Value - item.TimeIn.Value).TotalHours, 1);
                    item.TimeKeepSymbolCode = string.Format("{0:F1} H", hours);
                    item.TimeKeepSymbolName = string.Format("{0:F1} Giờ", hours);
                    item.TimeKeepSymbolTimeTotal = hours;
                }
            }
            else // Manual check
            {
                if (timeSymbols.ContainsKey(item.TimeKeepSymbolCode))
                    item.TimeKeepSymbolTimeTotal = timeSymbols[item.TimeKeepSymbolCode]?.TimeTotal ?? 0;
            }

            if (targets.ContainsKey(item.TargetId))
            {
                item.TargetCode = targets[item.TargetId].Code;
                item.TargetName = targets[item.TargetId].Name;
            }
        });

        if (listHistory.Any())
        {
            foreach (var user in users)
            {
                var userHistory = listHistory.Where(x => x.UserId == user.UserId).ToList();

                if (userHistory.Any())
                {
                    if (userHistory.Count() > 1 && userHistory.Sum(x => x.TimeKeepSymbolTimeTotal) > 10)
                    {
                        int i = 0;
                        userHistory.ForEach(item =>
                        {
                            if (i > 0)
                            {
                                item.TimeKeepSymbolCode = "";
                                item.TimeKeepSymbolTimeTotal = 0;
                            }
                            i++;
                        });
                    }
                    user.Histories = userHistory;
                }

                user.OvertimesHistories = overTimes
                    .Where(x => x.UserIdsStr.Contains($",{user.UserId},"))
                    .GroupBy(x => x.Date.Date)
                    .Select(x => new OverTimeHistoryModel
                    {
                        Date = x.Key,
                        TotalHours = x.Sum(p => p.TotalHours)
                    }).ToList();
            }
        }

        // Only selected user has checkout in current month

        users = users.Where(x => x.Histories != null && x.Histories.Any())
            .ToList();

        if (param.CheckCurrentUser)
        {
            users = users.Where(x => x.UserId == userId).ToList();
        }
        return users.OrderBy(x => x.DepartmentName);
    }

    public async Task<TimeKeepingReportReponse> TimeKeepingReportV2(TimeKeepViewModel param, int userId, string roles)
    {
        return await _timeKeepingReportHelper.TimeKeepingReportV2(param, userId, roles);
    }

    ///// <summary>
    ///// Xuất file báo cáo
    ///// </summary>
    ///// <param name="fromDate"></param>
    ///// <param name="toDate"></param>
    ///// <param name="departmentId"></param>
    ///// <param name="targetId"></param>
    ///// <param name="keyword"></param>
    ///// <returns></returns>
    public IEnumerable<Report> ExportReport(DateTime fromDate, DateTime toDate, int departmentId = 0, int targetId = 0, string keyword = "", int currentUser = 0)
    {
        var f = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
        var t = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

        var listHistory = (from history in _context.InOutHistories.Where(x => (x.TimeIn.Value >= f || x.TimeIn.Value >= f.AddHours(7)) && (x.TimeIn.Value <= t || x.TimeIn.Value.AddHours(7) <= t.AddHours(7)))

                               // join tk in _context.TimeKeep on history.TimekeepId equals tk.Id

                           join tks in _context.Symbols on history.SymbolId equals tks.Id into temp_tks
                           from timekeepsymbol in temp_tks.DefaultIfEmpty()

                           select new GetForReport()
                           {
                               Id = history.Id,
                               //TimekeepId = tk.Id,
                               TimeKeepSymbolId = history.SymbolId,
                               TimeKeepSymbolName = timekeepsymbol != null ? timekeepsymbol.Name : "",
                               TimeKeepSymbolCode = timekeepsymbol != null ? timekeepsymbol.Code : "",
                               TimeKeepSymbolTimeTotal = timekeepsymbol != null ? timekeepsymbol.TimeTotal : 0,
                               CheckInMethod = history.CheckInMethod,
                               TimeIn = history.TimeIn,
                               TimeOut = history.TimeOut,
                               //DateTimeKeep = history.DateTimeKeep,
                               UserId = history.UserId,
                               IsOverTime = history.IsOverTime
                           }).ToList();

        var users = (
                from p in _context.Users

                join dp in _context.Departments on p.DepartmentId equals dp.Id into dp_temp
                from department in dp_temp.DefaultIfEmpty()

                where !p.IsDelete
                && (departmentId != 0 ? p.DepartmentId == departmentId : p.Id != 0)
                && (targetId != 0 ? p.TargetId == targetId : p.Id != 0)
                  && (keyword != null && keyword.Length > 0 ?
                     p.Username.Trim().Contains(keyword) || p.Username.Trim().StartsWith(keyword) || p.Username.Trim().EndsWith(keyword) ||
                    p.FullName.Trim().Contains(keyword) || p.FullName.Trim().StartsWith(keyword) || p.FullName.Trim().EndsWith(keyword)
                   : p.Id != 0)
                select new Report
                {
                    UserId = p.Id,
                    FullName = p.FullName,
                    DepartmentName = department != null ? department.Name : "",
                }).ToList();
        // Auto assin symbol
        var timeSymbols = _context.Symbols.ToList();

        listHistory.ForEach(item =>
        {
            if (string.IsNullOrEmpty(item.TimeKeepSymbolCode)
                && item.TimeIn.HasValue
                && item.TimeOut.HasValue)
            {
                var hours = Math.Round((item.TimeOut.Value - item.TimeIn.Value).TotalHours, 1);
                item.TimeKeepSymbolCode = string.Format("{0:F1} H", hours);
                item.TimeKeepSymbolName = string.Format("{0:F1} Giờ", hours);
                item.TimeKeepSymbolTimeTotal = hours;
            }
        });

        if (listHistory.Any())
        {
            foreach (var user in users)
            {
                var userHistory = listHistory.Where(x => x.UserId == user.UserId).ToList();
                if (userHistory.Any())
                {
                    user.Histories = userHistory;
                }
            }
        }

        // Only selected user has checkout in current month
        users = users.Where(x => x.Histories != null && x.Histories.Any())
                .ToList();
        if (currentUser != 0)
        {
            users = users.Where(x => x.UserId == currentUser)
            .ToList();
        }

        return users;
    }
}