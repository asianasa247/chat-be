using ManageEmployee.Models.Hotels;

namespace ManageEmployee.ViewModels.HotelModels;

public class GoodRoomTypePagingModel
{
    public int Id { get; set; }
    public string? GoodNameVn { get; set; }
    public string? GoodNameKo { get; set; }
    public string? GoodNameEn { get; set; }
    public int Quantity { get; set; }
    public int RoomTypeRoomConfigureId { get; set; }
    public double? LengthRoom { get; set; }
    public double? WidthRoom { get; set; }
    public int? AdultQuantity { get; set; }
    public int? ChildrenQuantity { get; set; }
}

public class GoodRoomTypeSelectModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class GoodRoomTypeModel
{
    public int Id { get; set; }
    public string? GoodNameVn { get; set; }
    public string? GoodNameKo { get; set; }
    public string? GoodNameEn { get; set; }
    public int Quantity { get; set; }
    public int RoomTypeRoomConfigureId { get; set; }
    public double? LengthRoom { get; set; }
    public double? WidthRoom { get; set; }
    public int? AdultQuantity { get; set; }
    public int? ChildrenQuantity { get; set; }
    public bool IsExtraBed { get; set; }
    public int BedTypeRoomConfigureId { get; set; }
    public string? Description { get; set; }
    public List<RoomConfigureTypeModel>? RoomConfigureTypes { get; set; }
    public List<GoodRoomBedModel>? RoomBeds { get; set; }
}

public class GoodRoomTypeOrderModel
{
    public int Id { get; set; }
    public string? GoodNameVn { get; set; }
    public string? GoodNameKo { get; set; }
    public string? GoodNameEn { get; set; }
    public int Quantity { get; set; }
    public int RoomTypeRoomConfigureId { get; set; }
    public double? LengthRoom { get; set; }
    public double? WidthRoom { get; set; }
    public int? AdultQuantity { get; set; }
    public int? ChildrenQuantity { get; set; }
    public bool IsExtraBed { get; set; }
    public int BedTypeRoomConfigureId { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public List<RoomConfigureTypeModel>? RoomConfigureTypes { get; set; }
}