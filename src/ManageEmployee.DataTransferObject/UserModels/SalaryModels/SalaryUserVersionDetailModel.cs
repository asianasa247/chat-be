namespace ManageEmployee.DataTransferObject.UserModels.SalaryModels;

public class SalaryUserVersionDetailModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Code { get; set; }
    public int? ContractTypeId { get; set; }
    public double? SalaryTo { get; set; }
    public double? Percent { get; set; }
    public double? SocialInsuranceSalary { get; set; }
    public DateTime Date { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Note { get; set; }
}
