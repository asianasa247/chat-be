namespace ManageEmployee.DataTransferObject.Stationery;
public class StationeryImportModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public List<StationeryImportItemModel>? Items { get; set; }
}
