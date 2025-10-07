using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProduceProductEntities;

public class WarehouseProduceProduct : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }
    public bool IsManufactureOrder { get; set; }
}
