namespace ManageEmployee.DataTransferObject.Reports;

public class VoucherReportItem
{
    public string? VoucherNumber { get; set; } // Số chứng từ
    public DateTime Date { get; set; } // Ngày chứng từ
    public string? Title { get; set; }
    public List<VoucherSubject> Subjects { get; set; } = new List<VoucherSubject>(); // Trích yếu
    public double DebitTotalAmount { get; set; } // Cộng Ghi nợ
    public double CreditTotalAmount { get; set; } // Cộng ghi có
    public string? IncludedVoucherCount { get; set; } // Số chứng từ kèm theo
    public VoucherSubject Details = new VoucherSubject();

}
