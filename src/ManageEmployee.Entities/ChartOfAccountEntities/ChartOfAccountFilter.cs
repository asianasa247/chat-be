using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ChartOfAccountEntities;

public class ChartOfAccountFilter
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(500)]
    public string? Accounts { get; set; }
    [StringLength(255)]
    public string? Type { get; set; }
    [StringLength(255)]
    public string? DocumentCode { get; set; }
    public int Order { get; set; }
    public bool IsReverse { get; set; }
}
