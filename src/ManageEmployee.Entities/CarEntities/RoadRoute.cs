using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class RoadRoute : BaseEntityCommon
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string? Code { get; set; }
    [MaxLength(255)]
    public string? Name { get; set; }
    [MaxLength(1000)]
    public string? RoadRouteDetail { get; set; }
    public string? PoliceCheckPointIdStr { get; set; }
    public double NumberOfTrips { get; set; }
}