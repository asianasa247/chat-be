using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SalaryEntities;

public class SalaryUserVersion : BaseEntityCommon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [MaxLength(255)]
    public string? Code { get; set; }
    [MaxLength(255)]
    public string? UserFullName { get; set; }
    [MaxLength(255)]
    public string? UserName { get; set; }
    public int? ContractTypeId { get; set; }
    public double? SalaryFrom { get; set; }
    public double? SalaryTo { get; set; }
    public double? Percent { get; set; }// phan tram nhan luong thuc te
    public double? SocialInsuranceSalary { get; set; }
    public DateTime Date { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    [MaxLength(500)]
    public string? Note { get; set; }
}