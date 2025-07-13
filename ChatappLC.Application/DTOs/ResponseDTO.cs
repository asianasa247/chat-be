namespace ChatappLC.Application.DTOs;

public class ResponseDTO<T>
{
    public bool Flag { get; set; }
    public string Message { get; set; }
    public T? Data { get; set; }

    public ResponseDTO(bool flag, string message, T? data = default)
    {
        Flag = flag;
        Message = message;
        Data = data;
    }
}
