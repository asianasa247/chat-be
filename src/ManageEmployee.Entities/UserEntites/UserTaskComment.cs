using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.UserEntites;

public class UserTaskComment
{
    public int Id { get; set; }
    public int? UserTaskId { get; set; }
    public int? UserId { get; set; }
    [StringLength(36)]
    public string? Type { get; set; }
    public string? Comment { get; set; }
    public int? ParentId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string? FileLink { get; set; }
}
