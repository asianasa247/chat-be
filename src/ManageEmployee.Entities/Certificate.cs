using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class Certificate : BaseEntity
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? Description { get; set; }
    public int? CompanyId { get; set; } = 0;
    public bool Status { get; set; } = false;
}