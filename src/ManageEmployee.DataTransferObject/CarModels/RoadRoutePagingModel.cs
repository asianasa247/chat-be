namespace ManageEmployee.DataTransferObject.CarModels;

public class RoadRoutePagingModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public double NumberOfTrips { get; set; }
    public string? RoadRouteDetail { get; set; }
    public string? PoliceCheckPoint { get; set; }
}