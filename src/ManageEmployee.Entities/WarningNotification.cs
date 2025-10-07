namespace ManageEmployee.Entities;

public class WarningNotification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Message { get; set; }
    public int Status { get; set; }
    public int WarningId { get; set; }
    public string? WarningTableName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Date { get; set; }
    public bool IsDeleted { get; set; }
}
