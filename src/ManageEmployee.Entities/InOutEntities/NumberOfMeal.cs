using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class NumberOfMeal : BaseEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? TimeType { get; set; }
    public double QuantityFromInOut { get; set; }
    public double QuantityAdd { get; set; }
    public string? UserIdStr { get; set; }
}
