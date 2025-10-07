namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class GetHistory
{
    public int Id { get; set; }
    public int TimekeepId { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime TimeOut { get; set; }
    public int Type { get; set; }
    public int TimeKeepSymbolId { get; set; }
    public string TargetName { get; set; }
    public DateTime DateTimeKeep { get; set; }
    public string FullName { get; set; }
    public string Code { get; set; }
    public int TypeOfWork { get; set; }
}