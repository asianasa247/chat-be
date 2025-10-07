namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerModelPaging
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
    public int? UserCreated { get; set; }
    public string DebitCode { get; set; }
    public string DebitDetailCodeFirst { get; set; }
    public string DebitDetailCodeSecond { get; set; }
    public CustomerWithDetail Details { get; set; }
    public double? TotalAmountPay { get; set; }
    public double? TotalAmountCN { get; set; }
    public int? TotalTask { get; set; }
    public int? CustomerQuoteCount { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? CreatedAt { get; set; }
}
