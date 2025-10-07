using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BillEntities;

public class BillTracking
{
    public long Id { get; set; }
    public int BillId { get; set; }
    [StringLength(36)]
    public string? UserCode { get; set; }
    [StringLength(255)]
    public string? CustomerName { get; set; }
    [StringLength(36)]
    public string? TranType { get; set; }
    public string? Note { get; set; }
    [StringLength(36)]
    public string? Status { get; set; }
    public bool IsRead { get; set; } = false;
    public bool IsImportant { get; set; } = false;
    public int? UserIdReceived { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public long DisplayOrder { get; set; } = 0;
    public int? Prioritize { get; set; } = 0;
    [StringLength(255)]
    public string? Type { get; set; }// BillTrackingTypeConst
}
