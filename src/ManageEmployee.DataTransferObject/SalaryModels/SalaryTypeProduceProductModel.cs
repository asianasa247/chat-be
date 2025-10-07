namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryTypeProduceProductModel
{
    public int Id { get; set; }
    public int ProduceProductId { get; set; }
    public string? ProduceProductCode { get; set; }
    public int SalaryTypeId { get; set; }
    public double Quantity { get; set; }
    public List<SalaryTypeProduceProductDetailModel>? Items { get; set; }
    public string? Note { get; set; }
}
