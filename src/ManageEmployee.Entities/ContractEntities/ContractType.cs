using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities.ContractEntities;

public class ContractType
{
    public int Id { get; set; }

    [StringLength(500)]
    public string? Name { get; set; }

    [StringLength(36)]
    public string? Code { get; set; }
    public TypeContractEnum TypeContract { get; set; }
}
