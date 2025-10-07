namespace ManageEmployee.Models.Hotels;

public class GoodRoomBedModel
{
    public int RoomTypeId { get; set; }// goodId
    public int? AdultQuantity { get; set; }
    public List<BedTypeRoomConfigureQuantities>? BedTypeRoomConfigureQuantities { get; set; }
}
