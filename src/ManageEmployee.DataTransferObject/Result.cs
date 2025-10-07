namespace ManageEmployee.DataTransferObject;

public class Result
{
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public static Result Complete()
    {
        return new Result
        {
            IsSuccess = true
        };
    }

    public static Result<TData> Complete<TData>(TData data)
    {
        return new Result<TData>
        {
            IsSuccess = true,
            Data = data
        };
    }
    public static Result Failed()
    {
        return new Result
        {
            IsSuccess = false
        };
    }

    public static Result<TData> Failed<TData>()
    {
        return new Result<TData>
        {
            IsSuccess = false
        };
    }
}

public class Result<TData> : Result
{
    public TData? Data { get; set; }
}
