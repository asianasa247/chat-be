namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportCalculator
{
    public double OpeningDebit { get; set; }
    public double OpeningCredit { get; set; }
    public double OpeningBacklog { get; set; }
    public double ArisingDebit { get; set; }
    public double ArisingCredit { get; set; }
    public double ArisingBacklog { get; set; }
    public double ClosingDebit { get; set; }
    public double ClosingCredit { get; set; }
    public double ClosingBacklog { get; set; }
    public double AccumulatedDebit { get; set; }
    public double AccumulatedCredit { get; set; }
    public double AccumulatedBacklog { get; set; }
}
