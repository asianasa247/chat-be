namespace ManageEmployee.DataTransferObject;

public class InvoiceDeclarationModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? TemplateSymbol { get; set; }
    public string? InvoiceSymbol { get; set; }
    public int? TotalInvoice { get; set; }
    public int? FromOpening { get; set; }
    public int? ToOpening { get; set; }
    public int? FromArising { get; set; }
    public int? ToArising { get; set; }
    public int? TotalRelease { get; set; }
    public string? Note { get; set; }

    public int? FromUsed { get; set; }
    public int? ToUsed { get; set; }
    public int? TotalUsed { get; set; }
    public int? UsedNumber { get; set; }
    public int? DeleteNumber { get; set; }
    public string? DeleteNumberItem { get; set; }

    public int? FromClosing { get; set; }
    public int? ToClosing { get; set; }
    public int? ClosingNumber { get; set; }
    public int? Month { get; set; }
}
