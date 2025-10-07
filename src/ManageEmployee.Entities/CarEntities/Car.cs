using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class Car : BaseEntity
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? LicensePlates { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }

    [StringLength(1000)]
    public string? FileLink { get; set; }
    public string? Content { get; set; }
    public double MileageAllowance { get; set; }
    public double FuelAmount { get; set; }
}
