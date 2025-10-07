namespace ManageEmployee.Entities;

public class TimeKeepingInOutLogging
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Data { get; set; }
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public string? Traces { get; set; }
}