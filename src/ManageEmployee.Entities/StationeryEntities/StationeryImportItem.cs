namespace ManageEmployee.Entities.StationeryEntities;

public class StationeryImportItem
{
    public int Id { get; set; }
    public int StationeryId { get; set; }
    public int StationeryImportId { get; set; }
    public double Quantity { get; set; }
    public double? UnitPrice { get; set; }
}
