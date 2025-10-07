using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProduceProductEntities;

public class ManufactureOrder : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    [MaxLength(500)]
    public string? Note { get; set; }
    public int UserId { get; set; }
    public bool IsProduceProduct { get; set; }
    public bool IsCanceled { get; set; }
    public string? CanceledNote { get; set; }
    public int? ResponsibleUserId { get; set; }
}
