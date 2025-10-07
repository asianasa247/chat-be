using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject;

public class DeskFLoorPagingationRequestModel : PagingRequestModel
{
    public int? FloorId { get; set; }
    public bool? IsFloor { get; set; }
}
