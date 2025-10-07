namespace ManageEmployee.Entities;

public class TillManager
{
    public int Id { get; set; }
    public DateTime FromAt { get; set; }
    public double FromAmount { get; set; }
    public DateTime? ToAt { get; set; }
    public double ToAmountAuto { get; set; }
    public bool IsDifferentMoney { get; set; }
    public double? AmountDifferent { get; set; }
    public int UserId { get; set; }
    public bool IsFinish { get; set; }
}
