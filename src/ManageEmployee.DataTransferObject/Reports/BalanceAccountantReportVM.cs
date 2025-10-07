namespace ManageEmployee.DataTransferObject.Reports;

public class BalanceAccountantReportVM
{
    public string? Company { get; set; } // Tên công ty
    public string? Address { get; set; } // Địa chỉ
    public string? TaxId { get; set; } // Mã số thuế
    public string? Type { get; set; } // Loại chứng từ
    public string? TypeName { get; set; } // Tên loại chứng từ
    public string? ChiefAccountantName { get; set; } // Tên kế toán trưởng
    public string? CEOName { get; set; } // Tên giám đốc
    public string? VoteMaker { get; set; }
    public string? ChiefAccountantNote { get; set; } // Tên kế toán trưởng
    public string? CEONote { get; set; } // Tên giám đốc
}
