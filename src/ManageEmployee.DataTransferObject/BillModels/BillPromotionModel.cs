namespace ManageEmployee.DataTransferObject.BillModels;

public class BillPromotionModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public string? Standard { get; set; }
    public double Discount { get; set; }
    public double Amount { get; set; }
    public string? Code { get; set; }
    public double Qty { get; set; }
    public string? Name { get; set; }
    public string? Unit { get; set; }
}