using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.CarModels;

public class DriverRouterDetailModel
{
    public int Id { get; set; }
    public int DriverRouterId { get; set; }
    public DateTime Date { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Location { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public int PoliceCheckPointId { get; set; }
    public List<FileDetailModel>? Files { get; set; }
}