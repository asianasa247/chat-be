using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.AllowanceEntities;

public class Allowance : BaseEntity
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int? CompanyId { get; set; }
    public bool Status { get; set; }
}