namespace ManageEmployee.Entities.HotelEntities.RoomEntities;

public class GoodRoomType
{
    public int Id { get; set; }
    public int GoodId { get; set; }
    public int Quantity { get; set; }
    public int RoomTypeRoomConfigureId { get; set; }
    public double? LengthRoom { get; set; }
    public double? WidthRoom { get; set; }
    public int? AdultQuantity { get; set; }
    public int? ChildrenQuantity { get; set; }
    public bool IsExtraBed { get; set; }
    public int BedTypeRoomConfigureId { get; set; }
    public string? AmenityTypeRoomConfigureyTypeIds { get; set; }
    public string? AmenityTypeRoomConfigureyIds { get; set; }
}