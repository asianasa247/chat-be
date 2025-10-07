namespace ManageEmployee.DataTransferObject.PagingRequest;

public class UpdatePriceListRequest : GoodRequestModel
{
    public int PriceFrom { get; set; }//0:gia ban; 1: gia von ; 2: giam gia; 3: thue
    public int PriceTo { get; set; }//0:gia ban; 1: gia von ; 2: giam gia; 3: thue
    public string? PriceList { get; set; }
}
