using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.PrizeModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.LotteryEntities;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Controllers.Web;
[ApiController]
[Route("api/[controller]")]
public class WebPrizesController : ControllerBase
{
    readonly ApplicationDbContext dbContext;
    private readonly IWebSettingsSpinService _webSettingsSpinService;
    private readonly IWebPrizeService _webPrizeService;
    public WebPrizesController(ApplicationDbContext dbContext,
        IWebSettingsSpinService webSettingsSpinService,
        IWebPrizeService webPrizeService)
    {
        this.dbContext = dbContext;
        _webSettingsSpinService = webSettingsSpinService;
        _webPrizeService = webPrizeService;
    }


    [HttpGet]
    [Route("getproductforprin")]
    public async Task<IActionResult> GetProductForPrin()
    {

        var prize = await (from p in dbContext.Prizes
                           join s in dbContext.SettingsSpins
                           on p.IdSettingsSpin equals s.SettingId
                           into query
                           from j in query.DefaultIfEmpty()
                           select new GetPrizeModel()
                           {
                               PrizeId = p.PrizeId,
                               Code = p.Code,
                               Name = p.Name,
                               IdSettingsSpin = p.IdSettingsSpin,
                               NameSettingSpin = j == null ? "no name" : j.Name,
                               Description = p.Description,
                               Quantity = p.Quantity,
                               OrdinalSpin = p.OrdinalSpin,
                               Note = p.Note,
                               Goods = (
                                    from good in dbContext.PrizeGoods.Where(x => x.PrizeId == p.PrizeId)
                                    join p in dbContext.Goods on good.GoodId equals p.Id
                                    select new PrizeGoodModel()
                                    {
                                        Id = good.Id,
                                        Code = !string.IsNullOrEmpty(p.Detail2) ? p.Detail2 : (p.Detail1 ?? p.Account),
                                        Name = !string.IsNullOrEmpty(p.Detail2) ? p.DetailName2 : (p.DetailName1 ?? p.AccountName),
                                        Image = p.Image1 ?? p.Image2,
                                    }
                                         ).ToList()
                           }).FirstOrDefaultAsync();

        if (prize == null)
        {
            return NotFound();
        }

        return Ok(prize);
    }

    [HttpGet]
    [Route("getcustomerforprize")]
    public async Task<IActionResult> getCustomerForPrize()
    {
        var customers = await dbContext.Customers.ToListAsync();
        return Ok(customers);
    }

    [HttpGet("spin")]
    public async Task<IActionResult> GetSpins()
    {
        var response = await _webSettingsSpinService.GetAsync();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Data = response
        });
    }
    [HttpGet("prize")]
    public async Task<IActionResult> GetPrizes()
    {
        var response = await _webPrizeService.Get();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Data = response
        });
    }

    [HttpGet("prize-customer")]
    public async Task<IActionResult> GetSpinCustomerPrizeAsync(int? spinId)
    {
        var response = await _webSettingsSpinService.GetSpinCustomerPrizeAsync(spinId);

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Data = response
        });
    }

    [HttpGet("spins")]
    [ProducesResponseType(typeof(Response<List<SettingsSpin>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetListSpinAsync()
    {
        var response = await _webSettingsSpinService.GetListSpinAsync();

        return Ok(new CommonWebResponse
        {
            State = true,
            Code = 200,
            Data = response
        });
    }
}

