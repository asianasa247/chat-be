using ManageEmployee.Entities.WebEntities;

namespace ManageEmployee.DataTransferObject.Web;

public class CartViewModel : Cart
{
    public string? GoodCode { get; set; }
    public string? GoodName { get; set; }
    public double Price { get; set; }
    public double PriceDiscount { get; set; }
    public double TotalPrice { get; set; }
    public List<string>? Images { get; set; }
}
