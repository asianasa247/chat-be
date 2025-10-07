namespace ManageEmployee.Entities.GoodsEntities;

public class GoodDetail
{
    public int ID { get; set; }
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Detail1 { get; set; }
    public string? DetailName1 { get; set; }
    public string? Detail2 { get; set; }
    public string? DetailName2 { get; set; }
    public string? Warehouse { get; set; }
    public double? Quantity { get; set; }
    public double? UnitPrice { get; set; }
    public double? Amount { get; set; }
    public string? AccountParent { get; set; }
    public string? AccountNameParent { get; set; }
    public string? Detail1Parent { get; set; }
    public string? DetailName1Parent { get; set; }
    public string? Detail2Parent { get; set; }
    public string? DetailName2Parent { get; set; }
    public string? WarehouseParent { get; set; }
    public bool? IsDeleted { get; set; }
    public int? GoodID { get; set; }
    public int? GoodsQuotaId { get; set; }

}