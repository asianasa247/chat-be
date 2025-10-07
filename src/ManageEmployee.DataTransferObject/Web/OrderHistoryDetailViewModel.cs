using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.OrderEntities;

namespace ManageEmployee.DataTransferObject.Web;

public class OrderHistoryDetailViewModel : OrderDetail
{
    public Goods? Good { get; set; }
}
