using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ConfigurationEntities;

public class ConfigDiscount : BaseEntityCommon
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public int PositionDetailId { get; set; }
    public double DiscountReceivedMonth { get; set; }
    public double DiscountReceivedYear { get; set; }
    public double PercentAdvanceDiscountMonth { get; set; }
    public string? Note { get; set; }
}
