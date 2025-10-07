using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.MenuEntities;

public class MenuKpi
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(36)]
    public string? Name { get; set; }
    public double Point { get; set; }
    public double? FromValue { get; set; }
    public double? ToValue { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    public int Type { get; set; } = 0;// 0: cham cong; 1: doanh so
}
