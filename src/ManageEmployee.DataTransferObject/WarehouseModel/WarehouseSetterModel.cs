namespace ManageEmployee.DataTransferObject.WarehouseModel;
public class WarehouseSetterModel
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? ManagerName { get; set; }
    public List<int>? ShelveIds { get; set; }
    public bool IsSyncChartOfAccount { get; set; }
}
