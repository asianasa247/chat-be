using ManageEmployee.Entities.OrderEntities;

namespace ManageEmployee.DataTransferObject.Web;

public class OrderHistoryViewModel : Order
{
    public string? StatusName { get; set; }
    public List<OrderHistoryDetailViewModel>? OrderDetails { get; set; }
}