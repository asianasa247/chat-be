using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.UserEntites;

public class UserTaskCheckList
{
    public int Id { get; set; }
    public int? UserTaskId { get; set; }
    [StringLength(1000)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? FileLink { get; set; }
    public bool? Status { get; set; }
}
