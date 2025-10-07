namespace ManageEmployee.DataTransferObject.Reports;

public class AccountBalanceItemModel
{
    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }
    public int AccountType { get; set; }
    public double OpeningDebit { get; set; }
    public double OpeningCredit { get; set; }
    public double ArisingDebit { get; set; }
    public double ArisingCredit { get; set; }
    public double CumulativeDebit { get; set; }
    public double CumulativeCredit { get; set; }
    public double ClosingDebit { get; set; }
    public double ClosingCredit { get; set; }
    public bool IsForeign { get; set; }
    public string? Hash { get; set; }
    public string? ParentRef { get; set; }
    public string? Duration { get; set; }
    public bool HasChild { get; set; }
    // nội bộ
    public double OpeningDebitNB { get; set; }
    public double OpeningCreditNB { get; set; }
}
