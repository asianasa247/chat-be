using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class ExpenditurePlan : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
    public bool IsDone { get; set; }
    public bool IsPart { get; set; }
    public int UserId { get; set; }
}
