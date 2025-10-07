using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.LedgerModels;

public class LedgerProduceProductModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public string? Type { get; set; }
    public List<FileDetailModel>? Files { get; set; }
    public List<LedgerProduceProductDetailModel>? Items { get; set; }
}
