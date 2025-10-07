using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductPagingModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public double Quantity { get; set; }
    public double QuantityDelivered { get; set; }
    public double QuantityInProgress { get; set; }
    public double TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public OrderStatus Status { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public bool IsSpecialOrder { get; set; }
    public bool ShoulDelete { get; set; }
    public bool ShoulNotAccept { get; set; }
    public int UserCreated { get; set; }
    public string? UserCreatedCode { get; set; }
    public string? UserCreatedName { get; set; }
    public string? Code { get; set; }
    public bool IsFinished { get; set; }
}
