namespace ManageEmployee.DataTransferObject.NumberOfMealModels;

public class NumberOfMealModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string TimeType { get; set; }
    public double QuantityFromInOut { get; set; }
    public double QuantityAdd { get; set; }
}
