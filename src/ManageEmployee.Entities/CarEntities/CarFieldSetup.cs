using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class CarFieldSetup : BaseEntityCommon
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int CarFieldId { get; set; }
    public double? ValueNumber { get; set; }
    public DateTime? FromAt { get; set; }
    public DateTime? ToAt { get; set; }
    public DateTime? WarningAt { get; set; }
    [StringLength(500)]
    public string? UserIdString { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }

    [StringLength(500)]
    public string? FileLink { get; set; }
}
