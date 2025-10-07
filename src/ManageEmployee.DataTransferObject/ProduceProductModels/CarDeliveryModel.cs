using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class CarDeliveryModel
{
    public int Id { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public string? LicensePlates { get; set; }
    public string? Driver { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
    public List<FileDetailModel>? FileLinks { get; set; }
}