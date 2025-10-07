namespace ManageEmployee.DataTransferObject.PagingResultModels;

public class PagingResult<T> where T : class
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public List<T> Data { get; set; } = new List<T>();
}
