namespace ManageEmployee.DataTransferObject.CarModels;

public class CarGetterPagingModel
{
    public int Id { get; set; }
    public string? LicensePlates { get; set; }
    public string? Content { get; set; }
    public double MileageAllowance { get; set; }
    public double FuelAmount { get; set; }
    public string? Note { get; set; }
}
