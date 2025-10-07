using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CustomerEntities;

public class CustomerContactHistory: BaseEntityCommon
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? Contact { get; set; }
    public string? Position { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public DateTime? NextTime { get; set; }
    public string? ExchangeContent { get; set; }
    public int? StatusId { get; set; }
    public int? JobsId { get; set; }
    public string? FileLink { get; set; }
}