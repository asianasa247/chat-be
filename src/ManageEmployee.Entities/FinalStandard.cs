using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class FinalStandard : BaseEntity
{
    public int Id { get; set; }
    public string? CreditCode { get; set; }
    public string? DebitCode { get; set; }
    public double? PercentRatio { get; set; }
    public string? Type { get; set; }
}
