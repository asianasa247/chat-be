namespace ManageEmployee.DataTransferObject.PrintViewModelModels;

public class PagePrintViewModel
{
    public int Height { get; set; }
    public int Width { get; set; }
    public PrintViewModel? QrCode { get; set; }
    public PrintViewModel? Barcode { get; set; }
}
