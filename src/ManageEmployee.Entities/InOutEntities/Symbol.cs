namespace ManageEmployee.Entities.InOutEntities;

public class Symbol
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime TimeOut { get; set; }
    public double TimeTotal { get; set; }
    public string? Note { get; set; }
    public bool Status { get; set; }
    public int CheckInTimeThreshold { get; set; }
    public int CheckOutTimeThreshold { get; set; }
    public DateTime ShiftStartAt { get; set; }
    public DateTime ShiftEndAt { get; set; }
}