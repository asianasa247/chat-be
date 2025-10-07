using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.UserEntites;

public class UserTaskTracking
{
    public int Id { get; set; }
    public int? UserTaskId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? UserCreated { get; set; }
    public int? UserUpdated { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public double? ActualHours { get; private set; }
}
