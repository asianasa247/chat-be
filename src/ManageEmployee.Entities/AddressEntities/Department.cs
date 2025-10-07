namespace ManageEmployee.Entities.AddressEntities;

public class Department
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public bool isDelete { get; set; } = false;
}