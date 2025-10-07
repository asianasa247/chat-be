using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class ContractTypePagingRequestModel : PagingRequestModel
{
    public TypeContractEnum? TypeContract { get; set; }
}
