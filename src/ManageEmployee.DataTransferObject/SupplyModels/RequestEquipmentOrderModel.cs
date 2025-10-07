using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SupplyModels;

public class RequestEquipmentOrderModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public double Amount { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public List<FileDetailModel>? Files { get; set; }
    public int? CustomerId { get; set; }
    public List<RequestEquipmentOrderDetailModel>? Items { get; set; }
}
