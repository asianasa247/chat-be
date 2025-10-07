using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SalaryEntities;

public class SalaryTypeProduceProduct : BaseEntityCommon
{
    public int Id { get; set; }
    public int ProduceProductId { get; set; }
    public string? ProduceProductCode { get; set; }
    public int SalaryTypeId { get; set; }
    public double Quantity { get; set; }
    public string? Note { get; set; }
}
