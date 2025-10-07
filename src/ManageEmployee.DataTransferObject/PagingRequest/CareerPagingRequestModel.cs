using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class CareerPagingRequestModel : PagingRequestModel
{
    public LanguageEnum? Type { get; set; }
}
