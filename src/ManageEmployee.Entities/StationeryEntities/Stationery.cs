using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.StationeryEntities;

public class Stationery : BaseEntity
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Code { get; set; }
    [StringLength(500)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? Unit { get; set; }
}
