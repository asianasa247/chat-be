namespace ManageEmployee.DataTransferObject.CarModels;

public class DriverRouterModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public int PetrolConsumptionId { get; set; }
    public List<DriverRouterDetailModel>? Items { get; set; }
    public double? AdvancePaymentAmount {  get; set; }
    public decimal? CostAmount {  get; set; }
    public double? FuelAmount {  get; set; }
    public double? RemainingAmount {  get; set; }
}
