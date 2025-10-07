using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class RelationShip : BaseEntity
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    [StringLength(255)]
    public string EmployeeName { get; set; } = "";
    public int PersonOppositeId { get; set; } = 0;
    [StringLength(255)]
    public string PersonOppositeName { get; set; } = "";
    [StringLength(255)]
    public string ClaimingYourself { get; set; } = "";
    [StringLength(255)]
    public string ProclaimedOpposite { get; set; } = "";
    public int? Type { get; set; } = 0;// 1: trong cty, 2: ngoai cong ty
}
