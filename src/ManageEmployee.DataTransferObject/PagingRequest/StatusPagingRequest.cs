using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class StatusPagingRequest : PagingRequestModel
{
    public StatusTypeEnum? Type { get; set; }
}
