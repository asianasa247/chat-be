namespace ManageEmployee.DataTransferObject.Web;

public class CommonWebResponse
{
    public bool State { get; set; }
    public int? Code { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}