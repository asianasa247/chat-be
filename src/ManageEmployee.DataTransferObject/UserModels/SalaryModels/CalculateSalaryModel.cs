namespace ManageEmployee.DataTransferObject.UserModels.SalaryModels;

public class CalculateSalaryModel
{
    public string? UserName { get; set; }
    public string? FullName { get; set; }
    public string? BankAccount { get; set; }
    public string? Bank { get; set; }
    public double InoutCount { get; set; }
    public double Salary { get; set; }
    public double ContractualSalary { get; set; }
    public double AttendanceBonus { get; set; }
    public double MealAllowance { get; set; }
    public double RemainingAmount { get; set; }
}