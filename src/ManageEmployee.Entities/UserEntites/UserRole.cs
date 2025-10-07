using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.UserEntites;

public class UserRole
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Title { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    public int Order { get; set; } = 0;
    public int? UserCreated { get; set; }
    public bool IsNotAllowDelete { get; set; } = false;
}