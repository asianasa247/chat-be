namespace ManageEmployee.Entities.GoodsEntities;

public class GoodsPromotionDetail
{
    public int Id { get; set; }
    public int GoodsPromotionId { get; set; }
    public string? Standard { get; set; }
    public double Discount { get; set; }
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Detail1 { get; set; }
    public double Qty { get; set; }
    public string? Detail1Name { get; set; }
    public string? Detail2 { get; set; }
    public string? Detail2Name { get; set; }
}