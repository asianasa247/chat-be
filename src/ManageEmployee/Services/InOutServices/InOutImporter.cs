using ManageEmployee.Helpers;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.InOuts;
using Common.Extensions;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.Services.InOutServices;
public class InOutImporter: IInOutImporter
{
    private readonly ApplicationDbContext _context;
    private List<Symbol> _symbols;
    private List<Shift> _shifts;
    private List<InOutHistory> _outHistories;
    private List<ShiftUserDetail> _shiftUserDetails;
    private List<User> _users;

    public InOutImporter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task ImportAsync(IFormFile file)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        {
            ExcelWorksheet sheet = package.Workbook.Worksheets.First();
            var i = 4;
            
            var (fromDate, toDate) = GetExcelDateRange(sheet, i);

            await InitDataAsync(fromDate, toDate);

            while (sheet.Cells[i, 1].Value != null)
            {
                var date = DateTime.Parse(sheet.Cells[i, 4].Value.ToString());
                var fullName = sheet.Cells[i, 2].Value.ToString();
                var cellTimekeeperIdVal = sheet.Cells[i, 1].Value.ToString().DefaultIfNullOrEmpty("0").Replace("'", "");
                var timekeeperId = int.Parse(cellTimekeeperIdVal);
                var user = _users.FirstOrDefault(x => x.FullName.RemoveAccents() == fullName?.ToLower() && x.TimekeeperId == timekeeperId);

                if (user is null)
                {
                    i++;
                    continue;
                }

                var shiftUserDetail = _shiftUserDetails.FirstOrDefault(x => x.UserId == user.Id);
                if (shiftUserDetail == null)
                {
                    i++;
                    continue;
                }

                var symbolId = int.Parse(shiftUserDetail.GetType().GetProperty($"Day{date.Day}").GetValue(shiftUserDetail, null).ToString());
                var symbol = _symbols.FirstOrDefault(x => x.Id == symbolId);
                
                if (symbol == null)
                {
                    i++;
                    continue;
                }

                var isOvernightShift = symbol.ShiftStartAt.TimeOfDay > symbol.ShiftEndAt.TimeOfDay;
                
                var symbolShifts = _shifts.Where(x => x.SymbolId == symbolId).ToList();
                var checkInThresholdMinutes = symbol.CheckInTimeThreshold;
                var checkOutThresholdMinutes = symbol.CheckOutTimeThreshold;
                var timeOfDay = date.TimeOfDay;
                var shiftIn = symbolShifts.FirstOrDefault(x =>
                    x.TimeIn.Add(new TimeSpan(0, -checkInThresholdMinutes, 0)) <= timeOfDay &&
                    x.TimeIn.Add(new TimeSpan(0, checkInThresholdMinutes, 0)) >= timeOfDay
                );

                var shiftOut = symbolShifts.FirstOrDefault(x =>
                    x.TimeOut.Add(new TimeSpan(0, -checkOutThresholdMinutes, 0)) <= timeOfDay &&
                    x.TimeOut.Add(new TimeSpan(0, checkOutThresholdMinutes, 0)) >= timeOfDay
                );

                if (shiftIn is null && shiftOut is null)
                {
                    i++;
                    continue;
                }

                InOutHistory history = null;

                var shiftTimeIn = (shiftIn ?? shiftOut).TimeIn;
                var isShiftTimeInYesterday = isOvernightShift & shiftTimeIn >= symbol.ShiftStartAt.TimeOfDay;

                // Validate checkin to avoid duplicate
                if (shiftIn != null)
                {
                    var isExisted = _outHistories.Any(x =>
                        x.UserId == user.Id &&
                        x.TimeIn?.Date == date.Date &&
                        x.TimeIn != null &&

                            x.TimeIn?.TimeOfDay >= shiftIn.TimeIn.Add(new TimeSpan(0, -checkInThresholdMinutes, 0)) &&
                            x.TimeIn?.TimeOfDay <= shiftIn.TimeIn.Add(new TimeSpan(0, checkInThresholdMinutes, 0))
                        );

                    if (isExisted)
                    {
                        i++;
                        continue;
                    }
                }

                // Validate check-out is in timeframe of shift
                if (shiftOut != null)
                {
                    // Check existed to avoid duplicate
                    var isExisted = _outHistories.Any(x =>
                        x.UserId == user.Id &&
                        x.TimeOut != null &&
                        x.TimeOut?.Date == date.Date &&

                            x.TimeOut?.TimeOfDay >= shiftOut.TimeOut.Add(new TimeSpan(0, -checkOutThresholdMinutes, 0)) &&
                            x.TimeOut?.TimeOfDay <= shiftOut.TimeOut.Add(new TimeSpan(0, checkOutThresholdMinutes, 0))
                        );

                    if (isExisted)
                    {
                        i++;
                        continue;
                    }
                    history = _outHistories.FirstOrDefault(x =>
                        x.UserId == user.Id &&
                        x.TimeIn != null &&
                        x.TimeOut == null &&
                        (
                            x.TimeIn?.Date == date.Date ||
                            isShiftTimeInYesterday && x.TimeIn?.Date == date.AddDays(-1).Date
                        ) &&

                            x.TimeIn?.TimeOfDay >=
                            shiftOut.TimeIn.Add(new TimeSpan(0, -checkInThresholdMinutes, 0)) &&
                            x.TimeIn?.TimeOfDay <=
                            shiftOut.TimeIn.Add(new TimeSpan(0, checkInThresholdMinutes, 0))

                    );
                }

                // new one if not exist
                if (history == null)
                {
                    history = InitInOutHistory(date, timekeeperId, user, symbol, shiftIn, shiftOut, isShiftTimeInYesterday);
                    _outHistories.Add(history);
                }

                if (shiftIn != null && history.TimeIn is null)
                {
                    history.TimeIn = date;
                }

                if (shiftOut != null && history.TimeOut is null)
                {
                    history.TimeOut = date;
                }

                i++;
            }

            // Update meal
            _context.InOutHistories.UpdateRange(_outHistories);
            await _context.SaveChangesAsync();
        }
    }

    private InOutHistory InitInOutHistory(
        DateTime date, 
        int timekeeperId, 
        User user, 
        Symbol symbol,
        Shift shiftIn, 
        Shift shiftOut, 
        bool isShiftTimeInYesterday)
    {
        var currentShift = shiftIn ?? shiftOut;
        var history = new InOutHistory()
        {
            TargetId = timekeeperId,
            UserId = user.Id,
            SymbolId = symbol.Id,
            Checked = true,
            TimeFrameFrom = new DateTime(
                year: date.Year,
                month: date.Month,
                day: date.Day,
                hour: currentShift.TimeIn.Hours,
                minute: currentShift.TimeIn.Minutes,
                second: currentShift.TimeIn.Seconds
            ),
            TimeFrameTo = new DateTime(
                year: date.Year,
                month: date.Month,
                day: date.Day,
                hour: currentShift.TimeOut.Hours,
                minute: currentShift.TimeOut.Minutes,
                second: currentShift.TimeOut.Seconds
            ),
        };

        bool isCheckInAction = shiftIn != null;

        if (isCheckInAction)
        {
            history.TargetDate = date;
            if (isShiftTimeInYesterday)
            {
                history.TimeFrameTo = history.TimeFrameTo.AddDays(1);
            }
        }
        else // Check-out action
        {
            history.TargetDate = date.Date.AddDays(isShiftTimeInYesterday ? -1 : 0);
            if (isShiftTimeInYesterday)
            {
                history.TimeFrameFrom = history.TimeFrameFrom.AddDays(-1);
            }
        }

        return history;
    }

    private (DateTime fromDate, DateTime toDate) GetExcelDateRange(ExcelWorksheet sheet, int startRowIndex)
    {
        List<DateTime> dates = new List<DateTime>();
        while (sheet.Cells[startRowIndex, 4].Value != null)
        {
            var date = Convert.ToDateTime(sheet.Cells[startRowIndex, 4].Value);
            dates.Add(date);
            startRowIndex++;
        }
        return (dates.Min(), dates.Max());
    }

    private async Task InitDataAsync(DateTime fromDate, DateTime toDate)
    {
        var shiftUser = await _context.ShiftUsers.FirstOrDefaultAsync(x => (x.Month == fromDate.Month && x.Year == fromDate.Year) || (x.Month == toDate.Month && x.Year == toDate.Year));
        if (shiftUser is null)
        {
            throw new ErrorException("Bạn chưa thiết lập ca");
        }

        fromDate = fromDate.AddDays(-2);
        toDate = toDate.AddDays(2);
        _symbols = await _context.Symbols.ToListAsync();
        _shifts = await _context.Shifts.ToListAsync();

       _outHistories = await _context.InOutHistories.Where(x =>
             fromDate.Date <= x.TargetDate.Date &&
             x.TargetDate.Date <= toDate.Date

        ).ToListAsync();

        _shiftUserDetails = await _context.ShiftUserDetails.Where(x => x.ShiftUserId == shiftUser.Id).ToListAsync();
        _users = await _context.Users.ToListAsync();

    }
}
