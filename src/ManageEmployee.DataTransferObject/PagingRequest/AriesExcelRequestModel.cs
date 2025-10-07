using ManageEmployee.DataTransferObject.LedgerWarehouseModels;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class AriesExcelSearchModel : LedgerRequestModel
{
    public int? Month { get; set; }
}
