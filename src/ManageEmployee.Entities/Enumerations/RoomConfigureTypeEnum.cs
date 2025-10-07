using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum RoomConfigureTypeEnum
{
    [Description("Kiểu phòng")]
    RoomType,
    [Description("Loại giường")]
    BedType,
    [Description("Loại tiện ích")]
    AmenityType,
}
