using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class EventWithImage
{
    public int Id { get; set; }
    [MaxLength(500)]
    public string? Name { get; set; }
    public int Order { get; set; }
    [MaxLength(5000)]
    public string? FileLink { get; set; }
    public DateTime Date { get; set; }
    [MaxLength(500)]
    public string? LinkDriver { get; set; }
    [MaxLength(500)]
    public string? Note { get; set; }
}
