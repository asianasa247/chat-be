using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum OrderStatus
{
    [Description("Mới tạo")]
    CREATED = 1,
    [Description("Đã xác nhận")]
    CONFIRMED = 2,
    [Description("Đang giao")]
    SHIPPING = 3,
    [Description("Đã giao")]
    SHIPPED = 4,
    [Description("Hoàn thành")]
    COMPLETED = 5,
    [Description("Hủy")]
    CANCELED = 6,
}
