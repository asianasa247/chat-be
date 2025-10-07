using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SalaryEntities;

public class SalaryLevel : BaseEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int PositionId { get; set; }
    public string? PositionName { get; set; }
    public int SalaryCost { get; set; }
    public int Amount { get; set; }
    public DateTime Date { get; set; }
    public double Coefficient { get; set; }
    public string? Note { get; set; }
}
