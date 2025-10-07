using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.InOut;

// Output Model
public class InOutHistoryViewModel : PagingRequestModel
{
    public int Id { get; set; }
    public string? TargetCode { get; set; }
    public string? TargetName { get; set; }
    public int? UserId { get; set; }
    public string? UserFullName { get; set; }
    public string? UserName { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public int? SymbolId { get; set; }
    public string? SymbolCode { get; set; }
    public string? SymbolName { get; set; }
    public int? CheckInMethod { get; set; } // 0 -> Manual, 1 -> Automatic
    public bool Checked { get; set; } = false;
    public int IsOverTime { get; set; } = 1;  // 1 - BT; 2-TC; 3-P; 4-KP
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
    public int? TargetId { get; set; }
    public double? TotalTime { get; set; }
    public DateTime TimeFrameFrom { get; set; }
    public DateTime TimeFrameTo { get; set; }
}
