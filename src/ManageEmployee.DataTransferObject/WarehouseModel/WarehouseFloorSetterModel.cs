namespace ManageEmployee.DataTransferObject.WarehouseModel;
public class WarehouseFloorSetterModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Note { get; set; }
    public List<int>? PositionIds { get; set; }
}
