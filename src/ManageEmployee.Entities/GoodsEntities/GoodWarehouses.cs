using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.GoodsEntities;

public class GoodWarehouses
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? MenuType { get; set; }
    [StringLength(36)]
    public string? PriceList { get; set; }
    [StringLength(36)]
    public string? GoodsType { get; set; }
    public double Quantity { get; set; }
    public double QuantityInput { get; set; }
    [StringLength(1000)]
    public string? Note { get; set; }
    public int Status { get; set; }
    [StringLength(36)]
    public string? Account { get; set; }
    [StringLength(255)]
    public string? AccountName { get; set; }
    [StringLength(36)]
    public string? Warehouse { get; set; }
    [StringLength(255)]
    public string? WarehouseName { get; set; }
    [StringLength(36)]
    public string? Detail1 { get; set; }
    [StringLength(255)]
    public string? DetailName1 { get; set; }
    [StringLength(36)]
    public string? Detail2 { get; set; }
    [StringLength(255)]
    public string? DetailName2 { get; set; }
    [StringLength(255)]
    public string? Image1 { get; set; }
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string? Type { get; set; }
    public int? Order { get; set; }
    public string? OrginalVoucherNumber { get; set; }// ma chung tu
    public bool IsPrinted { get; set; } = false;
    public int? LedgerId { get; set; }
}
