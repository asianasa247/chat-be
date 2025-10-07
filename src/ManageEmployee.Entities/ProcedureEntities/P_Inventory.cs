using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_Inventory : BaseEntity
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? ProcedureNumber { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    public int? P_ProcedureStatusId { get; set; }
    [StringLength(255)]
    public string? P_ProcedureStatusName { get; set; }
    public bool isFinish { get; set; }
    [StringLength(36)]
    public string? Warehouse { get; set; }
    [StringLength(255)]
    public string? WarehouseName { get; set; }
    [StringLength(36)]
    public string? Detail1 { get; set; }
    [StringLength(255)]
    public string? DetailName1 { get; set; }
    [StringLength(36)]
    public string? Detail2 { get; set; }
    [StringLength(255)]
    public string? DetailName2 { get; set; }

}
