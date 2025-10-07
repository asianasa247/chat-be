using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;

public class PaymentProposalModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime? Date { get; set; }
    public string? PaymentMethod { get; set; }
    public double TotalAmount { get; set; }
    public double RefundAmount { get; set; }
    public double AdvanceAmount { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsImmediate { get; set; }
    public int? AdvancePaymentId { get; set; }
    public int? RequestEquipmentId { get; set; }
    public int? RequestEquipmentOrderId { get; set; }
    public List<FileDetailModel>? Files { get; set; }
    public List<PaymentProposalDetailModel>? Items { get; set; }
}
