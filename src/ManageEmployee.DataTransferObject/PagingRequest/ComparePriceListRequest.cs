using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class ComparePriceListRequest : SearchViewModel
{
    public List<string> PriceLists { get; set; } = new List<string>();
    public double? FromDifferentSalePrice { get; set; } = 0;
    public double? ToDifferentSalePrice { get; set; } = 0;
}