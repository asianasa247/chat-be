namespace ManageEmployee.DataTransferObject.Reports;

public class IncurExpense
{
    public string Name { get; set; } = string.Empty;
    public double SumDebit { get; set; } = 0;
    public double SumCredit { get; set; } = 0;
    public double Balance { get; set; } = 0;
    public double Expense { get; set; } = 0;
    public double AccumulatedDebit { get; set; } = 0;
    public double AccumulatedCredit { get; set; } = 0;
    public double OpeningStock { get; set; } = 0;
}
