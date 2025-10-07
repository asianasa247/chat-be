using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;
public class RequestEquipmentModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DepartmentId { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsSave { get; set; }
    public List<FileDetailModel>? Files { get; set; }
    public List<RequestEquipmentDetailModel>? Items { get; set; }
}
