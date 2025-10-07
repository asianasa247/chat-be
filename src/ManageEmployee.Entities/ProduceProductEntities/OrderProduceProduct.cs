using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProduceProductEntities;

public class OrderProduceProduct : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public bool IsSpecialOrder { get; set; }
    public double TotalAmount { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsDelivered { get; set; }
    public bool IsDone { get; set; }
}
