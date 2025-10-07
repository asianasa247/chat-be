using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class HistoryAchievement
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    [StringLength(255)]
    public string? Description { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public DateTime? DeleteAt { get; set; }
    public bool IsDelete { get; set; } = false;
    public int? UserCreated { get; set; }
    public int? UserUpdated { get; set; }
    [StringLength(500)]
    public string? FileUrl { get; set; }
    [StringLength(255)]
    public string? FileName { get; set; }
}