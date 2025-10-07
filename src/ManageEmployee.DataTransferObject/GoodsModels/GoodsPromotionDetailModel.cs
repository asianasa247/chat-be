using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodsPromotionDetailModel
{
    public int Id { get; set; }
    public string? Standard { get; set; }
    public double Discount { get; set; }
    public double Qty { get; set; }
    public CommonModel? AccountObj { get; set; }
    public CommonModel? Detail1Obj { get; set; }
    public CommonModel? Detail2Obj { get; set; }
}
