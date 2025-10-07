namespace ManageEmployee.DataTransferObject.SearchModels;
public class SearchViewModel : GoodSearchViewModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? Type { get; set; }
    public string? GoodType { get; set; }
    public string? CategoryTypesSearch { get; set; }
    public string? GoodCode { get; set; }
    public bool isCashier { get; set; } = false;
    public bool isQuantityStock { get; set; } = false;
    public int Status { get; set; } = 1;
    public string? Warehouse { get; set; }
    public bool isManage { get; set; } = false;
    public int MinStockType { get; set; } = 0;
}
