namespace ChatappLC.Domain.Entities;

public class User
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public required string PhoneNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public required string Password { get; set; }
    public string Role { get; set; } = string.Empty;
}

