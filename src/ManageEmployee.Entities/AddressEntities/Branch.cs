using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.AddressEntities;

public class Branch
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ManagerName { get; set; }
    [StringLength(36)]
    public string? TelephoneNumber { get; set; }
    [StringLength(500)]
    public string? Address { get; set; }
    [StringLength(255)]
    public string? Image { get; set; }
    public bool IsDelete { get; set; } = false;

}
