namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportSumRow
{

    public double SumDebit { get; set; }
    public double SumCredit { get; set; }
    public double SumDebitLuyKe { get; set; }
    public double SumCreditLuyKe { get; set; }
    public double SumDebitDuCT { get; set; }
    public double SumCreditDuCT { get; set; }
    public bool isDebit { get; set; }
}
