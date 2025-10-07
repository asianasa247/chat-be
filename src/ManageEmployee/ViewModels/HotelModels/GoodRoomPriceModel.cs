namespace ManageEmployee.ViewModels.HotelModels;

public class GoodRoomPriceRequestModel
{
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public List<int>? RoomTypeIds { get; set; }
}
public class GoodRoomPriceModel
{
    public int RoomTypeId { get; set; }
    public double PriceShow { get; set; }
    public double Discount { get; set; }
    public double Price { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public bool IsHaveBreakfast { get; set; }
}

public class GoodRoomPriceGetModel
{
    public int RoomTypeId { get; set; }
    public string? RoomTypeName { get; set; }
    public List<PriceForDate>? Prices { get; set; }
}
public class PriceForDate
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public DateTime Date { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public int QuantityOrder { get; set; }
    public bool IsHaveBreakfast { get; set; }
}
