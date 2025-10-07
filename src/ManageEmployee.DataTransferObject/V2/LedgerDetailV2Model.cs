using ManageEmployee.DataTransferObject.ProjectModels;

namespace ManageEmployee.DataTransferObject.V2;

public class LedgerDetailV2Model
{
    public long Id { get; set; }
    public string? Type { get; set; }
    public int Month { get; set; }
    public DateTime? BookDate { get; set; }
    public string? VoucherNumber { get; set; }
    public bool IsVoucher { get; set; } = false;
    public string? OrginalCode { get; set; }
    public string? OrginalVoucherNumber { get; set; }
    public DateTime? OrginalBookDate { get; set; }
    public string? OrginalFullName { get; set; }
    public string? OrginalDescription { get; set; }
    public string? OrginalDescriptionEN { get; set; }
    public string? OrginalCompanyName { get; set; }
    public string? OrginalAddress { get; set; }
    public string? AttachVoucher { get; set; }
    public string? ReferenceVoucherNumber { get; set; }
    public DateTime? ReferenceBookDate { get; set; }
    public string? ReferenceFullName { get; set; }
    public string? ReferenceAddress { get; set; }
    public string? InvoiceCode { get; set; }
    public string? InvoiceAdditionalDeclarationCode { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceTaxCode { get; set; }
    public string? InvoiceAddress { get; set; }
    public string? InvoiceSerial { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? InvoiceName { get; set; }
    public string? InvoiceProductItem { get; set; }
    public int DepreciaMonth { get; set; } = 0;
    public int Order { get; set; } = 0;
    public int Group { get; set; } = 0;
    public DateTime? DepreciaDuration { get; set; }
    public double Quantity { get; set; } = 0;
    public double UnitPrice { get; set; } = 0;
    public double OrginalCurrency { get; set; } = 0;
    public double ExchangeRate { get; set; } = 0;
    public double Amount { get; set; } = 0;
    public bool IsAriseMark { get; set; } = false;
    public int IsInternal { get; set; } = 0;
    public int? BillId { get; set; }
    public int? FixedAsset242Id { get; set; }
    public int? Year { get; set; }
    public double? TotalAmount { get; set; }
    public double? InvoicePercent { get; set; }
    public CommonModel? Credit { get; set; }
    public CommonModel? CreditDetailFirst { get; set; }
    public CommonModel? CreditDetailSecond { get; set; }
    public CommonModel? Debit { get; set; }
    public CommonModel? DebitDetailFirst { get; set; }
    public CommonModel? DebitDetailSecond { get; set; }
    public ProjectModel? Project { get; set; }
}
