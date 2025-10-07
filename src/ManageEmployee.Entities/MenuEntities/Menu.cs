using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.MenuEntities;

public class Menu
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(255)]
    public string? NameEN { get; set; }
    [StringLength(255)]
    public string? NameKO { get; set; }
    [StringLength(36)]
    public string? CodeParent { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    public bool IsParent { get; set; }
    public int Order { get; set; }
}
