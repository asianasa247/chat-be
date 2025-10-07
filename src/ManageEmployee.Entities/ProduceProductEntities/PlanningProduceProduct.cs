using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProduceProductEntities;

public class PlanningProduceProduct : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? StatusId { get; set; }
    public bool IsPlanningProduct { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsDone { get; set; }
    public bool IsPart { get; set; }
    public string? FileDeliveredStr { get; set; }
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
    public string? CreateFromTable { get; set; }
}
