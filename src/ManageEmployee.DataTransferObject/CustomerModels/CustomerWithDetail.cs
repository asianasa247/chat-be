namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerWithDetail
{
    public int CustomerId { get; set; }
    public int CustomerOfUserId { get; set; } = -1;
    public string CustomerOfUserFullName { get; set; } = string.Empty;
    public int LastJobId { get; set; } = -1;
    public string LastJobName { get; set; } = string.Empty;
    public string LastJobColor { get; set; }
    public DateTime LastJobStartTime { get; set; }
    public int LastJobStatusId { get; set; } = -1;
    public string LastJobStatusName { get; set; } = string.Empty;
}
