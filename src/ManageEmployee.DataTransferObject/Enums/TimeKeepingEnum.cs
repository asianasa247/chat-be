using System.ComponentModel;

namespace ManageEmployee.DataTransferObject.Enums;

public enum InOutTimeLineStatusEnum
{
    [Description("INCOMING")]
    InComing,
    [Description("IN_PROGRESS")]
    InProgress,
    [Description("PASSED")]
    Passed,
}