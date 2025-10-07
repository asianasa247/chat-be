namespace ManageEmployee.Handlers;

public class CustomActionResult<TData>
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public TData? SuccessData { get; set; }
    public string? Message { get; set; }
}