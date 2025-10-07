namespace ManageEmployee.DataTransferObject.InOutModels;

public class ShiftUserBodyRequestModel : DateRangeFilter
{
    public List<int>? UserIds { get; set; }
    public int Symbol { get; set; }
}
