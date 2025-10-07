namespace ManageEmployee.DataTransferObject;

public class PasswordModel
{
    public int Id { get; set; }
    public string? OldPassword { get; set; }
    public string? Password { get; set; }
}