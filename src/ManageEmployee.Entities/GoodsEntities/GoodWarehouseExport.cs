using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.GoodsEntities;

public class GoodWarehouseExport : BaseEntity
{
    public int Id { get; set; }
    public int GoodId { get; set; }
    public double Quantity { get; set; }
    public int GoodWarehouseId { get; set; }
    public int BillId { get; set; }

    [StringLength(36)]
    public string? GoodsCode { get; set; }

    [StringLength(255)]
    public string? GoodsName { get; set; }

    [StringLength(36)]
    public string? Warehouse { get; set; }

    [StringLength(255)]
    public string? WarehouseName { get; set; }

    [StringLength(255)]
    public string? QrCode { get; set; }

    public DateTime? DateExpiration { get; set; }
}