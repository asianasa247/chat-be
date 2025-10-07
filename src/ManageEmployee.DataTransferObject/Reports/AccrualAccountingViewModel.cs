namespace ManageEmployee.DataTransferObject.Reports;

public class AccrualAccountingViewModel
{
    public double OpeningStock { get; set; } = 0; // Tồn đầu kỳ
    public double ClosingStock { get; set; } = 0; // Tồn cuối kỳ
    public List<IncurExpense>? IncurExpenses { get; set; } // List phát sinh
}
