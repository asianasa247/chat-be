using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodsPagingResult
{
    public int pageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public IEnumerable<Goods> Goods { get; set; } = new List<Goods>();
}
