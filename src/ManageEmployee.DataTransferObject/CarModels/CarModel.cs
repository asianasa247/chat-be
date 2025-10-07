using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.CarModels;

public class CarModel
{
    public int Id { get; set; }
    public string? LicensePlates { get; set; }
    public string? Note { get; set; }
    public string? Content { get; set; }
    public double MileageAllowance { get; set; }
    public double FuelAmount { get; set; }
    public List<FileDetailModel>? Files { get; set; }
}
