using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.LedgerEntities;

public class LedgerWareHouse
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    [StringLength(1000)]
    public string? LedgerIds { get; set; }

    public int LedgerCount { get; set; }
    public int? CustomerId { get; set; }
    public int? Month { get; set; }
    public int? IsInternal { get; set; }
    public string? Type { get; set; }
    public int CreateBy { get; set; }
    public int UpdateBy { get; set; }
}
