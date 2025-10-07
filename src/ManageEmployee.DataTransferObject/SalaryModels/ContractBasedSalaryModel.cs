namespace ManageEmployee.DataTransferObject.SalaryModels;

public class ContractBasedSalaryModel
{
    public int UserId { get; set; }
    public double Percent { get; set; }
    public double AmountSpentMonthly { get; set; }
    public double Quantity { get; set; }
    public double AmountPerPercent => Quantity * AmountSpentMonthly * Percent / 100;
}
