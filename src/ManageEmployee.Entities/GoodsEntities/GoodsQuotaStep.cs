using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.GoodsEntities;

public class GoodsQuotaStep : BaseEntityCommon
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? UserIds { get; set; }
}
