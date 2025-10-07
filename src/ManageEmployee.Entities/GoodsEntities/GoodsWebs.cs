using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.CategoryEntities;

namespace ManageEmployee.Entities.GoodsEntities;

public class GoodsWebs
{
    public GoodsWebs()
    {
        CategoryStatusWebPeriodGoods = new HashSet<CategoryStatusWebPeriodGood>();
    }

    public int Id { get; set; }

    [StringLength(36)]
    public string MenuType { get; set; }

    [StringLength(36)]
    public string PriceList { get; set; }

    [StringLength(36)]
    public string GoodsType { get; set; }

    public double SalePrice { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public long Inventory { get; set; }

    [StringLength(500)]
    public string Position { get; set; }

    [StringLength(255)]
    public string Delivery { get; set; }

    public long MinStockLevel { get; set; }
    public long MaxStockLevel { get; set; }
    public int Status { get; set; } = 1;// 0: ngung kinh doanh; 1: dang kinh doanh

    [StringLength(36)]
    public string Account { get; set; }

    [StringLength(255)]
    public string AccountName { get; set; }

    [StringLength(36)]
    public string Warehouse { get; set; }

    [StringLength(255)]
    public string WarehouseName { get; set; }

    [StringLength(255)]
    public string Detail1 { get; set; }

    [StringLength(255)]
    public string DetailName1 { get; set; }

    [StringLength(255)]
    public string Detail2 { get; set; }

    [StringLength(255)]
    public string DetailName2 { get; set; }
    public string Detail1English { get; set; }
    public string DetailName1English { get; set; }
    public string Detail1Korean { get; set; }
    public string DetailName1Korean{ get; set; }

    [StringLength(255)]
    public string Image1 { get; set; }

    [StringLength(255)]
    public string Image2 { get; set; }

    [StringLength(255)]
    public string Image3 { get; set; }

    [StringLength(255)]
    public string Image4 { get; set; }

    [StringLength(255)]
    public string Image5 { get; set; }

    public bool IsDeleted { get; set; } = false;

    [StringLength(500)]
    public string WebGoodNameVietNam { get; set; }

    public double? WebPriceVietNam { get; set; }
    public double? WebDiscountVietNam { get; set; }

    [StringLength(1000)]
    public string TitleVietNam { get; set; }

    public string ContentVietNam { get; set; }

    [StringLength(500)]
    public string WebGoodNameKorea { get; set; }

    public double? WebPriceKorea { get; set; }
    public double? WebDiscountKorea { get; set; }

    [StringLength(255)]
    public string TitleKorea { get; set; }

    public string ContentKorea { get; set; }

    [StringLength(500)]
    public string WebGoodNameEnglish { get; set; }

    public double? WebPriceEnglish { get; set; }
    public double? WebDiscountEnglish { get; set; }

    [StringLength(255)]
    public string TitleEnglish { get; set; }

    public string ContentEnglish { get; set; }
    public bool? isPromotion { get; set; } = false;
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string StockUnit { get; set; }
    public int UserCreated { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public DateTime? DateApplicable { get; set; } = DateTime.Now;
    public double? Net { get; set; }
    public long? TaxRateId { get; set; }
    public double? OpeningStockQuantityNB { get; set; }
    public ICollection<CategoryStatusWebPeriodGood> CategoryStatusWebPeriodGoods { get; set; }
    public bool IsService { get; set; }
    public int? GoodsQuotaId { get; set; }
    public int? NumberItem { get; set; }
    public string? WebUrl { get; set; }
    }
