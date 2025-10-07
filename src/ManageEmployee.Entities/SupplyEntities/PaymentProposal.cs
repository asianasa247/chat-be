using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.SupplyEntities;

public class PaymentProposal : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? AdvancePaymentId { get; set; }
    public string? AdvancePaymentCode { get; set; }
    public int? RequestEquipmentId { get; set; }
    public string? RequestEquipmentCode { get; set; }
    public int? RequestEquipmentOrderId { get; set; }
    public string? RequestEquipmentOrderCode { get; set; }

    public DateTime Date { get; set; }
    public string? PaymentMethod { get; set; }
    public double TotalAmount { get; set; }
    public double RefundAmount { get; set; }
    public double AdvanceAmount { get; set; }
    public bool IsImmediate { get; set; }
    public string? Note { get; set; }
    public string? TableName { get; set; }
    public int? TableId { get; set; }
    public string? FileStr { get; set; }
    public bool IsDone { get; set; }
    public bool IsPart { get; set; }
    public bool IsInprogress { get; set; }
}
