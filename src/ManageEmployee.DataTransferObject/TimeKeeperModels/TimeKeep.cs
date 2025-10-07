using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeep : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TargetId { get; set; }
    public int TypeOfWork { get; set; }
    public int Type { get; set; }
    public int TimeKeepSymbolId { get; set; }
    public DateTime DateTimeKeep { get; set; }
    public int IsOverTime { get; set; } = 1;  // 1 - BT; 2-TC; 3-P; 4-KP

}
