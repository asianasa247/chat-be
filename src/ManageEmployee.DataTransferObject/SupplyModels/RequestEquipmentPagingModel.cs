using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;

public class RequestEquipmentPagingModel
{
    public int Id { get; set; }
    public string? UserName { get; set; }
    public int DepartmentId { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsFinished { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
    public List<FileDetailModel>? Files { get; set; }
}
