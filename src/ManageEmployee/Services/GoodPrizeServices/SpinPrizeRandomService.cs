using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.LotteryEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Hubs;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodPrizeServices
{
    public class SpinPrizeRandomService: ISpinPrizeRandomService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebSettingsSpinService _webSettingsSpinService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;

        public SpinPrizeRandomService(ApplicationDbContext dbContext,
            IWebSettingsSpinService webSettingsSpinService,
            IHubContext<BroadcastHub, IHubClient> hubContext)
        {
            _context = dbContext;
            _webSettingsSpinService = webSettingsSpinService;
            _hubContext = hubContext;
        }
        public async Task RandomAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var datetimeNow = DateTime.Now;
            var spins = await _webSettingsSpinService.GetAsync();
            foreach (var spin in spins)
            {
                if (spin.TimeStartSpin > datetimeNow)
                {
                    continue;
                }
                var history = await _context.HistorySpins.FirstOrDefaultAsync(x => x.IdSettingsSpin == spin.SettingId);
                if (history != null)
                {
                    continue;
                }
                history = new HistorySpin
                {
                    CreatedAt = DateTime.Now,
                    IdSettingsSpin = spin.SettingId,
                    WinTime = spin.AwarDay,
                    ReceivedDay = spin.AwarDay,
                };
                await _context.HistorySpins.AddAsync(history);
                await _context.SaveChangesAsync();

                var prizes = await _context.Prizes.Where(x => x.IdSettingsSpin == spin.SettingId).OrderBy(x => x.OrdinalSpin).ToListAsync();
                var historyDetailAdds = new List<HistorySpinDetail>();
                var customers = spin.Customers;
                foreach (var prize in prizes)
                {
                    var goods = await _context.PrizeGoods.Where(x => x.PrizeId == prize.PrizeId).Select(x => x.GoodId).ToListAsync();
                    if (!goods.Any())
                    {
                        throw new ErrorException($"Bạn chưa nhập danh sách hàng hóa thưởng {prize.Name}");
                    }
                    for (int i = 0; i < prize.Quantity; i++)
                    {
                        if (!customers.Any())
                        {
                            break;
                        }
                        // random customer
                        Random rnd = new Random();
                        int customerRandomIndex = rnd.Next(customers.Count);

                        int goodRandomIndex = rnd.Next(goods.Count);

                        var historyDetail = new HistorySpinDetail
                        {
                            WinTime = spin.AwarDay,
                            ReceivedDay = spin.AwarDay,
                            HistorySpinId = history.HistoryId,
                            CustomerId = customers[customerRandomIndex].Id,
                            GoodId = goods[goodRandomIndex],
                            PrizeId = prize.PrizeId,
                            SettingsSpinId = spin.SettingId,
                        };
                        historyDetailAdds.Add(historyDetail);
                        customers.RemoveAt(customerRandomIndex);
                    }
                }
                await _context.HistorySpinDetails.AddRangeAsync(historyDetailAdds);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                await _hubContext.Clients.All.BroadcastMessage();
            }
        }
    }
}
