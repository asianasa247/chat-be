using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProduceProductEntities;

public class ProduceProduct : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public int UserId { get; set; }
    public int ManufactureOrderId { get; set; }
    public string? ManufactureOrderCode { get; set; }
    public int? ResponsibleUserId { get; set; }
    public bool IsImportProduct { get; set; }
    public bool IsExportProduct { get; set; }
}
