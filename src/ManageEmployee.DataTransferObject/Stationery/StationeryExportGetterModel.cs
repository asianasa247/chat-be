namespace ManageEmployee.DataTransferObject.Stationery;

public class StationeryExportGetterModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public int DepartmentId { get; set; }
    public int UserId { get; set; }
}
