using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodslDetailModel
{
    public int Id { get; set; }
    public string? MenuType { get; set; }
    public string? PriceList { get; set; }
    public string? GoodsType { get; set; }
    public double SalePrice { get; set; }
    public double Price { get; set; }
    public double DiscountPrice { get; set; }
    public long Inventory { get; set; }
    public string? Position { get; set; }
    public string? Delivery { get; set; }
    public long MinStockLevel { get; set; }
    public long MaxStockLevel { get; set; }
    public int Status { get; set; } = 1;// 0: ngung kinh doanh; 1: dang kinh doanh
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public string? Image1 { get; set; }
    public string? Image2 { get; set; }
    public string? Image3 { get; set; }
    public string? Image4 { get; set; }
    public string? Image5 { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? WebGoodNameVietNam { get; set; }
    public double? WebPriceVietNam { get; set; }
    public double? WebDiscountVietNam { get; set; }
    public string? TitleVietNam { get; set; }
    public string? ContentVietNam { get; set; }
    public string? WebGoodNameKorea { get; set; }
    public double? WebPriceKorea { get; set; }
    public double? WebDiscountKorea { get; set; }
    public string? TitleKorea { get; set; }
    public string? ContentKorea { get; set; }
    public string? WebGoodNameEnglish { get; set; }
    public double? WebPriceEnglish { get; set; }
    public double? WebDiscountEnglish { get; set; }
    public string? TitleEnglish { get; set; }
    public string? ContentEnglish { get; set; }
    public bool? isPromotion { get; set; } = false;
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string? StockUnit { get; set; }
    public int UserCreated { get; set; } = 0;
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public DateTime? DateApplicable { get; set; } = DateTime.Now;
    public double? TaxVAT { get; set; }
    public long? TaxRateId { get; set; }
    public double? TotalAmount { get; set; }
    public string? ListDetailName { get; set; }
    public double? Quantity { get; set; }
    public double? Net { get; set; }
    public string? TaxRateName { get; set; }
    public CommonModel? AccountObj { get; set; }
    public CommonModel? DetailFirstObj { get; set; }
    public CommonModel? DetailSecondObj { get; set; }
    public double? OpeningDebitNB { get; set; } = 0;
    public double? StockUnitPriceNB { get; set; } = 0;
    public double? OpeningStockQuantityNB { get; set; } = 0;
    public bool IsService { get; set; }
}
