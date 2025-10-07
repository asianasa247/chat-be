namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportViewModel
{
    /// <summary>
    /// ngày ghi sổ
    /// </summary>
    public DateTime? BookDate { get; set; }
    /// <summary>
    /// Ngày chứng từ
    /// </summary>
    public DateTime? OrginalBookDate { get; set; }
    /// <summary>
    /// Số chứng từ
    /// </summary>
    public string? VoucherNumber { get; set; }
    public string? OrginalVoucherNumber { get; set; }
    public string? Description { get; set; }
    public string? TakeNote { get; set; }
    public string? DebitCode { get; set; }
    public string? DebitCodeParent { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditCodeParent { get; set; }
    public double DebitAmount { get; set; }
    public double CreditAmount { get; set; }
    public string? ReciprocalCode { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public bool IsDebit { get; set; }
    public int Type { get; set; }// 1:cộng phát sinh; 2: cộng lũy kế; 3: dư

}
