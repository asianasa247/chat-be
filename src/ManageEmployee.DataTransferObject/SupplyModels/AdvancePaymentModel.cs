using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;
public class AdvancePaymentModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int UserId { get; set; }
    public double Amount { get; set; }
    public string? Note { get; set; }
    public DateTime DatePayment { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsImmediate { get; set; }
    public FileDetailModel? SettlementFile { get; set; }
    public List<AdvancePaymentDetailModel>? Items { get; set; }
    public List<FileDetailModel>? Files { get; set; }

}
