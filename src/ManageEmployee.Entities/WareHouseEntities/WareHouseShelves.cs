using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.WareHouseEntities;

public class WareHouseShelves
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }
    public int OrderHorizontal { get; set; }
    public int OrderVertical { get; set; }
}
