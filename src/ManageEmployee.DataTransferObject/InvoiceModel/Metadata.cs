namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class Metadata
{
    public string? keyTag { get; set; }
    public string? valueType { get; set; }
    public DateTime? dateValue { get; set; }
    public string? stringValue { get; set; }
    public int numberValue { get; set; }
    public string? keyLabel { get; set; }
    public bool isRequired { get; set; }
    public bool isSeller { get; set; }
}
