using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.MenuModels;

public class MenuPagingationRequestModel : PagingRequestModel
{
    public string? CodeParent { get; set; }
    public int? userRoleId { get; set; }
    public bool isParent { get; set; }
}
