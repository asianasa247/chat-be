using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.LotteryEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Web
{
    public class WebSettingsSpinService: IWebSettingsSpinService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public WebSettingsSpinService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<WebSpinWithCustomerModel>> GetAsync()
        {
            var time = DateTime.Now;
            var spins = await GetListCurrentSpinAsync();
            var listOut = new List<WebSpinWithCustomerModel>();
            foreach (var spin in spins)
            {
                var itemOut = _mapper.Map< WebSpinWithCustomerModel>(spin);
                itemOut.Customers = await _context.Customers
                    .Where(x => x.CustomerClassficationId == spin.IdCustomerClassification)
                    .Select(x => _mapper.Map<Customer, CustomerModelView>(x))
                    .ToListAsync();
                listOut.Add(itemOut);
            }
            return listOut;
        }
        public async Task<List<SettingsSpin>> GetListCurrentSpinAsync()
        {
            var time = DateTime.Now;
            return await _context.SettingsSpins.Where(x => x.TimeStartSpin <= time && x.TimeEnd >= time).ToListAsync();
        }

        public async Task<List<SpinCustomerPrizeModel>> GetSpinCustomerPrizeAsync(int? spinId)
        {
            var spins = new List<SettingsSpin>();

            if (spinId is null)
            {
                spins = await GetListCurrentSpinAsync();
            }
            else
            {
                spins = await _context.SettingsSpins.Where(x => x.SettingId == spinId).ToListAsync();
            }

            var listOut = new List<SpinCustomerPrizeModel>();
            foreach(var spin in spins)
            {
                var itemOut = new SpinCustomerPrizeModel
                {
                    SettingsSpinId = spin.SettingId,
                    SettingsSpinCode= spin.Code,
                    SettingsSpinName = spin.Name,
                    Details = new List<SpinCustomerPrizeDetailModel>()
                };
                var history = await _context.HistorySpins.FirstOrDefaultAsync(x => x.IdSettingsSpin == spin.SettingId);
                if(history is null)
                {
                    continue;
                }
                var historyDetails = await _context.HistorySpinDetails.Where(x => x.HistorySpinId == history.HistoryId).ToListAsync();
                var prizes = await _context.Prizes.Where(x => x.IdSettingsSpin == spin.SettingId).ToListAsync();
                foreach(var historyDetail in historyDetails)
                {
                    var prize = prizes.Find(x => x.PrizeId == historyDetail.PrizeId);
                    var customer = await _context.Customers.FindAsync(historyDetail.CustomerId);
                    var good = await _context.Goods.FindAsync(historyDetail.GoodId);
                    if (good is null)
                    {
                        throw new ErrorException(ErrorMessages.GoodsCodeAlreadyExist);
                    }
                    itemOut.Details.Add(new SpinCustomerPrizeDetailModel
                    {
                        PrizeId = historyDetail.PrizeId,
                        PrizeCode = prize?.Code,
                        PrizeName = prize?.Name,
                        CustomerId = historyDetail.CustomerId,
                        CustomerCode = customer?.Code,
                        CustomerName = customer?.Name,
                        GoodId = historyDetail.GoodId,
                        GoodName = GoodNameGetter.GetNameFromGood(good)
                    });
                }    
                listOut.Add(itemOut);
            }
            return listOut;
        }

        public async Task<List<SettingsSpin>> GetListSpinAsync()
        {
            return await _context.SettingsSpins.Where(x => x.TimeStart <= DateTime.Now).OrderByDescending(x => x.TimeEnd).Take(50).ToListAsync();
        }
    }
}
