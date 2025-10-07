namespace ManageEmployee.Entities.HotelEntities.RoomEntities;

public class GoodRoomPrice
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }// goodId
    public double PriceShow { get; set; }
    public double Discount { get; set; }
    public double Price { get; set; }
    public bool IsHaveBreakfast { get; set; }
    public DateTime Date { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
