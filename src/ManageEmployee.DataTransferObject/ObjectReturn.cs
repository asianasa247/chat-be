namespace ManageEmployee.DataTransferObject;

public class ObjectReturn
{
    public int status { get; set; }
    public string? code { get; set; }
    public string? message { get; set; }
    public dynamic? data { get; set; }
}
