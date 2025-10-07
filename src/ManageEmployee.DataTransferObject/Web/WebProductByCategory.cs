using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataTransferObject.Web;

public class WebProductByCategory
{
    public string? CategoryName { get; set; }
    public string? CategoryCode { get; set; }
    public List<UserTaskFileModel>? CategoryImages { get; set; }
    public List<Goods>? Products { get; set; }
}
