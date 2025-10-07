namespace ManageEmployee.DataTransferObject.UserModels;

public class User_SalaryModel
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public double Salary { get; set; }
    public int? ContractTypeId { get; set; }
    public int DepartmentId { get; set; }
    public string? PositionName { get; set; }
    public double SalaryTotal { get; set; }
    public int? DayInOut { get; set; }
    public double SalaryReal { get; set; }
    public double ThueTNCN { get; set; }
    public double TamUng { get; set; }
    public double SalarySend { get; set; }
    public string? SoThuTu { get; set; }
    public List<User_SalaryModel>? listChild { get; set; }
    public List<SalarySocialModel>? salarySocial { get; set; }
    public List<AllowanceUserModel>? allowances { get; set; }
    public double? NumberWorkdays { get; set; } = 0;
    public int? DayOff { get; set; }
    public double SocialInsuranceSalary { get; set; } = 0;
    public bool IsManager { get; set; }
    public bool IsProbation { get; set; }
    public double? SalaryPercentage { get; set; }
}
