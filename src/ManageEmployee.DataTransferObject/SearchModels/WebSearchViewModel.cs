
namespace ManageEmployee.DataTransferObject.SearchModels;
public class WebSearchViewModel : SearchViewModel, ISearchWithGoodsIds
{
    public List<int> GoodsIds { get; set; }
}
