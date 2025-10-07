namespace Common.Models;

public class IdentityUser
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }
    public string? FullName { get; set; }
}