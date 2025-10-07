namespace ManageEmployee.DataTransferObject.CarModels;

public class DriverRouterPagingModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public string? Driver { get; set; }
    public string? LicensePlates { get; set; }
    public int PetrolConsumptionId { get; set; }
    public string? RoadRouteName { get; set; }
    public double KmFrom { get; set; }
    public double KmTo { get; set; }
}
