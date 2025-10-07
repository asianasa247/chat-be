namespace ManageEmployee.Entities.CarEntities;

public class DriverRouterDetail
{
    public int Id { get; set; }
    public int DriverRouterId { get; set; }
    public string? Status { get; set; }
    public DateTime Date { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Location { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public string? FileStr { get; set; }
    public int PoliceCheckPointId { get; set; }
}
