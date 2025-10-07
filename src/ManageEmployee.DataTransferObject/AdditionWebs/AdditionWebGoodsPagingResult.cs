namespace ManageEmployee.DataTransferObject.AdditionWebs;

public class AdditionWebGoodsPagingResult
{
    public int pageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }

    public IEnumerable<AdditionWebGoodsResult> Goods { get; set; } = new List<AdditionWebGoodsResult>();
}
