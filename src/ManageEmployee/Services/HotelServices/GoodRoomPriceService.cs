using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.HotelEntities.RoomEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.ViewModels.HotelModels;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.HotelServices;

public class GoodRoomPriceService : IGoodRoomPriceService
{
    private readonly ApplicationDbContext _context;

    public GoodRoomPriceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Update(GoodRoomPriceModel form)
    {
        var data = new List<GoodRoomPrice>();
        for (DateTime date = form.FromAt; date <= form.ToAt; date = date.AddDays(1))
        {
            var price = await _context.GoodRoomPrices.FirstOrDefaultAsync(x => x.RoomTypeId == form.RoomTypeId && x.Date == date);
            if (price == null)
            {
                price = new GoodRoomPrice();
            }
            price.RoomTypeId = form.RoomTypeId;
            price.Price = form.Price;
            price.PriceShow = form.PriceShow;
            price.Discount = form.Discount;
            price.UpdatedAt = DateTime.Now;
            price.Date = date;
            price.IsHaveBreakfast = form.IsHaveBreakfast;
            data.Add(price);
        }
        _context.GoodRoomPrices.UpdateRange(data);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GoodRoomPriceGetModel>> Get(GoodRoomPriceRequestModel form)
    {
        if (form.RoomTypeIds is null)
            form.RoomTypeIds = new List<int>();

        var goodPrices = await _context.GoodRoomPrices.Where(x => x.Date >= form.FromAt && x.Date <= form.ToAt).ToListAsync();
        var goodRoomTypes = await _context.GoodRoomTypes.Where(x => !form.RoomTypeIds.Any() || form.RoomTypeIds.Contains(x.GoodId)).ToListAsync();
        var goods = await _context.Goods.Where(x => !x.IsDeleted && x.Status == 1 && (!form.RoomTypeIds.Any() || form.RoomTypeIds.Contains(x.Id))).ToListAsync();
        var bills = await _context.Bills.Where(x => x.Status.Contains(TranTypeConst.Paid) && x.Date >= form.FromAt && x.Date <= form.ToAt).ToListAsync();
        var billIds = bills.Select(x => x.Id);
        var billDetails = await _context.BillDetails.Where(x => billIds.Contains(x.BillId)).ToListAsync();
        var orders = await _context.Order.Where(x => x.Status != OrderStatus.CANCELED && x.FromAt >= form.FromAt && x.ToAt <= form.ToAt).ToListAsync();
        var orderIds = orders.Select(x => x.Id);
        var orderDetails = await _context.OrderDetail.Where(x => orderIds.Contains(x.OrderId)).ToListAsync();

        var dataOut = new List<GoodRoomPriceGetModel>();
        foreach (var goodRoomType in goodRoomTypes)
        {
            var good = goods.FirstOrDefault(x => x.Id == goodRoomType.GoodId);
            if (good is null)
                continue;

            var price = new GoodRoomPriceGetModel
            {
                RoomTypeId = goodRoomType.Id,
                RoomTypeName = good?.WebGoodNameVietNam ?? GoodNameGetter.GetNameFromGood(good),
                Prices = new List<PriceForDate>()
            };
            for (var date = form.FromAt; date <= form.ToAt; date = date.AddDays(1))
            {
                var billIdForDates = bills.Where(x => x.Date == date).Select(x => x.Id).ToList();
                var goodPrice = goodPrices.Find(x => x.Date == date && x.RoomTypeId == goodRoomType.GoodId);
                //check order
                var orderId = orders.Where(x => x.FromAt <= date && x.ToAt >= date).Select(x => x.Id);
                price.Prices.Add(new PriceForDate
                {
                    Date = date,
                    RoomTypeId = goodRoomType.Id,
                    IsHaveBreakfast = goodPrice?.IsHaveBreakfast ?? false,
                    Price = goodPrice?.Price ??good.Price,
                    Quantity = goodRoomType.Quantity - billDetails.Where(x => billIdForDates.Contains(x.BillId) && x.GoodsId == goodRoomType.GoodId).Sum(x => x.Quantity),
                    QuantityOrder = orderDetails.Where(x => x.GoodId == good.Id && orderId.Contains(x.OrderId)).Sum(x => x.Quantity)
                });
            }
            dataOut.Add(price);
        }
        return dataOut;
    }
}