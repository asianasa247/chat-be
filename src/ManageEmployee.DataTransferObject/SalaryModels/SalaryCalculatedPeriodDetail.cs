namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryCalculatedPeriodDetail
{
    public double WorkingHours { get; set; }
    public double SalaryPerDay { get; set; }
    public double WorkingDays { get; set; }
    public double ShiftHoursNumber { get; set; }
    public double Salary { get; set; }
    public double EstimatedSalary { get; set; }
    public double WorkingDaysInMonth { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime FromDate { get; set; }
}
