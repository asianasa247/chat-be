using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerContactHistoryDto
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
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
    public List<IFormFile> FileLink { get; set; }
}
