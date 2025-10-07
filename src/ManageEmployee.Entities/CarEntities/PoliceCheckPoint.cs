using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.CarEntities;

public class PoliceCheckPoint
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string? Code { get; set; }
    [MaxLength(255)]
    public string? Name { get; set; }
    public double Amount { get; set; }
}