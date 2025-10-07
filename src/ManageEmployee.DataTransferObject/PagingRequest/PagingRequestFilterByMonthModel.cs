namespace ManageEmployee.DataTransferObject.PagingRequest;

public class PagingRequestFilterByMonthModel : PagingRequestModel
{
    public int Month { get; set; }
    public int Year { get; set; }
}
