using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.TimeKeeperModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.InOutServices;
public class InOutReportService: IInOutReportService
{
    private readonly ApplicationDbContext _context;
    private readonly IInOutService _inOutService;

    public InOutReportService(ApplicationDbContext context, IInOutService inOutService)
    {
        _context = context;
        _inOutService = inOutService;
    }

    public async Task SetData(int month, int yearFilter)
    {
        var shiftuserDels = await _context.InOutReports.Where(x => x.Month == month && x.Year == yearFilter).ToListAsync();
        if (shiftuserDels.Any())
        {
            _context.InOutReports.RemoveRange(shiftuserDels);
        }

        var fromAt = new DateTime(yearFilter, month, 1);

        TimeKeepViewModel param = new TimeKeepViewModel
        {
            FromDate = fromAt,
            ToDate = fromAt.AddMonths(1).AddDays(-1),
        };
        var datas = await _inOutService.GetReport(param, userId: 0, roles: string.Empty);

        if (datas is null || !datas.Any())
        {
            return;
        }

        var inOutReports = new List<InOutReport>();
        foreach (var data in datas)
        {
            var inOutReport = new InOutReport
            {
                UserId = data.UserId,
                Month = month,
                Year = yearFilter,
            };

            for (var fromDt = fromAt; fromDt <= param.ToDate; fromDt = fromDt.AddDays(1))
            {
                var inoutHistories = string.Join(",", data.Histories.Where(x => x.DateTimeKeep == fromDt).Select(x => x.TimeKeepSymbolCode).ToList());
                switch (fromDt.Day)
                {
                    case 1:
                        inOutReport.Day1 = inoutHistories;
                        break;
                    case 2:
                        inOutReport.Day2 = inoutHistories;
                        break;
                    case 3:
                        inOutReport.Day3 = inoutHistories;
                        break;
                    case 4:
                        inOutReport.Day4 = inoutHistories;
                        break;
                    case 5:
                        inOutReport.Day5 = inoutHistories;
                        break;
                    case 6:
                        inOutReport.Day6 = inoutHistories;
                        break;
                    case 7:
                        inOutReport.Day7 = inoutHistories;
                        break;
                    case 8:
                        inOutReport.Day8 = inoutHistories;
                        break;
                    case 9:
                        inOutReport.Day9 = inoutHistories;
                        break;
                    case 10:
                        inOutReport.Day10 = inoutHistories;
                        break;
                    case 11:
                        inOutReport.Day11 = inoutHistories;
                        break;
                    case 12:
                        inOutReport.Day12 = inoutHistories;
                        break;
                    case 13:
                        inOutReport.Day13 = inoutHistories;
                        break;
                    case 14:
                        inOutReport.Day14 = inoutHistories;
                        break;
                    case 15:
                        inOutReport.Day15 = inoutHistories;
                        break;
                    case 16:
                        inOutReport.Day16 = inoutHistories;
                        break;
                    case 17:
                        inOutReport.Day17 = inoutHistories;
                        break;
                    case 18:
                        inOutReport.Day18 = inoutHistories;
                        break;
                    case 19:
                        inOutReport.Day19 = inoutHistories;
                        break;
                    case 20:
                        inOutReport.Day20 = inoutHistories;
                        break;
                    case 21:
                        inOutReport.Day21 = inoutHistories;
                        break;
                    case 22:
                        inOutReport.Day22 = inoutHistories;
                        break;
                    case 23:
                        inOutReport.Day23 = inoutHistories;
                        break;
                    case 24:
                        inOutReport.Day24 = inoutHistories;
                        break;
                    case 25:
                        inOutReport.Day25 = inoutHistories;
                        break;
                    case 26:
                        inOutReport.Day26 = inoutHistories;
                        break;
                    case 27:
                        inOutReport.Day27 = inoutHistories;
                        break;
                    case 28:
                        inOutReport.Day28 = inoutHistories;
                        break;
                    case 29:
                        inOutReport.Day29 = inoutHistories;
                        break;
                    case 30:
                        inOutReport.Day30 = inoutHistories;
                        break;
                    case 31:
                        inOutReport.Day31 = inoutHistories;
                        break;
                }
                
            }

            inOutReports.Add(inOutReport);
        }
        await _context.InOutReports.AddRangeAsync(inOutReports);

        // remove InoutHistory
        var inoutHistoryDels = await _context.InOutHistories.Where(x => x.TimeIn < fromAt.AddMonths(-3)).ToListAsync();
        _context.InOutHistories.RemoveRange(inoutHistoryDels);
        await _context.SaveChangesAsync();
    }

    public async Task<PagingResult<InOutReport>> GetPaging(PagingRequestModel param, int month, int yearFilter)
    {
        var query = _context.InOutReports.Where(x => x.Month == month && x.Year == yearFilter);

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var totalItem = await query.CountAsync();
        return new PagingResult<InOutReport>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }
}
