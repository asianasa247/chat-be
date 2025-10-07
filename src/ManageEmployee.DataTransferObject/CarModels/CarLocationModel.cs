namespace ManageEmployee.DataTransferObject.CarModels;


public class CarLocationModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public string? ProcedureNumber { get; set; }
    public List<CarLocationDetailModel>? Items { get; set; }
}