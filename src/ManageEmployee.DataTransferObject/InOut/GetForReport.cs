namespace ManageEmployee.DataTransferObject.InOut;

public class GetForReport
{
    public int Id { get; set; }
    public int UserId { get; set; }
    //public int TimekeepId { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public int CheckInMethod { get; set; } // 0 -> Manual, 1 -> Automatic
    public int TimeKeepSymbolId { get; set; }
    public string? TimeKeepSymbolCode { get; set; }
    public double TimeKeepSymbolTimeTotal { get; set; }
    public string? TimeKeepSymbolName { get; set; }
    public DateTime DateTimeKeep { get; set; }
    public int IsOverTime { get; set; }  // 1 - BT; 2-TC; 3-P; 4-KP
    public int TargetId { get; set; }
    public string? TargetCode { get; set; }
    public string? TargetName { get; set; }
}
