
namespace ManageEmployee.DataTransferObject.BaseResponseModels;
public class BaseResponseModel : BaseResponseCommonModel
{
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public object? DataTotal { get; set; }
    public int NextStt { get; set; }
}