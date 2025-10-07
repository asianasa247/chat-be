using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.LedgerEntities;

public class Ledger
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Type { get; set; }
    public int Month { get; set; }
    public DateTime? BookDate { get; set; }
    public string VoucherNumber { get; set; }
    public bool IsVoucher { get; set; } = false;
    public string OrginalCode { get; set; }
    public string OrginalVoucherNumber { get; set; }
    public DateTime? OrginalBookDate { get; set; }
    public string OrginalFullName { get; set; }
    public string OrginalDescription { get; set; }
    public string OrginalDescriptionEN { get; set; }
    public string OrginalCompanyName { get; set; }
    public string OrginalAddress { get; set; }
    public string AttachVoucher { get; set; }
    public string ReferenceVoucherNumber { get; set; }
    public DateTime? ReferenceBookDate { get; set; }
    public string ReferenceFullName { get; set; }
    public string ReferenceAddress { get; set; }
    public string InvoiceCode { get; set; }
    public string InvoiceAdditionalDeclarationCode { get; set; }
    public string InvoiceNumber { get; set; }
    public string InvoiceTaxCode { get; set; }
    public string InvoiceAddress { get; set; }
    public string InvoiceSerial { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string InvoiceName { get; set; }
    public string InvoiceProductItem { get; set; }
    public string DebitCode { get; set; }
    public string DebitWarehouse { get; set; }
    public string DebitDetailCodeFirst { get; set; }
    public string DebitDetailCodeSecond { get; set; }
    public string CreditCode { get; set; }
    public string CreditWarehouse { get; set; }
    public string CreditDetailCodeFirst { get; set; }
    public string CreditDetailCodeSecond { get; set; }
    public string ProjectCode { get; set; }
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
    public bool IsDelete { get; set; } = false;
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public DateTime? DeleteAt { get; set; }
    public long UserCreated { get; set; } = 0;
    public long UserUpdated { get; set; } = 0;
    public long UserDeleted { get; set; } = 0;
    public int IsInternal { get; set; } = 0;
    public string DebitCodeName { get; set; }
    public string DebitDetailCodeFirstName { get; set; }
    public string DebitDetailCodeSecondName { get; set; }
    public string CreditCodeName { get; set; }
    public string CreditDetailCodeFirstName { get; set; }
    public string CreditDetailCodeSecondName { get; set; }
    public string DebitWarehouseName { get; set; }
    public string CreditWarehouseName { get; set; }
    public int? BillId { get; set; }
    public int? Year { get; set; } = DateTime.UtcNow.Year;
    public int? Tab { get; set; }
    public double? PercentImportTax { get; set; }
    public double? PercentTransport { get; set; }
    public double? AmountTransport { get; set; }
    public double? AmountImportWarehouse { get; set; }
    public int? Classification { get; set; }



    public void CheckAndMap(Ledger ledger)
    {

        Type = ledger.Type;
        Month = ledger.Month;
        BookDate = ledger.BookDate;
        VoucherNumber = ledger.VoucherNumber;
        IsVoucher = ledger.IsVoucher;
        OrginalCode = ledger.OrginalCode;
        OrginalVoucherNumber = ledger.OrginalVoucherNumber;
        OrginalBookDate = ledger.OrginalBookDate;
        OrginalFullName = ledger.OrginalFullName;
        OrginalDescription = ledger.OrginalDescription;
        OrginalDescriptionEN = ledger.OrginalDescriptionEN;
        OrginalCompanyName = ledger.OrginalCompanyName;
        OrginalAddress = ledger.OrginalAddress;
        AttachVoucher = ledger.AttachVoucher;
        ReferenceVoucherNumber = ledger.ReferenceVoucherNumber;
        ReferenceBookDate = ledger.ReferenceBookDate;
        ReferenceFullName = ledger.ReferenceFullName;
        ReferenceAddress = ledger.ReferenceAddress;
        InvoiceCode = ledger.InvoiceCode;
        InvoiceAdditionalDeclarationCode = ledger.InvoiceAdditionalDeclarationCode;
        InvoiceNumber = ledger.InvoiceNumber;
        InvoiceTaxCode = ledger.InvoiceTaxCode;
        InvoiceAddress = ledger.InvoiceAddress;
        InvoiceSerial = ledger.InvoiceSerial;
        InvoiceDate = ledger.InvoiceDate;
        InvoiceName = ledger.InvoiceName;
        InvoiceProductItem = ledger.InvoiceProductItem;
        DebitCode = ledger.DebitCode;
        DebitWarehouse = ledger.DebitWarehouse;
        DebitDetailCodeFirst = ledger.DebitDetailCodeFirst;
        DebitDetailCodeSecond = ledger.DebitDetailCodeSecond;
        CreditCode = ledger.CreditCode;
        CreditWarehouse = ledger.CreditWarehouse;
        CreditDetailCodeFirst = ledger.CreditDetailCodeFirst;
        CreditDetailCodeSecond = ledger.CreditDetailCodeSecond;
        ProjectCode = ledger.ProjectCode;
        DepreciaMonth = ledger.DepreciaMonth;
        Order = ledger.Order;
        Group = ledger.Group;
        DepreciaDuration = ledger.DepreciaDuration;
        Quantity = ledger.Quantity;
        UnitPrice = ledger.UnitPrice;
        OrginalCurrency = ledger.OrginalCurrency;
        ExchangeRate = ledger.ExchangeRate;
        Amount = ledger.Amount;
        IsAriseMark = ledger.IsAriseMark;
        IsInternal = ledger.IsInternal;
        DebitCodeName = ledger.DebitCodeName;
        DebitDetailCodeFirstName = ledger.DebitDetailCodeFirstName;
        DebitDetailCodeSecondName = ledger.DebitDetailCodeSecondName;
        CreditCodeName = ledger.CreditCodeName;
        CreditDetailCodeFirstName = ledger.CreditDetailCodeFirstName;
        CreditDetailCodeSecondName = ledger.CreditDetailCodeSecondName;
        DebitWarehouseName = ledger.DebitWarehouseName;
        CreditWarehouseName = ledger.CreditWarehouseName;
        BillId = ledger.BillId;

        AmountTransport = ledger.AmountTransport;
        PercentTransport = ledger.PercentTransport;
        Tab = ledger.Tab;
        PercentImportTax = ledger.PercentImportTax;
        AmountImportWarehouse = ledger.AmountImportWarehouse;
        UserUpdated = ledger.UserUpdated;
        UpdateAt = ledger.UpdateAt;

    }
    public Ledger() { }
}
