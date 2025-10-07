using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.RegisterModels;

public class RegisterLaborWebModel
{
    [Required]
    public string? FullName { get; set; }
    public string? Email { get; set; }
    [Required]
    public string? TelephoneNumber { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Password { get; set; }
}