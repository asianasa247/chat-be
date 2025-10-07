namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class MeterReading
{
    public string? meterName { get; set; }
    public string? previousIndex { get; set; }
    public string? currentIndex { get; set; }
    public string? factor { get; set; }
    public string? amount { get; set; }
}
