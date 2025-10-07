namespace ManageEmployee.DataTransferObject.Reports;

public class GeneralDiaryReportItem
{
    public string? VoucherNumber { get; set; } // Số chứng từ
    public DateTime? OrginalBookDate { get; set; } // Ngày chứng từ
    public DateTime? BookDate { get; set; } // Ngày ghi sổ
    public string? OrginalDescription { get; set; }// dien giai
    public string? DebitCode { get; set; } // Cộng Ghi nợ
    public double Amount { get; set; } // Cộng Ghi nợ
    public string? CreditCode { get; set; } // Cộng ghi có
}
