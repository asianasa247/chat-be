using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class GeneralInvoiceInfo
{
    public string? invoiceType { get; set; }
    [MaxLength(20)]
    public string? templateCode { get; set; }
    [MaxLength(7)]
    [RegularExpression("^[a-zA-Z0-9/]*$")]
    public string? invoiceSeries { get; set; }
    public DateTime? invoiceIssuedDate { get; set; }
    [StringLength(3)]
    [RegularExpression("[A-Z]+")]
    public string? currencyCode { get; set; }
    [StringLength(1)]
    public string? adjustmentType { get; set; }
    [StringLength(255)]
    public string? adjustedNote { get; set; }
    [StringLength(1)]
    public string? adjustmentInvoiceType { get; set; }
    [MaxLength(15)]
    [RegularExpression(": ^[a-zA-Z0-9]*$")]
    public string? originalInvoiceId { get; set; }
    public DateTime? originalInvoiceIssueDate { get; set; }
    [MaxLength(255)]
    public string? additionalReferenceDesc { get; set; }
    public DateTime? additionalReferenceDate { get; set; }
    public bool paymentStatus { get; set; }
    public bool cusGetInvoiceRight { get; set; }
    public decimal exchangeRate { get; set; }
    [StringLength(36)]
    public string? transactionUuid { get; set; }
    [StringLength(100)]
    public string? certificateSerial { get; set; }
    public string? originalInvoiceType { get; set; }
    [MaxLength(20)]
    public string? originalTemplateCode { get; set; }
    [MaxLength(100)]
    public string? reservationCode { get; set; }
}
