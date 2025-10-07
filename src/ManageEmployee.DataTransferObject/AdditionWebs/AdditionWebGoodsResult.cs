using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataTransferObject.AdditionWebs;

public class AdditionWebGoodsResult : Goods
{
    public bool IsSelected { get; set; }
    public string FullImage1 { get; set; }

    public string FullImage2 { get; set; }

    public string FullImage3 { get; set; }

    public string FullImage4 { get; set; }

    public string FullImage5 { get; set; }
}
