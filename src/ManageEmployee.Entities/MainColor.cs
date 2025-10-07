using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class MainColor
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Color { get; set; }
    public bool IsDark { get; set; } = false;
    [StringLength(255)]
    public string? Theme { get; set; }
    public int UserId { get; set; }
}