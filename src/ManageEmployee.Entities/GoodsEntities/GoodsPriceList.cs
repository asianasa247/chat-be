using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.GoodsEntities;

public class GoodsPriceList
{
    public int Id { get; set; }
    public int GoodsId { get; set; }

    [StringLength(36)]
    public string? PriceList { get; set; }
    public double SalePrice { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
}