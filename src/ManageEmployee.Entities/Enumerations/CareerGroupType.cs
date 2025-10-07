using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum CareerGroupType
{
    [Description("Khối văn phòng")]
    Office = 1,

    [Description("Khối bán hàng")]
    Sale = 2,
}
