using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class CarField : BaseEntityCommon
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public int Order { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
}
