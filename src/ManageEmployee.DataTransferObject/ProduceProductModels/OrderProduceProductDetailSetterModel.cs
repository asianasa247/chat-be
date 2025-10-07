namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductDetailSetterModel
{
    public int Id { get; set; }
    public int GoodsId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }

}
