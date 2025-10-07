namespace ManageEmployee.DataTransferObject.V2;

public class LedgerV2Model
{
    public long Id { get; set; }
    public string? Type { get; set; }
    public int Month { get; set; }
    public DateTime? BookDate { get; set; }
    public string? VoucherNumber { get; set; }
    public string? OrginalCode { get; set; }
    public string? OrginalVoucherNumber { get; set; }
    public DateTime? OrginalBookDate { get; set; }
    public string? OrginalDescription { get; set; }
    public string? OrginalDescriptionEN { get; set; }
    public string? InvoiceCode { get; set; }
    public string? InvoiceAdditionalDeclarationCode { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceTaxCode { get; set; }
    public string? InvoiceAddress { get; set; }
    public string? InvoiceSerial { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? InvoiceName { get; set; }
    public string? InvoiceProductItem { get; set; }
    public string? DebitCode { get; set; }
    public string? DebitWarehouse { get; set; }
    public string? DebitDetailCodeFirst { get; set; }
    public string? DebitDetailCodeSecond { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditWarehouse { get; set; }
    public string? CreditDetailCodeFirst { get; set; }
    public string? CreditDetailCodeSecond { get; set; }
    public int Order { get; set; } = 0;
    public double Quantity { get; set; } = 0;
    public double UnitPrice { get; set; } = 0;
    public double ExchangeRate { get; set; } = 0;
    public double Amount { get; set; } = 0;
    public int IsInternal { get; set; } = 0;
    public string? DebitCodeName { get; set; }
    public string? DebitDetailCodeFirstName { get; set; }
    public string? DebitDetailCodeSecondName { get; set; }
    public string? CreditCodeName { get; set; }
    public string? CreditDetailCodeFirstName { get; set; }
    public string? CreditDetailCodeSecondName { get; set; }
    public double? TotalAmount { get; set; }
    public double OrginalCurrency { get; set; } = 0;
    public int? BillId { get; set; }
}
