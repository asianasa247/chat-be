namespace ManageEmployee.Entities.CustomerEntities;

public class CustomerQuote_Detail
{
    public long Id { get; set; }
    public long IdCustomerQuote { get; set; }
    public int GoodsId { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public DateTime CreateDate { get; set; }
    public string? StockUnit { get; set; }
    public double Quantity { get; set; }
    public string? Note { get; set; }
}
