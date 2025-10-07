using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.Web;

public class ProductSearchModel : PagingRequestModel
{
    public string? CategoryCode { get; set; }
    public SortType SortType { get; set; }
    public double? AmountFrom { get; set; }
    public double? AmountTo { get; set; }
}
