namespace ManageEmployee.DataTransferObject.InvoiceModel.ImvoiceRequest;

public class CreateImvoiceRequest
{
    public GeneralInvoiceInfo? generalInvoiceInfo { get; set; }
    public BuyerInfo? buyerInfo { get; set; }
    public SellerInfo? sellerInfo { get; set; }
    public List<Payment>? payments { get; set; }
    public List<ItemInfo>? itemInfo { get; set; }
    public List<Metadata>? metadata { get; set; }
    public List<MeterReading>? meterReading { get; set; }
    public SummarizeInfo? summarizeInfo { get; set; }
    public List<TaxBreakdown>? taxBreakdowns { get; set; }
}
