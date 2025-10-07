namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryChangePeriodModel
{
    public int UserId { get; set; }
    public double NumberWorkdays { get; set; }
    public double? SalaryTo { get; set; }
    public double? Percent { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime EffectiveTo { get; set; }
    public double EstimatedSalary => SalaryTo.GetValueOrDefault() * Percent.GetValueOrDefault() / 100;
}
