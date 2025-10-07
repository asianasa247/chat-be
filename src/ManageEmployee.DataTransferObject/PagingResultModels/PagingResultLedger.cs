namespace ManageEmployee.DataTransferObject.PagingResultModels;

public class PagingResultLedger<T> where T : class
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int NextStt { get; set; } = 1;
    public List<T> Data { get; set; } = new List<T>();
}
