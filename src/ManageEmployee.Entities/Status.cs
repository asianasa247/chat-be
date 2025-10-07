using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities;

public class Status : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CompanyId { get; set; } = 0;
    public bool StatusDetect { get; set; } = false;
    public string? Color { get; set; }
    public int Order { get; set; }
    public StatusTypeEnum Type { get; set; }
}