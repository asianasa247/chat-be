namespace ManageEmployee.DataTransferObject.AddressModels;

public class BranchModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ManagerName { get; set; }
    public bool IsDelete { get; set; } = false;
    public string? TelephoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Image { get; set; }
}
