namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryCalculatedModel
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public double Salary { get; set; }
    public double WorkingDays { get; set; }
    public List<SalaryCalculatedPeriodDetail>? Detail { get; set; }
}
