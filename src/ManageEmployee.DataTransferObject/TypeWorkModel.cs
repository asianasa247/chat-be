namespace ManageEmployee.DataTransferObject;

public class TypeWorkModel
{
    public int Id { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? BranchId { get; set; }
    public string? BranchName { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Color { get; set; }
    public double? Point { get; set; }
}
