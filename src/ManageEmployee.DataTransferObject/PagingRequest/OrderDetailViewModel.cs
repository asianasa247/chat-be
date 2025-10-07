namespace ManageEmployee.DataTransferObject.PagingRequest;
public class OrderDetailViewModel
{
    public int Id { get; set; }
    public int GoodId { get; set; }
    public int? GoodDetailId { get; set; }
    public string? GoodCode { get; set; }
    public string? GoodName { get; set; }
    public double Price { get; set; }
    public double TaxVAT { get; set; }
    public double TotalAmount { get; set; }
    public double DiscountPrice { get; set; }
    public int Quantity { get; set; }
}
