namespace ManageEmployee.DataTransferObject.FixedAssetsModels;

public class FixedAssetExport
{
    public int Id { get; set; }
    public DateTime? UsedDate { get; set; }
    public int? TotalMonth { get; set; }
    public double? CarryingAmount { get; set; }
    public string? DepartmentManager { get; set; }
    public string? DepartmentManagerName { get; set; }
    public string? UserManagerName { get; set; }
    public string? CreditCodeName { get; set; }
    public string? CreditDetailCodeFirstName { get; set; }
    public string? CreditDetailCodeSecondName { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditDetailCodeFirst { get; set; }
    public string? CreditDetailCodeSecond { get; set; }
    public short? Use { get; set; }
    public double? Quantity { get; set; }
    public double? HistoricalCost { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public double? UnitPrice { get; set; }

}