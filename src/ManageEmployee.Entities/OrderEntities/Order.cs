using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.OrderEntities;

public class Order : BaseEntity
{
    public int Id { get; set; }
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
    /// Id khách hàng
    /// </summary>
    public int CustomerId { get; set; }

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

    public bool IsPayment { get; set; }
    public DateTime PaymentAt { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    [StringLength(255)]
    public string? Broker { get; set; }
    [StringLength(36)]
    public string? Identifier { get; set; }
    public int? BillId { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime Date { get; set; }
    public string? Promotion { get; set; }

}