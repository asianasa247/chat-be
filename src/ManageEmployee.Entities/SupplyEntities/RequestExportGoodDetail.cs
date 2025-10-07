namespace ManageEmployee.Entities.SupplyEntities;

public class RequestExportGoodDetail
{
    public int Id { get; set; }
    public int RequestExportGoodId { get; set; }
    public int GoodId { get; set; }
    public double Quantity { get; set; }
    public double TotalAmout { get; set; }
    public string? Note { get; set; }
}