using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class Degree : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CompanyId { get; set; }
    public bool Status { get; set; }
    public int? Order { get; set; }
}