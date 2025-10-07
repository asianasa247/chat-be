namespace ManageEmployee.DataTransferObject;

public class AuthRoleModel
{
    public string? Name { get; set; }
    public IList<string>? Roles { get; set; }
}