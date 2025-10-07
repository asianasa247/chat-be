namespace ManageEmployee.DataTransferObject.SalaryModels;

public class SalaryAllowanceModel
{
    public decimal Value { get; set; }
    public int WorkingDaysTo { get; set; }
    public int WorkingDaysFrom { get; set; }
    public int UserId { get; set; }
}
