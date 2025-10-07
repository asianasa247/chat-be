namespace ManageEmployee.DataTransferObject.Stationery;

public class StationeryImportItemModel
{
    public int Id { get; set; }
    public int StationeryId { get; set; }
    public int StationeryImportId { get; set; }
    public double Quantity { get; set; }
    public double? UnitPrice { get; set; }
}
