using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.LedgerWarehouseModels;

public class LedgerRequestModel : PagingRequestModel
{
    public string? DocumentType { get; set; }
    public int FilterMonth { get; set; } = 1;
    public int IsInternal { get; set; } = 1;

}
