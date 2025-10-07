namespace ManageEmployee.Entities.ProduceProductEntities;

public class CarDelivery
{
    public int Id { get; set; }
    public int TableId { get; set; }
    public string? TableName { get; set; }
    public string? LicensePlates { get; set; }
    public string? Driver { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
    public string? FileLink { get; set; }
}