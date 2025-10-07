using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BillEntities;

public class BillDetailRefund
{
    public int Id { get; set; }
    public int? BillDetailId { get; set; }
    public int? BillId { get; set; }
    public int? BillPromotionId { get; set; }
    public int? GoodsId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [StringLength(255)]
    public string? Note { get; set; }
    public int? BranchId { get; set; }
}
