using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.Web;

public class CartEditViewModel
{
    public int Id { get; set; }
    public int? GoodId { get; set; }
    public int? CustomerId { get; set; }
    public int? Quantity { get; set; }
    public CartStateEnum State { get; set; }
    public string? GoodCode { get; set; }
    public string? GoodName { get; set; }
    public double? Price { get; set; }
    public double? PriceDiscount { get; set; }
    public double? TotalPrice { get; set; }
    public List<string>? Images { get; set; }
}
