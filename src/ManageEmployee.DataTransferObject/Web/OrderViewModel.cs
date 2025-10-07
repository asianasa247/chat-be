namespace ManageEmployee.DataTransferObject.Web;

public class OrderViewModel
{
    public double TotalPrice { get; set; }
    public double TotalPriceDiscount { get; set; }
    public double TotalPricePaid { get; set; }
    public string? Tell { get; set; }
    public string? FullName { get; set; }
    public bool IsPayment { get; set; }
    public DateTime PaymentAt { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public string? Broker { get; set; }
    public int? ProvinceId { get; set; }
    public int? WardId { get; set; }
    public int? DistrictId { get; set; }
    public string? Identifier { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime Date { get; set; }
    public string? Promotion { get; set; }
    public List<OrderDetailModel>? Goods { get; set; }

}
