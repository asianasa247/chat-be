namespace ManageEmployee.Models;

public class Response<T>
{
    public Response()
    {
        Success = true;
        Code = 200;
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    public int Code { get; set; }
    public object? Messages { get; set; }
    public T? Data { get; set; }
    public string? RequestId { get; set; }
}
