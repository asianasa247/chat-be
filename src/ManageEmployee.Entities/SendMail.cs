using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class SendMail
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    [StringLength(255)]
    public string? Title { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? CreateSend { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public int Status { get; set; }
    public DateTime DateSent { get; set; }
}
