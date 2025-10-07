namespace ManageEmployee.DataTransferObject.BillModels;

public class BillTrackingForCashierModel
{
    public long Id { get; set; }
    public int BillId { get; set; }
    public string? UserCode { get; set; }
    public string? CustomerName { get; set; }
    public string? TranType { get; set; }
    public string? Note { get; set; }
    public string? Status { get; set; }
    public bool IsRead { get; set; }
    public bool IsImportant { get; set; }
    public int? UserIdReceived { get; set; }
    public DateTime CreatedDate { get; set; }
    public long DisplayOrder { get; set; }
    public int Prioritize { get; set; }
    public double TotalAmount { get; set; }
    public string? DeskName { get; set; }
    public string? BillNumber { get; set; }
    public string? PermisionShowButtons { get; set; }
}
