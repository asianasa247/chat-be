using Google.Apis.Gmail.v1.Data;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodPromotion;
using ManageEmployee.DataTransferObject.HistorySpinModels;
using ManageEmployee.Entities.LotteryEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static ManageEmployee.DataTransferObject.GoodPromotion.GoodPromotionModel;

namespace ManageEmployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoodPromotionController : ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        public GoodPromotionController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpPut("{Id:int:required}")]
        public async Task<IActionResult> Update([FromRoute][Required] int? Id, GoodPromotionDTO model)
        {
            var goodpromotion = await dbContext.PrizeGoods.FindAsync(Id);
            if (goodpromotion == null)
            {
                return NotFound();
            }
            goodpromotion.IdSettingsSpin = model.IdSettingsSpin;
            goodpromotion.PrizeId = model.PrizeId;
            goodpromotion.GoodId = model.GoodId;
            dbContext.Update(goodpromotion);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
            return Ok();
        }


        [HttpDelete("{Id:int:required}")]
        public async Task<IActionResult> Delete([FromRoute][Required] int? Id)
        {
            var gp = await dbContext.PrizeGoods.FindAsync(Id);
            if (gp != null)
            {
                dbContext.Remove(gp);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return StatusCode(500);
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Add(GoodPromotionDTO model)
        {
            PrizeGood goodPromotion = new PrizeGood();
            goodPromotion.IdSettingsSpin = model.IdSettingsSpin;
            goodPromotion.PrizeId = model.PrizeId;
            goodPromotion.GoodId = model.GoodId;

            await dbContext.AddAsync(goodPromotion);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoodPromotionModel>>> Get()
        {
            var goods = await dbContext.PrizeGoods.ToListAsync();
            List<GoodPromotionModel> result = new List<GoodPromotionModel>();
            foreach (var goodspromotion in goods)
            {
                GoodPromotionModel model = new GoodPromotionModel();
                model.GoodPromotionId = goodspromotion.Id;
                SettingsSpin settingSpin = await dbContext.SettingsSpins.FindAsync(goodspromotion.IdSettingsSpin);
                model.SettingsSpinInfo = new SettingSpinInfomation()
                {
                    Id = goodspromotion.IdSettingsSpin,
                    Code = settingSpin == null ? "No code" : settingSpin.Code,
                    Name = settingSpin == null ? "No Name" : settingSpin.Name
                };

                var prize = await dbContext.Prizes.FindAsync(goodspromotion.PrizeId);
                model.Prize = new PrizeInfo()
                {
                    Id = goodspromotion.PrizeId,
                    Code = prize == null ? "No Code" : prize.Code,
                    Name = prize == null ? "No Name" : prize.Name
                };

                var good = await dbContext.Goods.FindAsync(goodspromotion.GoodId);
                model.Good = new GoodInfo()
                {
                    Id = goodspromotion.GoodId,
                    Account = good == null ? "No account" : good.Account,
                    AccountName = good == null ? "No accountname" : good.AccountName,
                    Warehouse = good == null ? "No warehouse" : good.Warehouse,
                    WarehouseName = good == null ? "No warehousename" : good.WarehouseName,
                    Detail1 = good == null ? "No detail1" : good.Detail1,
                    DetailName1 = good == null ? "No detailName1" : good.DetailName1,
                    Detail2 = good == null ? "No detail2" : good.Detail2,
                    DetailName2 = good == null ? "No detailName2" : good.DetailName2,
                };
                result.Add(model); 
            }
            return result;
        }
    }
}
