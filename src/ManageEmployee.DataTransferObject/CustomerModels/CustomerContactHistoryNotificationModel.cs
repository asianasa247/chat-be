namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerContactHistoryNotificationModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Contact { get; set; }
    public DateTime? NextTime { get; set; }
    public string JobName { get; set; }
    public string StatusName { get; set; }
    public string ExchangeContent { get; set; }

}