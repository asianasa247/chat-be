namespace ManageEmployee.DataTransferObject.Stationery;

public class StationeryImportGetterModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public double Quantity { get; set; }
    public double TotalAmount { get; set; }
}
