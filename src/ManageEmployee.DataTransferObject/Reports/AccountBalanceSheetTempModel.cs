namespace ManageEmployee.DataTransferObject.Reports;

public class AccountBalanceSheetTempModel
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Parent { get; set; }
    public string? ParentRef { get; set; }
    public string? Hash { get; set; }
    public int Type { get; set; }
    public bool IsForeignCurrency { get; set; }
    public double OpeningDebit { get; set; }
    public double OpeningCredit { get; set; }
    public double OpeningForeignDebit { get; set; }
    public double OpeningForeignCredit { get; set; }
    public double ArisingDebit { get; set; } = 0;
    public double ArisingCredit { get; set; } = 0;
    public double ArisingForeignDebit { get; set; } = 0;
    public double ArisingForeignCredit { get; set; } = 0;
    public double CumulativeArisingDebit { get; set; } = 0;
    public double CumulativeArisingCredit { get; set; } = 0;
    public double CumulativeArisingForeignDebit { get; set; } = 0;
    public double CumulativeArisingForeignCredit { get; set; } = 0;
    public double ClosingDebit { get; set; } = 0;
    public double ClosingCredit { get; set; } = 0;
    public double ClosingForeignDebit { get; set; } = 0;
    public double ClosingForeignCredit { get; set; } = 0;
    public List<AccountBalanceSheetTempModel> Childs { get; set; } = new List<AccountBalanceSheetTempModel>();
    public AccountBalanceSheetTempModel? Ref { get; set; }
    public bool HasChild { get; set; }
    public string? WarehouseCode { get; set; }
    public string? Duration { get; set; }
    //
    public double OpeningDebitNB { get; set; }
    public double OpeningCreditNB { get; set; }
    public double OpeningForeignDebitNB { get; set; }
    public double OpeningForeignCreditNB { get; set; }
}
