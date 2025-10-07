namespace ManageEmployee.DataTransferObject.SupplyModels;

public class PaymentProposalDetailModel
{
    public int Id { get; set; }
    public int PaymentProposalId { get; set; }
    public string? Note { get; set; }
    public string? Content { get; set; }
    public double Amount { get; set; }
}