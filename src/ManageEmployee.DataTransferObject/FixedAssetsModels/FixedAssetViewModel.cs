using Common.Helpers;
using ManageEmployee.Entities.DocumentEntities;

namespace ManageEmployee.DataTransferObject.FixedAssetsModels;

public class FixedAssetViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double? HistoricalCost { get; set; }
    public string VoucherNumber { get; set; }
    public DateTime? UsedDate { get; set; }
    public DateTime? BuyDate { get; set; }
    public DateTime? EndOfDepreciation { get; set; }
    public DateTime? LiquidationDate { get; set; }
    public int? TotalMonth { get; set; }
    public double? DepreciationOfOneDay { get; set; }
    public double? AccruedExpense { get; set; }
    public int? TotalDayDepreciationOfThisPeriod { get; set; }
    public double? DepreciationOfThisPeriod { get; set; }
    public double? CarryingAmountOfLiquidationAsset { get; set; }
    public double? CarryingAmount { get; set; }
    public string DepartmentManager { get; set; }
    public string DepartmentManagerName { get; set; }
    public string UserManager { get; set; }
    public string UserManagerName { get; set; }
    public string Type { get; set; }
    public string DebitCodeName { get; set; }
    public string DebitDetailCodeFirstName { get; set; }
    public string DebitDetailCodeSecondName { get; set; }
    public string CreditCodeName { get; set; }
    public string CreditDetailCodeFirstName { get; set; }
    public string CreditDetailCodeSecondName { get; set; }
    public string DebitCode { get; set; }
    public string DebitWarehouse { get; set; }
    public string DebitDetailCodeFirst { get; set; }
    public string DebitDetailCodeSecond { get; set; }
    public string CreditCode { get; set; }
    public string CreditWarehouse { get; set; }
    public string CreditDetailCodeFirst { get; set; }
    public string CreditDetailCodeSecond { get; set; }
    public string InvoiceNumber { get; set; }
    public string InvoiceTaxCode { get; set; }
    public string InvoiceSerial { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public short? Use { get; set; }
    public double? Quantity { get; set; }
    public double? UnitPrice { get; set; }

    public int? DepartmentId { get; set; }
    public int? UserId { get; set; }
    public double? UsedDateUnix { get; set; }
    public FixedAssetViewModel() { }
    public FixedAssetViewModel(FixedAsset fixedAsset)
    {
        Id = fixedAsset.Id;
        Name = fixedAsset.Name;
        HistoricalCost = fixedAsset.HistoricalCost;
        VoucherNumber = fixedAsset.VoucherNumber;
        UsedDate = fixedAsset.UsedDate;
        EndOfDepreciation = fixedAsset.EndOfDepreciation;
        LiquidationDate = fixedAsset.LiquidationDate;
        TotalMonth = fixedAsset.TotalMonth;
        DepreciationOfOneDay = fixedAsset.DepreciationOfOneDay;
        AccruedExpense = fixedAsset.AccruedExpense;
        TotalDayDepreciationOfThisPeriod = fixedAsset.TotalDayDepreciationOfThisPeriod;
        DepreciationOfThisPeriod = fixedAsset?.DepreciationOfThisPeriod;
        CarryingAmountOfLiquidationAsset = fixedAsset?.CarryingAmountOfLiquidationAsset;
        CarryingAmount = fixedAsset?.CarryingAmount;
        DepartmentManager = fixedAsset?.DepartmentManager;
        UserManager = fixedAsset?.UserManager;
        Type = fixedAsset?.Type;
        DebitCode = fixedAsset?.DebitCode;
        DebitCodeName = fixedAsset?.DebitCodeName;
        DebitDetailCodeFirst = fixedAsset?.DebitDetailCodeFirst;
        DebitDetailCodeFirstName = fixedAsset?.DebitDetailCodeFirstName;
        DebitDetailCodeSecond = fixedAsset?.DebitDetailCodeSecond;
        DebitDetailCodeSecondName = fixedAsset?.DebitDetailCodeSecondName;
        DebitWarehouse = fixedAsset?.DebitWarehouse;
        CreditCode = fixedAsset?.CreditCode;
        CreditCodeName = fixedAsset?.CreditCodeName;
        CreditDetailCodeFirst = fixedAsset?.CreditDetailCodeFirst;
        CreditDetailCodeFirstName = fixedAsset?.CreditDetailCodeFirstName;
        CreditDetailCodeSecond = fixedAsset?.CreditDetailCodeSecond;
        CreditDetailCodeSecondName = fixedAsset?.CreditDetailCodeSecondName;
        CreditWarehouse = fixedAsset?.CreditWarehouse;
        InvoiceNumber = fixedAsset?.InvoiceNumber;
        InvoiceTaxCode = fixedAsset.InvoiceTaxCode;
        InvoiceSerial = fixedAsset?.InvoiceSerial;
        InvoiceDate = fixedAsset?.InvoiceDate;
        Use = fixedAsset?.Use;
        DepartmentId = fixedAsset?.DepartmentId;
        UserId = fixedAsset?.UserId;
        UsedDateUnix = DateHelpers.DateTimeToUnixTimeStamp(UsedDate ?? DateTime.Now);
        Quantity = fixedAsset?.Quantity;
    }
}
