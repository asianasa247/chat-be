using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities;

public class Career : BaseEntity
{
    public int Id { get; set; }

    [StringLength(255)]
    public string? Title { get; set; }

    public CareerGroupType Group { get; set; }

    [StringLength(255)]
    public string? Location { get; set; }

    [StringLength(255)]
    public string? Salary { get; set; }

    public WorkingMethodType WorkingMethod { get; set; }

    [StringLength(255)]
    public string? StartTime { get; set; }

    [StringLength(255)]
    public string? EndTime { get; set; }

    [StringLength(255)]
    public string? Department { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime ExpiredApply { get; set; }
    public string? Description { get; set; }
    public LanguageEnum Type { get; set; }
}
