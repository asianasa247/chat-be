namespace ManageEmployee.DataTransferObject.Reports;
#region FOR SỔ CHI TIẾT

#endregion
public class LedgerReportTonSLViewModel
{
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Detail1 { get; set; }
    public string? Detail1Name { get; set; }
    public string? Detail2 { get; set; }
    public string? Detail2Name { get; set; }
    public double OpenQuantity { get; set; }
    public double InputQuantity { get; set; }
    public double OutputQuantity { get; set; }
    public double CloseQuantity { get; set; }
    public string? NameGood { get; set; }


    public double OpenAmount { get; set; }
    public double InputAmount { get; set; }
    public double OutputAmount { get; set; }
    public double CloseAmount { get; set; }

}
