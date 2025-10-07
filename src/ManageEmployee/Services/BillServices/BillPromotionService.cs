using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices;

public class BillPromotionService : IBillPromotionService
{
    private readonly ApplicationDbContext _context;
    public BillPromotionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(List<BillPromotionModel> billPromotionRequests, int tableId, string tableName,
        int? carId = null, string carName = null, int? customerId = null)
    {
        if (billPromotionRequests != null && billPromotionRequests.Any())
        {
            var date = DateTime.Today;
            var goodsPromotions = await _context.GoodsPromotions.Where(x => x.FromAt <= date && x.ToAt >= date).ToListAsync();
            var goodsPromotionIds = goodsPromotions.Select(x => x.Id).ToList();
            var goodsPromotionDetails = await _context.GoodsPromotionDetails.Where(x => goodsPromotionIds.Contains(x.GoodsPromotionId)).ToListAsync();
            var billPromotionChecks = await _context.BillPromotions.Where(x => x.TableName == tableName && x.TableId == tableId
                                            && x.CarId == carId && x.CarName == carName && x.CustomerId == customerId).ToListAsync();
            if (billPromotionChecks.Any())
            {
                _context.BillPromotions.RemoveRange(billPromotionChecks);
                await _context.SaveChangesAsync();
            }


            var billPromotions = new List<BillPromotion>();
            foreach (var promotion in billPromotionRequests)
            {
                var goodsPromotionDetail = goodsPromotionDetails.FirstOrDefault(x => x.Id == promotion.Id);
                if (goodsPromotionDetail is null)
                {
                    throw new ErrorException("Not found promotion");
                }
                var goodPromotion = goodsPromotions.FirstOrDefault(x => x.Id == goodsPromotionDetail.GoodsPromotionId);

                var billPromotion = new BillPromotion();
                billPromotion.PromotionDetailId = promotion.Id;
                billPromotion.TableId = tableId;
                billPromotion.TableName = tableName;
                billPromotion.Note = promotion.Note;
                billPromotion.Amount = promotion.Amount;
                billPromotion.Discount = promotion.Discount;
                billPromotion.Qty = promotion.Qty;
                billPromotion.GoodsPromotionCode = goodPromotion?.Code;
                billPromotion.GoodsPromotionName = goodPromotion?.Name;
                billPromotion.CarId = carId;
                billPromotion.CarName = carName;
                billPromotion.CustomerId = customerId;
                billPromotion.Standard = goodsPromotionDetail.Standard;
                billPromotion.Account = goodsPromotionDetail.Account;
                billPromotion.AccountName = goodsPromotionDetail.AccountName;
                billPromotion.Detail1 = goodsPromotionDetail.Detail1;
                billPromotion.Detail1Name = goodsPromotionDetail.Detail1Name;
                billPromotion.Detail2 = goodsPromotionDetail.Detail2;
                billPromotion.Detail2 = goodsPromotionDetail.Detail2Name;
                billPromotion.Unit = promotion.Unit;
                billPromotions.Add(billPromotion);
            }
            _context.BillPromotions.UpdateRange(billPromotions);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Copy(List<BillPromotion> billPromotionSources, int tableId, string tableName)
    {
        var billPromotions = billPromotionSources.ConvertAll(x =>
        {
            x.Id = 0;
            x.TableId = tableId;
            x.TableName = tableName;
            return x;
        });

        await _context.BillPromotions.AddRangeAsync(billPromotions);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BillPromotionModel>> Get(int tableId, string tableName,
        int? carId = null, string carName = null, int? customerId = null)
    {
        var data = await GetBillPromotion(tableId, tableName, carId, carName, customerId);
        return data
                                .Select(x => new BillPromotionModel
                                {
                                    Id = x.PromotionDetailId,
                                    Code = x.GoodsPromotionCode,
                                    Note = x.Note,
                                    Discount = x.Discount,
                                    Qty = x.Qty,
                                    Name = x.GoodsPromotionName,
                                    Standard = x.Standard,
                                    Amount = x.Amount,
                                    Unit = x.Unit,
                                }).ToList();
    }

    public async Task<List<BillPromotion>> GetBillPromotion(int tableId, string tableName,
        int? carId = null, string carName = null, int? customerId = null)
    {
        return await _context.BillPromotions.Where(x => x.TableId == tableId && x.TableName == tableName
                             && (x.CarId == carId || carId == null)
                             && (string.IsNullOrEmpty(carName) || x.CarName == carName)
                             && (x.CustomerId == customerId || customerId == null)).ToListAsync();
    }

    public async Task<double> GetTotalAmountPromotion(int tableId, string tableName)
    {
        return await _context.BillPromotions.Where(x => x.TableId == tableId && x.TableName == tableName).SumAsync(x => x.Amount);
    }
}
