using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.RegisterModels;

public class RegisterModel
{
    [Required]
    public string? FullName { get; set; }

    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }

    public string? Address { get; set; }
    public string? Code { get; set; }

    public string? Identify { get; set; }
}
