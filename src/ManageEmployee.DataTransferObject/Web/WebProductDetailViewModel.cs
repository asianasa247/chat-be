using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataTransferObject.Web;

public class WebProductDetailViewModel
{
    public ProductsDetailResponse Good { get; set; }
    public Category? Category { get; set; }
    public List<string>? Images { get; set; }
    public IEnumerable<GoodsDetailModel>? Details { get; set; }
}

public class ProductsByMenuTypeResponse
{
    public int Id { get; set; }
    public string Account { get; set; }
    public string AccountName { get; set; }
    public string Warehouse { get; set; }
    public string WarehouseName { get; set; }
    public string Detail1 { get; set; }
    public string DetailName1 { get; set; }
    public string Detail2 { get; set; }
    public string DetailName2 { get; set; }
    public string Detail1English { get; set; }
    public string DetailName1English { get; set; }
    public string Detail1Korean { get; set; }
    public string DetailName1Korean { get; set; }
    public string Image1 { get; set; }
    public string Image2 { get; set; }
    public string Image3 { get; set; }
    public string Image4 { get; set; }
    public string Image5 { get; set; }
    public string MenuType { get; set; }
    public string PriceList { get; set; }
    public string GoodsType { get; set; }
    public double SalePrice { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public string WebGoodNameVietNam { get; set; }

    public double? WebPriceVietNam { get; set; }
    public double? WebDiscountVietNam { get; set; }
    public string WebGoodNameKorea { get; set; }

    public double? WebPriceKorea { get; set; }
    public double? WebDiscountKorea { get; set; }
    public string WebGoodNameEnglish { get; set; }

    public double? WebPriceEnglish { get; set; }
    public double? WebDiscountEnglish { get; set; }
    public List<ProductCategoryDetailModel> ProductCategoryDetails { get; set; }
}

public class ProductCategoryDetailModel
{
    public int Id { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
}
public class ProductsDetailResponse
{
    public int Id { get; set; }
    public string MenuType { get; set; }
    public string PriceList { get; set; }
    public string GoodsType { get; set; }
    public double SalePrice { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public long Inventory { get; set; }
    public string Account { get; set; }
    public string AccountName { get; set; }
    public string Warehouse { get; set; }
    public string WarehouseName { get; set; }
    public string Detail1 { get; set; }
    public string DetailName1 { get; set; }
    public string Detail2 { get; set; }
    public string DetailName2 { get; set; }
    public string Detail1English { get; set; }
    public string DetailName1English { get; set; }
    public string Detail1Korean { get; set; }
    public string DetailName1Korean { get; set; }
    public string WebGoodNameVietNam { get; set; }
    public double? WebPriceVietNam { get; set; }
    public double? WebDiscountVietNam { get; set; }
    public string TitleVietNam { get; set; }
    public string ContentVietNam { get; set; }
    public string WebGoodNameKorea { get; set; }
    public double? WebPriceKorea { get; set; }
    public double? WebDiscountKorea { get; set; }
    public string TitleKorea { get; set; }
    public string ContentKorea { get; set; }
    public string WebGoodNameEnglish { get; set; }
    public double? WebPriceEnglish { get; set; }
    public double? WebDiscountEnglish { get; set; }
    public string TitleEnglish { get; set; }
    public string ContentEnglish { get; set; }
    public bool? isPromotion { get; set; } = false;
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string StockUnit { get; set; }
    public int UserCreated { get; set; } = 0;
    public bool IsService { get; set; }
    public int? NumberItem { get; set; }
    public string Image1 { get; set; }
    public string Image2 { get; set; }
    public string Image3 { get; set; }
    public string Image4 { get; set; }
    public string Image5 { get; set; }
    public List<ProductCategoryDetailModel> ProductCategoryDetails { get; set; }
}