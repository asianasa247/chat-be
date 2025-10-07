namespace ManageEmployee.DataTransferObject.InOutModels;

public class ShiftUserFilterModel : DateRangeFilter
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
