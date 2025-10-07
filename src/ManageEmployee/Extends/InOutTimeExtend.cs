using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Transactions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.DataTransferObject.InOutRemote;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Extends;

public class InOutTimeExtend : IInOutTimeExtend
{
    private readonly ApplicationDbContext _context;
    private readonly static object _syncLock = new();
    private readonly AppSettings _appSettings;
    private readonly IUserService _userService;

    public InOutTimeExtend(ApplicationDbContext context, IOptions<AppSettings> appSettings, IUserService userService)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _userService = userService;
    }

    public bool InsertDataFromRemote(int userId, List<string> roles)
    {
        lock (_syncLock)
        {
            List<InOutHistory> insertInOutHistory = new ();
            List<InOutHistory> updateInOutHistory = new ();

            var current = _context.InOutHistories
                //.Where(x => x.)
                .Select(x => new
                {
                    Id = x.Id,
                    UId = $"1{x.UserId}{x.TimeIn.Value.ToString("yyyyMMddHHmmss")}{x.CheckInMethod}",
                    Out = x.TimeOut
                })
                .ToList()
                .GroupBy(x => x.UId)
                .Select(y => y.First())
                .ToDictionary(x => x.UId);

            var symbols = _context
                .Symbols
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.TimeIn,
                    x.TimeOut,
                    x.TimeTotal
                })
                .ToDictionary(x => x.Code);
            
            var users = _userService.QueryUserForPermission(userId, roles)
                .Select(x => new
                {
                    x.Id,
                    x.Username,
                    x.TargetId
                }).Where(x => !string.IsNullOrEmpty(x.Username))
                .ToDictionary(x => $"1{x.Username}");

            var histories = getRemoteData();

            foreach (var history in histories.Result.Data)
            {
                if (string.IsNullOrEmpty(history.UserCode)
                    || !history.UserCode.StartsWith('1')) continue;

                var currentUser = users
                    .ContainsKey(history.UserCode) ? users[history.UserCode] : null;

                if (currentUser == null) continue;

                var item = new InOutHistory()
                {
                    //TargetId = currentUser.TargetId,
                    UserId = currentUser.Id,
                    TimeIn = history.In,
                    TimeOut = history.Out,
                    SymbolId = 0,
                    CheckInMethod = 1 // check in by machine
                };

                if (item.TimeOut.HasValue)
                {
                    var timeIn = item.TimeIn.Value;
                    var timeOut = item.TimeOut.Value;

                    var validTimeRange = new TimeSpan(0, 1, 0, 0);

                    foreach (var currentSymbol in symbols)
                    {
                        var symbolTimeIn = currentSymbol.Value.TimeIn;
                        var symbolTimeOut = currentSymbol.Value.TimeOut;

                        // check valid timerange
                        if (timeIn >= symbolTimeIn.Subtract(validTimeRange)
                            && timeIn <= symbolTimeIn.Add(validTimeRange)
                            && timeOut >= symbolTimeOut.Subtract(validTimeRange)
                            && timeOut <= symbolTimeOut.Add(validTimeRange)
                        )
                        {
                            item.SymbolId = currentSymbol.Value.Id;
                        }
                    }
                }

                var key = $"{history.UserCode}{history.In.ToString("yyyyMMddHHmmss")}1";

                if (!current.ContainsKey(key))
                {
                    insertInOutHistory.Add(item);
                }
                else
                {
                    if (!current[key].Out.HasValue && history.Out.HasValue)
                    {
                        item.Id = current[key].Id;
                        updateInOutHistory.Add(item);
                    }
                }
            }

            foreach (var insert in insertInOutHistory)
            {
                _context.Entry(insert).State = EntityState.Added;
                _context.Add(insert);
            }

            foreach (var update in updateInOutHistory)
            {
                _context.Entry(update).State = EntityState.Modified;
                _context.Update(update);
            }

            var success = false;

            using (var scope = new TransactionScope())
            {
                _context.SaveChanges();
                scope.Complete();
                success = true;
            }

            return success;
        }
    }

    private async Task<PagingResult<InOutTime>> getRemoteData()
    {
        try
        {

            HttpClient client = new();
            client.BaseAddress = new Uri(_appSettings.TimeKeepBaseUrl);
            var response = client.GetAsync(_appSettings.TimeKeepBaseApi, HttpCompletionOption.ResponseHeadersRead).Result;

            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStreamAsync();
            var inOutHistories = JsonSerializer.Deserialize<PagingResult<InOutTime>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return inOutHistories;
        }
        catch
        {
            return new PagingResult<InOutTime>();
        }
    }
}