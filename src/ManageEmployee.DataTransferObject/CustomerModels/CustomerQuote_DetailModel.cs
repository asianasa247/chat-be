namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerQuote_DetailModel
{
    public long Id { get; set; }
    public long IdCustomerQuote { get; set; }
    public int GoodsId { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public DateTime CreateDate { get; set; }
    public string StockUnit { get; set; }
    public double Quantity { get; set; }
    public string Note { get; set; }
    public string GoodsName { get; set; }
    public string Image { get; set; }
}
