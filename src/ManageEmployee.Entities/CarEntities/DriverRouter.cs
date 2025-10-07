using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class DriverRouter : BaseEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public string? Status { get; set; }
    public int PetrolConsumptionId { get; set; }
    public double? AdvancePaymentAmount { get; set; }
    public double? FuelAmount { get; set; }
}
