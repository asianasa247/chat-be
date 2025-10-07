namespace ManageEmployee.DataTransferObject.WarehouseModel;

public class ReportForBranchModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public double Quantity { get; set; }
    public string? GoodsDetails { get; set; }
    public string? Type { get; set; }
    public List<List<ReportForBranchModel>>? Items { get; set; }
}
