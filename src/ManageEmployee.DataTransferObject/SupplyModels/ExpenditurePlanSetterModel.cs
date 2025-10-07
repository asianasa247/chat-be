namespace ManageEmployee.DataTransferObject.SupplyModels;

public class ExpenditurePlanSetterModel
{
    public List<int>? PaymentProposalIds { get; set; }
    public List<int>? AdvancePaymentIds { get; set; }
    public string? Note { get; set; }
    public int Id { get; set; }
}
