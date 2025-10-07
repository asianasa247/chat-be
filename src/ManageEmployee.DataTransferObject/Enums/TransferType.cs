using System.ComponentModel;

namespace ManageEmployee.DataTransferObject.Enums;

public enum TransferType
{
    [Description("Loại chứng từ")]
    DocumentType = 1,
    [Description("Tháng")]
    Month = 2,
}
