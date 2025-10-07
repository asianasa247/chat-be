namespace ManageEmployee.DataTransferObject.CategoryModels;

public class CategoryStatusWebPeriodModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public int UserId { get; set; }
    public DateTime CreateAt { get; set; }
    public List<CategoryStatusWebPeriodGoodShowWebModel>? Items { get; set; }

}
