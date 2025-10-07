using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class IsoftHistory : BaseEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? ClassName { get; set; }
    public string? Content { get; set; }
    public int? Order { get; set; }
}