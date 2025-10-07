using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.ContractTypeModels;

public class ContractTypeModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public TypeContractEnum TypeContract { get; set; }
}
