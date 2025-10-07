namespace ManageEmployee.DataTransferObject.Reports;

public class MonthSummary
{
    public int Month { get; set; }
    public double ArisingAmount { get; set; }
    public IEnumerable<LedgerDetail>? LedgerDetails { get; set; }
}
