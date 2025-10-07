using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum CartStateEnum
{
    [Description("Chưa mua")]
    WAIT = 0,
    [Description("Đã mua")]
    BOUGHT = 1,
    [Description("Đã xóa khỏi giỏ hàng")]
    DELETED = 2,
}
