using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_SalaryAdvance : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime? Date { get; set; }
    public bool IsForUser { get; set; }

}
