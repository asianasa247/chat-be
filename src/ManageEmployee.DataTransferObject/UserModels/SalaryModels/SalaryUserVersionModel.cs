namespace ManageEmployee.DataTransferObject.UserModels.SalaryModels;

public class SalaryUserVersionModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? UserFullName { get; set; }
    public string? ContractTypeName { get; set; }
    public double? SalaryTo { get; set; }
    public double? SocialInsuranceSalary { get; set; }
    public DateTime Date { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public string? Note { get; set; }
    public double? Percent { get; set; }// phan tram nhan luong thuc te
}
