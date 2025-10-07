namespace ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;

public class GoodsQuotaStepModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public IEnumerable<int>? UserIds { get; set; }
}
public class GoodsQuotaStepListModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
}
