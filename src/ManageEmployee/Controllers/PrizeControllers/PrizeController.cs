using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.LotteryEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.DataTransferObject.PrizeModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.Controllers.PrizeControllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class PrizeController : ControllerBase
    {
        readonly ApplicationDbContext dbContext;
        readonly UserManager<IdentityUser> userManager;
        readonly SignInManager<IdentityUser> signInManager;
        public PrizeController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            var prize = await dbContext.Prizes.FindAsync(Id);

            if (prize != null)
            {
                foreach (var prizeItem in dbContext.PrizeGoods.ToList())
                {
                    if (prize.PrizeId == prizeItem.PrizeId)
                    {
                        dbContext.Remove(prizeItem);
                    }

                }
                dbContext.Remove(prize);
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }
            }
            return Ok();

        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] PrizeDTO model)
        {

            Prize prize = new Prize();
            prize.Code = model.Code;
            prize.Name = model.Name;
            prize.Description = model.Description;
            prize.Quantity = model.Quantity;
            prize.OrdinalSpin = model.OrdinalSpin;
            prize.Note = model.Note;
            prize.IdSettingsSpin = model.IdSettingsSpin;
            dbContext.Add(prize);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            List<int> NotFoundId = new List<int>();
            foreach (var goodId in model.Goods)
            {
                if (dbContext.Goods.Any(g => g.Id == goodId))
                {
                    PrizeGood good = new PrizeGood();
                    good.IdSettingsSpin = prize.IdSettingsSpin;
                    good.PrizeId = prize.PrizeId;
                    good.GoodId = goodId;
                    dbContext.Add(good);
                }
                else
                {
                    NotFoundId.Add(goodId);
                }
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok(new { NotFoundgoodid = NotFoundId });

        }
        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> Update([FromRoute] int Id, [FromBody] PrizeDTO model)
        {
            var prize = await dbContext.Prizes.FindAsync(Id);
            if (prize == null)
            {
                return NotFound();
            }
            prize.Code = model.Code;
            prize.Name = model.Name;
            prize.Description = model.Description;
            prize.Quantity = model.Quantity;
            prize.OrdinalSpin = model.OrdinalSpin;
            prize.Note = model.Note;
            prize.IdSettingsSpin = model.IdSettingsSpin;
            dbContext.Update(prize);
            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            // Xử lý cập nhật danh sách sản phẩm
            var existingGoods = dbContext.PrizeGoods.Where(gp => gp.PrizeId == prize.PrizeId);
            dbContext.PrizeGoods.RemoveRange(existingGoods); // Xóa các bản ghi cũ

            List<int> NotFoundId = new List<int>();
            foreach (var goodId in model.Goods)
            {
                if (dbContext.Goods.Any(g => g.Id == goodId))
                {
                    PrizeGood goodPromotion = new PrizeGood();
                    goodPromotion.IdSettingsSpin = prize.IdSettingsSpin;
                    goodPromotion.PrizeId = prize.PrizeId;
                    goodPromotion.GoodId = goodId;
                    dbContext.Add(goodPromotion);
                }
                else
                {
                    NotFoundId.Add(goodId);
                }
            }

            try
            {
                await dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }

            return Ok(new { NotFoundgoodid = NotFoundId });
        }

        [HttpPost]
        [Route("AddGood/{Id}")]
        public async Task<IActionResult> AddGood([FromRoute] int Id, [FromBody] int goodId)
        {
            var prize = await dbContext.Prizes.FindAsync(Id);
            if (prize != null)
            {
                if (dbContext.Goods.Any(g => g.Id == goodId))
                {
                    PrizeGood good = new PrizeGood();
                    good.GoodId = goodId;
                    good.PrizeId = prize.PrizeId;
                    good.IdSettingsSpin = prize.IdSettingsSpin;
                    dbContext.Add(good);
                }
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception) { return StatusCode(500); }
                return Ok();
            }
            return NotFound();

        }

        [HttpDelete]
        [Route("DeleteGood/{Id}")]
        public async Task<IActionResult> DeleteGood([FromRoute] int Id, [FromBody] int goodId)
        {
            var prize = await dbContext.Prizes.FindAsync(Id);
            if (prize != null)
            {
                var goodpromotion = dbContext.PrizeGoods.FirstOrDefault(g => g.GoodId == goodId && g.PrizeId == Id);
                if (goodpromotion != null)
                {
                    dbContext.Remove(goodpromotion);
                }
                try
                {
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception) { return StatusCode(500); }
            }
            return Ok();
        }

        [HttpGet("PrizePage")]
        public async Task<IActionResult> GetPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest();

            var query = from p in dbContext.Prizes
                        join s in dbContext.SettingsSpins
                        on p.IdSettingsSpin equals s.SettingId into queryJoin
                        from j in queryJoin.DefaultIfEmpty()
                        select new GetPrizeModel
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
                        };

            var totalItems = await query.CountAsync();  // Tổng số mục
            var prizes = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                Data = prizes
            };

            return Ok(result);
        }

        [HttpGet]
        [Route("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] int Id)
        {
            if (Id <= 0)
            {
                return BadRequest();
            }

            var prize = await (from p in dbContext.Prizes
                               join s in dbContext.SettingsSpins
                               on p.IdSettingsSpin equals s.SettingId
                               into query
                               from j in query.DefaultIfEmpty()
                               where p.PrizeId == Id
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
                                        }
                                             ).ToList()
                               }).FirstOrDefaultAsync();

            if (prize == null)
            {
                return NotFound();
            }

            return Ok(prize);
        }


    }
}
