using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum CategoryEnum
{
    [Description("Nhóm sản phẩm")]
    GoodGroup = 1,
    [Description("Loại hàng")]
    GoodsType2 = 2,
    [Description("Vị trí")]
    Position = 3,
    [Description("Bảng giá")]
    PriceList = 4,
    [Description("Menu web")]
    MenuWeb = 5,
    [Description("Trạng thái hàng lỗi")]
    GoodsErrorStatus = 6,
    [Description("Menu web onepage")]
    MenuWebOnePage = 7,
    [Description("Menu chi tiết 1")]
    DetailNameGood = 8,
}