namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryTypeProduceProductPagingModel
{
    public int Id { get; set; }
    public int ProduceProductId { get; set; }
    public string? ProduceProductCode { get; set; }
    public int SalaryTypeId { get; set; }
    public double Quantity { get; set; }
    public string? Note { get; set; }
    public string? Users { get; set; }
    public DateTime CreatedAt { get; set; }
}