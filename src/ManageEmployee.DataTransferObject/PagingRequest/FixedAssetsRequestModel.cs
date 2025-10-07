using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class FixedAssetsRequestModel : PagingRequestModel
{
    public AssetsType FilterType { get; set; }
    public int FilterMonth { get; set; }
    public int IsInternal { get; set; } = 1;
}