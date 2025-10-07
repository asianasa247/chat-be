namespace ManageEmployee.DataTransferObject.ContractTypeModels;

public class ContractFileModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int ContractTypeId { get; set; }
    public int? DepartmentId { get; set; }
    public string? LinkFile { get; set; }
}
