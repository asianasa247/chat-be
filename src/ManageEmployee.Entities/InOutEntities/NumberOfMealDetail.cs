using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class NumberOfMealDetail : BaseEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? TimeType { get; set; }
    public string? Type { get; set; }// user; customer
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public string? CustomerName { get; set; }
    public string? Address { get; set; }
    public double QuantityAdd { get; set; }
    public string? Note { get; set; }
}
