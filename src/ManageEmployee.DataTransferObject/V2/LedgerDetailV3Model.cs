namespace ManageEmployee.DataTransferObject.V2;

public class LedgerDetailV3Model : LedgerDetailV2Model
{
    public int? Tab { get; set; }
    public double? PercentImportTax { get; set; }
    public double? PercentTransport { get; set; }
    public double? AmountTransport { get; set; }
    public double? AmountImportWarehouse { get; set; }
}
