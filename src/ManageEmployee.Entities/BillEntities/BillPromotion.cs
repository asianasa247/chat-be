namespace ManageEmployee.Entities.BillEntities;

public class BillPromotion
{
    public int Id { get; set; }
    public int PromotionDetailId { get; set; }
    public int TableId { get; set; }
    public string? TableName { get; set; }
    public string? Note { get; set; }
    public int GoodsPromotionDetailId { get; set; }
    public int GoodsPromotionId { get; set; }
    public string? Standard { get; set; }
    public double Discount { get; set; }
    public double Amount { get; set; }
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Detail1 { get; set; }
    public string? Detail1Name { get; set; }
    public string? Detail2 { get; set; }
    public string? Detail2Name { get; set; }
    public double Qty { get; set; }
    public string? Status { get; set; }// success, refund

    //GoodsPromotion
    public string? GoodsPromotionCode { get; set; }
    public string? GoodsPromotionName { get; set; }

    // produce product
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public int? CustomerId { get; set; }
    public string? Unit { get; set; }
}