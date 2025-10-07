using System.ComponentModel;

namespace ManageEmployee.Entities.Enumerations;

public enum ErrorEnum : short
{
    [Description("SUCCESS")]
    SUCCESS = 200,

    [Description("BAD_REQUEST")]
    BAD_REQUEST = 400,

    [Description("NOT_FOUND")]
    NOT_FOUND = 404,

    [Description("USER_IS_NOT_EXIST")]
    USER_IS_NOT_EXIST = 601,

    [Description("ERROR_PASS")]
    ERROR_PASS = 602,

    [Description("DEPARTMENT_CODE_IS_EXIST")]
    DEPARTMENT_CODE_IS_EXIST = 603,

    [Description("DEPARTMENT_IS_USE")]
    DEPARTMENT_IS_USE = 604,

    [Description("TAX_RATE_CODE_IS_EXIST")]
    TAX_RATE_CODE_IS_EXIST = 605,

    [Description("GOODS_IS_EXIST")]
    GOODS_IS_EXIST = 606,

    [Description("STATUS_CODE_IS_EXIST")]
    STATUS_CODE_IS_EXIST = 607,
}
