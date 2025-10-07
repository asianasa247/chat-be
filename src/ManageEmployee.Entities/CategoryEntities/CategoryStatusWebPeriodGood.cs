using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.Entities.CategoryEntities;

public class CategoryStatusWebPeriodGood
{
    public int Id { get; set; }
    public int CategoryStatusWebPeriodId { get; set; }
    public int GoodId { get; set; }

    public virtual CategoryStatusWebPeriod CategoryStatusWebPeriod { get; set; }
    public virtual Goods Goods { get; set; }
}
