using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject;

public class ResetPasswordModel
{
    [Required]
    public string? Username { get; set; }
}
