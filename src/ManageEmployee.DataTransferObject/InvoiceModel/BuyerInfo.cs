namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class BuyerInfo
{
    public string? buyerName { get; set; }
    public string? buyerCode { get; set; }
    public string? buyerLegalName { get; set; }
    public string? buyerTaxCode { get; set; }
    public string? buyerAddressLine { get; set; }
    public string? buyerPhoneNumber { get; set; }
    public string? buyerFaxNumber { get; set; }
    public string? buyerEmail { get; set; }
    public string? buyerBankName { get; set; }
    public string? buyerBankAccount { get; set; }
    public string? buyerDistrictName { get; set; }
    public string? buyerCityName { get; set; }
    public string? buyerCountryCode { get; set; }
    public string? buyerIdType { get; set; }
    public string? buyerIdNo { get; set; }
    public DateTime? buyerBirthDay { get; set; }
    public int buyerNotGetInvoice { get; set; }
    public string? buyerPostalCode { get; set; }
}
