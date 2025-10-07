namespace ManageEmployee.Entities.SupplyEntities;

public class AdvancePaymentDetail
{
    public int Id { get; set; }
    public int AdvancePaymentId { get; set; }
    public double Amount { get; set; }
    public string? Note { get; set; }
}