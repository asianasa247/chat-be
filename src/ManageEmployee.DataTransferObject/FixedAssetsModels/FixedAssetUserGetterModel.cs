using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.DataTransferObject.FixedAssetsModels;

public class FixedAssetUserGetterModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public decimal? HistoricalCost { get; set; }
    public string? VoucherNumber { get; set; }
    public DateTime? UsedDate { get; set; }
    public DateTime? EndOfDepreciation { get; set; }
    public DateTime? LiquidationDate { get; set; }
    public int? TotalMonth { get; set; }
    public decimal? DepreciationOfOneDay { get; set; }
    public decimal? AccruedExpense { get; set; }
    public int? TotalDayDepreciationOfThisPeriod { get; set; }
    public decimal? DepreciationOfThisPeriod { get; set; }
    public decimal? CarryingAmountOfLiquidationAsset { get; set; }
    public decimal? CarryingAmount { get; set; }
    public string? DepartmentManager { get; set; }
    public string? UserManager { get; set; }
    public string? Type { get; set; }
    public CommonModel? Debit { get; set; }
    public string? DebitWarehouse { get; set; }
    public CommonModel? DebitDetailFirst { get; set; }
    public CommonModel? DebitDetailSecond { get; set; }
    public CommonModel? Credit { get; set; }
    public string? CreditWarehouse { get; set; }
    public CommonModel? CreditDetailFirst { get; set; }
    public CommonModel? CreditDetailSecond { get; set; }
    public string? InvoiceNumber { get; set; }

    public string? InvoiceTaxCode { get; set; }
    public string? InvoiceSerial { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public short Use { get; set; }
    public int? DepartmentId { get; set; }
    public int? UserId { get; set; }
    public DateTime? BuyDate { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? AttachVoucher { get; set; }
    public string? Note { get; set; }
    public string? UsedCode { get; set; }
}
