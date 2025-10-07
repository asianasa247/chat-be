using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum GoodStatusEnum
{
    [Description("Dang kinh doanh")]
    DangKinhDoanh = 1,
    [Description("Ngung kinh doanh")]
    NgungKinhDoanh = 0,
}
