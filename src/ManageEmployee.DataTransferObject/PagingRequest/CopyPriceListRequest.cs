namespace ManageEmployee.DataTransferObject.PagingRequest;

public class CopyPriceListRequest : GoodRequestModel
{
    public string? PriceListFrom { get; set; }
    public string? PriceListTo { get; set; }
}
