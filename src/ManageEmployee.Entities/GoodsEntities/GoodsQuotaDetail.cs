namespace ManageEmployee.Entities.GoodsEntities;

public class GoodsQuotaDetail
{
    public int Id { get; set; }
    public int GoodsQuotaId { get; set; }
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Detail1 { get; set; }
    public string? DetailName1 { get; set; }
    public string? Detail2 { get; set; }
    public string? DetailName2 { get; set; }
    public string? Warehouse { get; set; }
    public double? Quantity { get; set; }
}
