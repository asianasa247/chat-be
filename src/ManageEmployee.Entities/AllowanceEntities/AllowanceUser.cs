using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.AllowanceEntities;

public class AllowanceUser : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AllowanceId { get; set; }
    public decimal Value { get; set; }
    public string? Note { get; set; }
    public int WorkingDaysFrom { get; set; }
    public int WorkingDaysTo { get; set; }
}