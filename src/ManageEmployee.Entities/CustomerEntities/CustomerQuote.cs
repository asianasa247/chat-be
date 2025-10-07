namespace ManageEmployee.Entities.CustomerEntities;

public class CustomerQuote
{
    public long Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreateDate { get; set; }
    public string? Note { get; set; }
}
