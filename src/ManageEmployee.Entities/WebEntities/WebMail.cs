using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.WebEntities;

public class WebMail
{
    public int Id { get; set; }
    [MaxLength(255)]
    public string? Email { get; set; }
    public int? CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
}
