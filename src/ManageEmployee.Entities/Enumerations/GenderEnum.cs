using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum GenderEnum : short
{
    [Description("All")]
    All = -1,
    [Description("Male")]
    Male = 0,
    [Description("Female")]
    Female = 1,
    [Description("Other")]
    Other = 2,
}
