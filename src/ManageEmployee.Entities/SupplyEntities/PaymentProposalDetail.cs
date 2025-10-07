namespace ManageEmployee.Entities.SupplyEntities;

public class PaymentProposalDetail
{
    public int Id { get; set; }
    public int PaymentProposalId { get; set; }
    public string? Note { get; set; }
    public string? Content { get; set; }
    public double Amount { get; set; }
}