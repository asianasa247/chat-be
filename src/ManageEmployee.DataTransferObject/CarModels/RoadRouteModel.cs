namespace ManageEmployee.DataTransferObject.CarModels;

public class RoadRouteModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? RoadRouteDetail { get; set; }
    public double NumberOfTrips { get; set; }
    public List<int>? PoliceCheckPointIds { get; set; }
}
