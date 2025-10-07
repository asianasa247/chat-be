using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class UpdateArisingAccountQueue
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    [StringLength(20)]
    public string? Status { get; set; }// prepare, procesing, success, error
    [StringLength(1000)]
    public string? Message { get; set; }
}
