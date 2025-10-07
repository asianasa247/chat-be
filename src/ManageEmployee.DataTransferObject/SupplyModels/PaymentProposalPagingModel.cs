using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;

public class PaymentProposalPagingModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public DateTime Date { get; set; }
    public string? PaymentMethod { get; set; }
    public double TotalAmount { get; set; }
    public double RefundAmount { get; set; }
    public double AdvanceAmount { get; set; }
    public string? Note { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
    public bool IsFinished { get; set; }
    public string? AdvancePaymentCode { get; set; }
    public string? RequestEquipmentCode { get; set; }
    public string? RequestEquipmentOrderCode { get; set; }
    public List<FileDetailModel>? Files { get; set; }
}
