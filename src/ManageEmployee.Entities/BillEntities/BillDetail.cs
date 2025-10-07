using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BillEntities;

public class BillDetail
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int GoodsId { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }

    [StringLength(36)]
    public string? DiscountType { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public double Price { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public int? LotNumber { get; set; } // Nullable for cases where no promotion is applied
    [StringLength(36)]
    public string? Status { get; set; }// success, refund
    public string DeliveryCode { get; set; }
    public int? BranchId { get; set; }
}
