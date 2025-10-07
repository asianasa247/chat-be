namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryTypeProduceProductDetailModel
{
    public int Id { get; set; }
    public int TargetId { get; set; }
    public int SalaryTypeProduceProductId { get; set; }
    public double Quantity { get; set; }
    public int UserId { get; set; }
    public double Percent { get; set; }
    public string? Note { get; set; }
}
