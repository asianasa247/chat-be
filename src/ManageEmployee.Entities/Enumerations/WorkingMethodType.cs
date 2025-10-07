using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum WorkingMethodType
{
    [Description("Toàn thời gian")]
    FullTime = 1,

    [Description("Bán thới gian")]
    PartTime = 2,

    [Description("Ca")]
    Shift = 3
}