namespace ManageEmployee.DataTransferObject.CategoryModels;

public class CategoryStatusWebPeriodGoodShowWebModel : CategoryStatusWebPeriodGoodModel
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Image1 { get; set; }
    public string? Image2 { get; set; }
    public int? ReviewCount { get; set; } = 0;
    public double Price { get; set; } = 0;
    public double SalePrice { get; set; } = 0;
}
