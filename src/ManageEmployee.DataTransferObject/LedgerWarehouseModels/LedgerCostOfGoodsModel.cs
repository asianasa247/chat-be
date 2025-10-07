namespace ManageEmployee.DataTransferObject.LedgerWarehouseModels;

public class LedgerCostOfGoodsModel
{
    public long Id { get; set; }
    public string? OrginalVoucherNumber { get; set; }
    public DateTime? OrginalBookDate { get; set; }
    public string? OrginalDescription { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditDetailCodeFirst { get; set; }
    public string? CreditDetailCodeSecond { get; set; }
    public string? CreditCodeName { get; set; }
    public string? CreditDetailCodeFirstName { get; set; }
    public string? CreditDetailCodeSecondName { get; set; }
    public string? DebitCode { get; set; }
    public string? DebitDetailCodeFirst { get; set; }
    public string? DebitDetailCodeSecond { get; set; }
    public string? DebitCodeName { get; set; }
    public string? DebitDetailCodeFirstName { get; set; }
    public string? DebitDetailCodeSecondName { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double Amount { get; set; }
    public string? CreditWarehouse { get; set; }
    public string? CreditWarehouseName { get; set; }
    public string? Type { get; set; }
    public string? VoucherNumber { get; set; }
    public int Month { get; set; }

    public string? RevenueCode { get; set; }
    public double RevenueUnitPrice { get; set; }
    public double RevenueAmmountPrice { get; set; }

    public int? Order { get; set; }

}