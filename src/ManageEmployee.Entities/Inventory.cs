using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class Inventory
{
    public int Id { get; set; }
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
    public double InputQuantity { get; set; } = 0;
    public double OutputQuantity { get; set; } = 0;
    public double CloseQuantity { get; set; } = 0;
    public double CloseQuantityReal { get; set; } = 0;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public string? Note { get; set; }
    public DateTime? DateExpiration { get; set; }
    public bool isCheck { get; set; }
}
