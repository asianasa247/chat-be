using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class VehicleRepairRequest : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; }
    public string FileStr { get; set; }
}
