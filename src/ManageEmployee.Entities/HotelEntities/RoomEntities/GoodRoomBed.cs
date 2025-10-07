namespace ManageEmployee.Entities.HotelEntities.RoomEntities;

public class GoodRoomBed
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }// goodId
    public int? AdultQuantity { get; set; }
    public string? BedTypeRoomConfigureQuantitys { get; set; }
}
