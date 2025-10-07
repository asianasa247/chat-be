namespace ManageEmployee.DataTransferObject.CategoryModels;

public class CategorySelectionModel
{

    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Type { get; set; } = 0;// CategoryEnum
    public int TypeView { get; set; } = 0;// CategoryEnum
    public string? CodeName { get; set; }
    public bool IsSizeImage { get; set; }   
    public string TypeMenu { get; set; }
}
