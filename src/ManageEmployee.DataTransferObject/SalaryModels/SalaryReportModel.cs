namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryReportModel
{
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public string? BankName { get; set; }
    public string? BankAccountNumber { get; set; }
    public double NumberOfWorkingDays { get; set; }
    public double Salary { get; set; }
    public double ContractualSalary { get; set; }
    public double AllowanceAmount { get; set; }
    public double DeduceMealCost { get; set; }
    public double SaleCommission { get; set; }
    public double RemainingAmount { get; set; }
}
