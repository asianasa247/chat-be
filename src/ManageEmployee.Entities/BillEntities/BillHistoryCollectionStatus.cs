using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BillEntities;

public class BillHistoryCollectionStatus
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
}
