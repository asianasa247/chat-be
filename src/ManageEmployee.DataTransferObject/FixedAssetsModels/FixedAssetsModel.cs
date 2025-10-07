using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.DataTransferObject.FixedAssetsModels;

public class FixedAssetsModel : FixedAsset
{
    public FixedAssetsModel FromLedger(Ledger leg)
    {
        HistoricalCost = leg.Amount;
        VoucherNumber = leg.VoucherNumber;
        UsedDate = leg.OrginalBookDate;
        TotalMonth = leg.DepreciaMonth;
        CreditCodeName = leg.DebitCodeName;
        CreditDetailCodeFirstName = leg.DebitDetailCodeFirstName;
        CreditDetailCodeSecondName = leg.DebitDetailCodeSecondName;
        CreditCode = leg.DebitCode;
        CreditWarehouse = leg.DebitWarehouse;
        CreditDetailCodeFirst = leg.DebitDetailCodeFirst;
        CreditDetailCodeSecond = leg.DebitDetailCodeSecond;
        InvoiceNumber = leg.InvoiceNumber;
        InvoiceTaxCode = leg.InvoiceTaxCode;
        InvoiceSerial = leg.InvoiceSerial;
        InvoiceDate = leg.InvoiceDate;

        Quantity = leg.Quantity;
        UnitPrice = leg.UnitPrice;
        AttachVoucher = leg.AttachVoucher;
        BuyDate = leg.OrginalBookDate;
        return this;
    }
}
