using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.InOutEntities;

public class InOutHistory
{
    [Key]
    public int Id { get; set; }
    public int TargetId { get; set; } = 0;
    public int UserId { get; set; } = 0;
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public int SymbolId { get; set; } = 0;
    public int CheckInMethod { get; set; } = 1; // 0 -> Manual, 1 -> Automatic
    public bool Checked { get; set; } = true;
    public int IsOverTime { get; set; } = 1; // 1 - BT; 2-TC; 3-P; 4-KP

    public DateTime TimeFrameFrom { get; set; }
    public DateTime TimeFrameTo { get; set; }
    public DateTime TargetDate { get; set; }
}
