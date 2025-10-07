namespace ManageEmployee.DataTransferObject.SupplyModels;
public class RequestExportGoodModel
{
    public int Id { get; set; }
    public int DepartmentId { get; set; }
    public int UserId { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public DateTime Date { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public List<RequestExportGoodDetailModel>? Items { get; set; }
}
