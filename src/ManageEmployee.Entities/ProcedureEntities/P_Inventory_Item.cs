using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ProcedureEntities;

public class P_Inventory_Item
{
    public int Id { get; set; }
    public int P_InventoryId { get; set; }
    [StringLength(255)]
    public string? GoodsCode { get; set; }
    [StringLength(500)]
    public string? GoodsName { get; set; }
    public double InputQuantity { get; set; } = 0;
    public double OutputQuantity { get; set; } = 0;
    public double Quantity { get; set; } = 0;
    public double QuantityReal { get; set; } = 0;
    [StringLength(500)]
    public string? Note { get; set; }
    [StringLength(500)]
    public string? QrCode { get; set; }
}
