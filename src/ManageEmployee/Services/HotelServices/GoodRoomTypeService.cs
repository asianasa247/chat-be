using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.HotelEntities.RoomEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Models.Hotels;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.ViewModels.HotelModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services.HotelServices;

public class GoodRoomTypeService : IGoodRoomTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly IGoodRoomPriceService _goodRoomPriceService;

    public GoodRoomTypeService(ApplicationDbContext context, IGoodRoomPriceService goodRoomPriceService)
    {
        _context = context;
        _goodRoomPriceService = goodRoomPriceService;
    }

    public async Task<GoodRoomTypeModel> GetById(int goodId)
    {
        var good = await _context.Goods.FindAsync(goodId);
        if (good == null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        var roomType = await _context.GoodRoomTypes.FirstOrDefaultAsync(x => x.GoodId == goodId);
        if (roomType == null)
            roomType = new GoodRoomType();

        var goodRoom = new GoodRoomTypeModel
        {
            Id = goodId,
            GoodNameEn = good.WebGoodNameEnglish,
            GoodNameKo = good.WebGoodNameKorea,
            GoodNameVn = good.WebGoodNameVietNam ?? GoodNameGetter.GetNameFromGood(good),
            Quantity = roomType.Quantity,
            RoomTypeRoomConfigureId = roomType.RoomTypeRoomConfigureId,
            LengthRoom = roomType.LengthRoom,
            WidthRoom = roomType.WidthRoom,
            AdultQuantity = roomType.AdultQuantity,
            ChildrenQuantity = roomType.ChildrenQuantity,
            IsExtraBed = roomType.IsExtraBed,
            BedTypeRoomConfigureId = roomType.BedTypeRoomConfigureId,
            Description = good.ContentVietNam
        };
        if (!string.IsNullOrEmpty(roomType.AmenityTypeRoomConfigureyTypeIds) && !string.IsNullOrEmpty(roomType.AmenityTypeRoomConfigureyIds))
        {
            var amenityTypeIds = JsonConvert.DeserializeObject<List<int>>(roomType.AmenityTypeRoomConfigureyTypeIds);
            var amenityIds = JsonConvert.DeserializeObject<List<int>>(roomType.AmenityTypeRoomConfigureyIds);
            var amenityTypes = await _context.RoomConfigureTypes.Where(x => amenityTypeIds.Contains(x.Id)).ToListAsync();

            var amenityTypeModels = new List<RoomConfigureTypeModel>();
            foreach (var amenityType in amenityTypes)
            {
                amenityTypeModels.Add(new RoomConfigureTypeModel
                {
                    Id = amenityType.Id,
                    NameVn = amenityType.NameVn,
                    NameKo = amenityType.NameKo,
                    NameEn = amenityType.NameEn,
                    Code = amenityType.Code,
                    Items = await _context.RoomConfigures.Where(x => x.RoomConfigureTypeId == amenityType.Id && amenityIds.Contains(x.Id)).Select(x => new RoomConfigureModel
                    {
                        Id = x.Id,
                        NameVn = x.NameVn,
                        Code = x.Code,
                    }).ToListAsync()
                });
            }
            goodRoom.RoomConfigureTypes = amenityTypeModels;
        }

        var beds = await _context.GoodRoomBeds.Where(X => X.RoomTypeId == roomType.Id).ToListAsync();
        if (beds.Any())
        {
            goodRoom.RoomBeds = beds.Select(x => new GoodRoomBedModel
            {
                RoomTypeId = x.Id,
                AdultQuantity = x.AdultQuantity,
                BedTypeRoomConfigureQuantities = JsonConvert.DeserializeObject<List<BedTypeRoomConfigureQuantities>>(x.BedTypeRoomConfigureQuantitys)
            }).ToList();
        }

        return goodRoom;
    }

    public async Task<PagingResult<GoodRoomTypePagingModel>> GetPaging(PagingRequestModel param)
    {
        if (param.Page < 1)
            param.Page = 1;
        var query = _context.Goods.Where(x => !x.IsDeleted
                                && x.Status == 1
                                && x.IsService
                                && string.IsNullOrEmpty(param.SearchText) || x.WebGoodNameVietNam.Contains(param.SearchText));
        var data = await query.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize)
            .Select(a => new GoodRoomTypePagingModel
            {
                Id = a.Id,
                GoodNameVn = a.WebGoodNameVietNam ?? GoodNameGetter.GetNameFromGood(a),
                GoodNameKo = a.WebGoodNameKorea,
                GoodNameEn = a.WebGoodNameEnglish
            }).ToListAsync();
        var goodIds = data.Select(x => x.Id).ToList();
        var roomTypes = await _context.GoodRoomTypes.Where(x => goodIds.Contains(x.GoodId)).ToListAsync();
        foreach (var item in data)
        {
            var roomType = roomTypes.FirstOrDefault(x => x.GoodId == item.Id);
            item.Quantity = roomType?.Quantity ?? 0;
        }
        return new PagingResult<GoodRoomTypePagingModel>
        {
            Data = data,
            PageSize = param.PageSize,
            CurrentPage = param.Page,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<IEnumerable<GoodRoomTypeSelectModel>> GetAll()
    {
        var goods = await _context.Goods.Where(x => !x.IsDeleted && x.Status == 1 && x.IsService).ToListAsync();
        return goods.Select(a => new GoodRoomTypeSelectModel
        {
            Id = a.Id,
            Name = a.WebGoodNameVietNam ?? GoodNameGetter.GetNameFromGood(a)
        });
    }

    public async Task Update(GoodRoomTypeModel param)
    {
        var good = await _context.Goods.FindAsync(param.Id);
        if (good == null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        good.WebGoodNameKorea = param.GoodNameKo;
        good.WebGoodNameEnglish = param.GoodNameEn;
        good.WebGoodNameVietNam = param.GoodNameVn;
        good.ContentVietNam = param.Description;
        _context.Goods.Update(good);

        var roomType = await _context.GoodRoomTypes.FirstOrDefaultAsync(x => x.GoodId == param.Id);
        if (roomType is null)
        {
            roomType = new GoodRoomType();
        }
        roomType.GoodId = param.Id;
        roomType.Quantity = param.Quantity;
        roomType.RoomTypeRoomConfigureId = param.RoomTypeRoomConfigureId;
        roomType.LengthRoom = param.LengthRoom;
        roomType.WidthRoom = param.WidthRoom;
        roomType.AdultQuantity = param.AdultQuantity;
        roomType.ChildrenQuantity = param.ChildrenQuantity;
        roomType.IsExtraBed = param.IsExtraBed;
        roomType.BedTypeRoomConfigureId = param.BedTypeRoomConfigureId;
        roomType.AmenityTypeRoomConfigureyTypeIds = JsonConvert.SerializeObject(param.RoomConfigureTypes.Select(x => x.Id).ToList());
        roomType.AmenityTypeRoomConfigureyIds = JsonConvert.SerializeObject(param.RoomConfigureTypes.SelectMany(x => x.Items.Select(x => x.Id)).Distinct().ToList());

        var bedDels = await _context.GoodRoomBeds.Where(X => X.RoomTypeId == roomType.Id).ToListAsync();
        _context.GoodRoomBeds.RemoveRange(bedDels);
        if (param.RoomBeds != null)
        {
            var beds = param.RoomBeds.Select(x => new GoodRoomBed
            {
                AdultQuantity = x.AdultQuantity,
                RoomTypeId = roomType.Id,
                BedTypeRoomConfigureQuantitys = JsonConvert.SerializeObject(x.BedTypeRoomConfigureQuantities)
            });
            await _context.GoodRoomBeds.AddRangeAsync(beds);
        }

        _context.GoodRoomTypes.Update(roomType);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<GoodRoomTypeOrderModel>> GetRoomTypeForOrder(DateTime fromAt, DateTime toAt)
    {
        var goods = await _context.Goods.Where(x => !x.IsDeleted
                                && x.Status == 1
                                && x.IsService).ToListAsync();
        var goodTypes = await _context.GoodRoomTypes.ToListAsync();

        var amenityTypeAlls = await _context.RoomConfigureTypes.ToListAsync();
        var amenities = await _context.RoomConfigures.ToListAsync();

        var roomPriceModel = new GoodRoomPriceRequestModel
        {
            FromAt = fromAt,
            ToAt = toAt,
        };
        var roomAvailables = await _goodRoomPriceService.Get(roomPriceModel);
        // check phong trong
        var goodAvaiables = new List<GoodRoomTypeOrderModel>();
        foreach (var good in goods)
        {
            var goodType = goodTypes.Find(x => x.GoodId == good.Id);
            if (goodType is null)
                continue;

            var roomAvailable = roomAvailables.Where(x => x.RoomTypeId == goodType.Id).SelectMany(x => x.Prices).ToList();
            if (!roomAvailable.Any())
            {
                continue;
            }
            var quantity = roomAvailable.Min(x => x.Quantity - x.QuantityOrder);

            var goodRoom = new GoodRoomTypeOrderModel
            {
                Id = good.Id,
                GoodNameEn = good.WebGoodNameEnglish,
                GoodNameKo = good.WebGoodNameKorea,
                GoodNameVn = good.WebGoodNameVietNam ?? GoodNameGetter.GetNameFromGood(good),
                Quantity = quantity,
                RoomTypeRoomConfigureId = goodType.RoomTypeRoomConfigureId,
                LengthRoom = goodType.LengthRoom,
                WidthRoom = goodType.WidthRoom,
                AdultQuantity = goodType.AdultQuantity,
                ChildrenQuantity = goodType.ChildrenQuantity,
                IsExtraBed = goodType.IsExtraBed,
                BedTypeRoomConfigureId = goodType.BedTypeRoomConfigureId,
                Description = good.ContentVietNam,
                Price = roomAvailable.FirstOrDefault().Price,
            };

            if (!string.IsNullOrEmpty(goodType.AmenityTypeRoomConfigureyTypeIds) && !string.IsNullOrEmpty(goodType.AmenityTypeRoomConfigureyIds))
            {
                var amenityTypeIds = JsonConvert.DeserializeObject<List<int>>(goodType.AmenityTypeRoomConfigureyTypeIds);
                var amenityIds = JsonConvert.DeserializeObject<List<int>>(goodType.AmenityTypeRoomConfigureyIds);
                var amenityTypeModels = new List<RoomConfigureTypeModel>();
                var amenityTypes = amenityTypeAlls.Where(x => amenityTypeIds.Contains(x.Id)).ToList();

                foreach (var amenityType in amenityTypes)
                {
                    amenityTypeModels.Add(new RoomConfigureTypeModel
                    {
                        Id = amenityType.Id,
                        NameVn = amenityType.NameVn,
                        NameKo = amenityType.NameKo,
                        NameEn = amenityType.NameEn,
                        Code = amenityType.Code,
                        Items = amenities.Where(x => x.RoomConfigureTypeId == amenityType.Id && amenityIds.Contains(x.Id)).Select(x => new RoomConfigureModel
                        {
                            Id = x.Id,
                            NameVn = x.NameVn,
                            Code = x.Code,
                        }).ToList()
                    });
                }
                goodRoom.RoomConfigureTypes = amenityTypeModels;
            }
            // add tien ich
            goodAvaiables.Add(goodRoom);
        }
        return goodAvaiables;
    }
}