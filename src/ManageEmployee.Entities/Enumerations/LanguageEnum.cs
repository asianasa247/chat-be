using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum LanguageEnum
{
    [Description("Tiếng Hàn Quốc")]
    Korea = 1,

    [Description("Tiếng Việt Nam")]
    VietNam = 2,

    [Description("Tiếng Anh")]
    English = 3
}
