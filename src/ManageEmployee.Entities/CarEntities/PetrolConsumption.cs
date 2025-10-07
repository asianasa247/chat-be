using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class PetrolConsumption : BaseEntity
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public int CarId { get; set; }
    public double PetroPrice { get; set; }
    public double KmFrom { get; set; }
    public double KmTo { get; set; }
    [StringLength(500)]
    public string? LocationFrom { get; set; }
    [StringLength(500)]
    public string? LocationTo { get; set; }
    public double AdvanceAmount { get; set; }
    [StringLength(500)]
    public string? Note { get; set; }
    public int? RoadRouteId { get; set; }
}
