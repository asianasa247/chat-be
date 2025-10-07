namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_InventoryViewModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    public int? P_ProcedureStatusId { get; set; }
    public string? P_ProcedureStatusName { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? UserCreated { get; set; }
    public string? UserCreatedName { get; set; }
    public int? UserUpdated { get; set; } = 0;
    public bool isFinish { get; set; }
    public List<P_Inventory_Item_ViewModel>? Items { get; set; }
}
