namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerContactHistoryPagingModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string Contact { get; set; }
    public string Position { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? NextTime { get; set; }
    public string ExchangeContent { get; set; }
    public int? StatusId { get; set; }
    public string StatusName { get; set; }
    public int? JobsId { get; set; }
    public string JobsName { get; set; }
    public string FileLink { get; set; }
    public List<string> FileLinkPaser { get; set; }
    public string StatusColor { get; set; }
    public string JobColor { get; set; }
    public string UserCreated { get; set; }
    public string UserCreatedImage { get; set; }

}
