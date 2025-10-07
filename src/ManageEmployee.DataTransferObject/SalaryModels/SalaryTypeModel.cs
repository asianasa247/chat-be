namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryTypeModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public double AmountSpent { get; set; }
    public double AmountSpentMonthly { get; set; }
    public double AmountAtTheEndYear { get; set; }
    public string? Note { get; set; }
}