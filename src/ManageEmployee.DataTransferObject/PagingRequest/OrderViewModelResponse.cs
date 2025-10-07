using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class OrderViewModelResponse
{
    public int Id { get; set; }
    /// <summary>
    /// Danh sách chi tiết đơn hàng
    /// </summary>
    public List<OrderDetailViewModel>? OrderDetails { get; set; }
    /// <summary>
    /// Tổng giá trị đơn hàng
    /// </summary>
    public double TotalPrice { get; set; }
    /// <summary>
    /// Tổng giảm giá
    /// </summary>
    public double TotalPriceDiscount { get; set; }
    /// <summary>
    /// Tổng số tiền phải thanh toán
    /// </summary>
    public double TotalPricePaid { get; set; }
    /// <summary>
    /// Trạng thái đơn hàng
    /// </summary>
    public OrderStatus Status { get; set; }
    /// <summary>
    /// Trạng thái đơn hàng
    /// </summary>
    public string? StatusName { get; set; }
    /// <summary>
    /// Id khách hàng
    /// </summary>
    public int CustomerId { get; set; }
    /// <summary>
    /// Tên KH
    /// </summary>
    public string? CustomerName { get; set; }
    /// <summary>
    /// Địa chỉ nhận hàng
    /// </summary>
    public string? ShippingAddress { get; set; }
    /// <summary>
    /// Sđt
    /// </summary>
    public string? Tell { get; set; }
    /// <summary>
    /// Tên người nhận
    /// </summary>
    public string? FullName { get; set; }
    public string? Notify { get; set; }
    public DateTime CreateAt { get; set; }

}
