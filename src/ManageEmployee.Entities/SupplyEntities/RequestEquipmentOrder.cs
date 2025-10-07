using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class RequestEquipmentOrder : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public double Amount { get; set; }
    public int RequestEquipmentId { get; set; }
    public string? RequestEquipmentCode { get; set; }
    public string? FileStr { get; set; }
    public int CustomerId { get; set; }
}
