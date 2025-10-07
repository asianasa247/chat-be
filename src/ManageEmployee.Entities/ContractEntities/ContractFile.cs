using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ContractEntities;

public class ContractFile
{
    public int Id { get; set; }
    [StringLength(500)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    public int ContractTypeId { get; set; }
    public int? DepartmentId { get; set; }
    [StringLength(500)]
    public string? LinkFile { get; set; }
}
