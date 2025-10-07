using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;

public class ExpenditurePlanDetailModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public string? FromProcedureNote { get; set; }
    public int FromProcedureId { get; set; }
    public string? FromProcedureCode { get; set; }
    public double? ExpenditurePlanAmount { get; set; }
    public double? ApproveAmount { get; set; }
    public bool IsApply { get; set; }
    public string? Status { get; set; }
    public List<FileDetailModel>? Files { get; set; }
    public List<FileDetailModel>? FileStatuses { get; set; }
}
