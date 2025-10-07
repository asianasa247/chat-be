using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class WebNewPagingRequestModel : PagingRequestModel
{
    public LanguageEnum? Type { get; set; }
    public int? CategoryId { get; set; }
}

public class WebNewHeadLinePagingRequestModel : PagingRequestModel
{
    public LanguageEnum? Type { get; set; }
    public string CategoryCode { get; set; }
}