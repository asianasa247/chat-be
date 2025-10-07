using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class RequestExportGood : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int UserId { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
}
