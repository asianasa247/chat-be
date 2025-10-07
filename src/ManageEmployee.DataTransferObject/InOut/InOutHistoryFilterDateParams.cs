namespace ManageEmployee.DataTransferObject.InOut;

public class InOutHistoryFilterDateParams : InOutHistoryFilterParams
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public bool CheckCurrentUser { get; set; }
}
