using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.Reports;

public class PagingationFinalStandardRequestModel : PagingRequestModel
{
    public List<int>? ListIDFinal { get; set; }
    public bool IsAllFinalStandar { get; set; }
    public int Internal { get; set; }

}
