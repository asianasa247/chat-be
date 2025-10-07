using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject;

public class HistoryAchievementModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Description { get; set; }
    public string? FullName { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public DateTime? DeleteAt { get; set; }
    public bool IsDelete { get; set; } = false;
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public IFormFile? File { get; set; }
}