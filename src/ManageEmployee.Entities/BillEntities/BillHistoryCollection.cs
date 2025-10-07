using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BillEntities;

public class BillHistoryCollection
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public int StatusUserId { get; set; }
    public int StatusAccountantId { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
}
