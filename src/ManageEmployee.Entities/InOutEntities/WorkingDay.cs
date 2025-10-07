using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.InOutEntities;

public class WorkingDay
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string? Days { get; set; }
    [MaxLength(500)]
    public string? Holidays { get; set; }
    public int Year { get; set; }
}