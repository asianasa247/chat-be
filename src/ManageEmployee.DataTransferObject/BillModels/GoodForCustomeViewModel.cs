namespace ManageEmployee.DataTransferObject.BillModels;

public class GoodForCustomeViewModel
{
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? UserCode { get; set; }
    public string? UserName { get; set; }
    public double Amount { get; set; }
    public double Quantity { get; set; }
    public double AmountBack { get; set; }
    public double AmountProfit { get; set; }
    public double QuantityBack { get; set; }
    public string? StockUnit { get; set; }
    public string? Note { get; set; }
    public string? Detail1 { get; set; }
    public string? GoodCode { get; set; }
    public string? Account { get; set; }
    public string? Detail1Name { get; set; }
    public string? GoodName { get; set; }
    public string? AccountName { get; set; }
    public List<GoodForCustomeViewModel>? Items { get; set; }
}
