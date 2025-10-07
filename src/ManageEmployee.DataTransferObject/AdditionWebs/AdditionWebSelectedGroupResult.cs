
namespace ManageEmployee.DataTransferObject.AdditionWebs;

public class AdditionWebSelectedGroupResult
{
    public int AdditionWebId { get; set; }
    public string? UrlWeb { get; set; }
    public AdditionWebCompanyResult CompanyInfo { get; set; }
    public IEnumerable<AdditionWebGoodsResult> Goods { get; set; }
}
