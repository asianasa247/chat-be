using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.OrderEntities;

public class OrderDetail : BaseEntity
{
    public int Id { get; set; }
    /// <summary>
    /// Id don hang
    /// </summary>
    public int OrderId { get; set; }
    /// <summary>
    /// Id sản phẩm
    /// </summary>
    public int GoodId { get; set; }
    /// <summary>
    /// Id chi tiết sản phẩm
    /// </summary>
    public int? GoodDetailId { get; set; }
    /// <summary>
    /// Số lượng
    /// </summary>
    public int Quantity { get; set; }
    /// <summary>
    /// Gía sản phẩm
    /// </summary>
    public double Price { get; set; }
    public double? TaxVAT { get; set; }
    public int? AdultQuantity { get; set; }
    public int? ChildrenQuantity { get; set; }
}
