using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PrizeModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Web
{
    public class WebPrizeService: IWebPrizeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebSettingsSpinService _webSettingsSpinService;
        private readonly IMapper _mapper;

        public WebPrizeService(ApplicationDbContext context,
            IWebSettingsSpinService webSettingsSpinService,
            IMapper mapper)
        {
            _context = context;
            _webSettingsSpinService = webSettingsSpinService;
            _mapper = mapper;
        }
        public async Task<List<WebPrizeModel>> Get()
        {
            var spins = await _webSettingsSpinService.GetListCurrentSpinAsync();
            var spinIds = spins.Select(x => x.SettingId);
            var prizes = await _context.Prizes.Where(x => spinIds.Contains(x.IdSettingsSpin)).ToListAsync();
            var listOut = new List<WebPrizeModel>();
            foreach(var prize in prizes)
            {
                var itemOut = _mapper.Map<WebPrizeModel>(prize);
                var goodIds = await _context.PrizeGoods.Where(x => x.PrizeId == prize.PrizeId).Select(x => x.GoodId).ToListAsync();
                var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

                itemOut.Goods = goods
                    .Select(x => new PrizeGoodModel
                    {
                        Code = GoodNameGetter.GetCodeFromGood(x),
                        Name = GoodNameGetter.GetNameFromGood(x),
                        Image = string.IsNullOrEmpty(x.Image2) ? x.Image1 : x.Image2,
                    }).ToList();

                listOut.Add(itemOut);
            }
            return listOut;
        }
    }
}