using ManageEmployee.Entities.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject;

public class AuthenticateModel
{
    [Required]
    public string? Username { get; set; }

    [Required]
    public string? Password { get; set; }
}

public class AuthenticateSocialModel
{
    public string Id { get; set; }
    public string? Name { get; set; }
    public DateTime? Birthday { get; set; }
    public GenderEnum Gender { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Avarta { get; set; }
    public string? Provider { get; set; }
    public string? PhotoUrl { get; set; }
    public string? ProviderId { get; set; }
}