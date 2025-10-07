using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.SearchModels;

public class GoodSearchViewModel : PagingRequestModel
{
    public string? Account { get; set; }
    public string? Detail1 { get; set; }
    public string? PriceCode { get; set; }
    public string? MenuType { get; set; }
    public string? Position { get; set; }

}
