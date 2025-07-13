namespace ChatappLC.Application.Common;
public class BaseSearchRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortField { get; set; }
    public bool IsDescending { get; set; } = false;
}
