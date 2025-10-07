namespace ManageEmployee.DataTransferObject
{
    public class GoodsSelectionItemDto
    {
        public int Id { get; set; }

        public string MenuType { get; set; }
        public string PriceList { get; set; }
        public string GoodsType { get; set; }

        public double SalePrice { get; set; }
        public double Price { get; set; }
        public double DiscountPrice { get; set; }
        public long Inventory { get; set; }

        public string Position { get; set; }
        public string Delivery { get; set; }

        public long MinStockLevel { get; set; }
        public long MaxStockLevel { get; set; }
        public int Status { get; set; }

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

        public bool IsDeleted { get; set; }

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

        public bool? isPromotion { get; set; }
        public DateTime? DateManufacture { get; set; }
        public DateTime? DateExpiration { get; set; }
        public string StockUnit { get; set; }
        public int UserCreated { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? DateApplicable { get; set; }
        public double? Net { get; set; }
        public long? TaxRateId { get; set; }
        public double? OpeningStockQuantityNB { get; set; }
        public bool IsService { get; set; }
        public int? GoodsQuotaId { get; set; }
        public int? NumberItem { get; set; }

        public string WebUrl { get; set; }

        public int IdUrl { get; set; }
    }

    public record class GoodsSelectionDetailsPagingResult
    {
        public int TotalItems { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public IReadOnlyList<GoodsSelectionItemDto> Items { get; set; } = new List<GoodsSelectionItemDto>();

        public IReadOnlyList<GoodsSelectionItemDto> Data => Items;
    }
}
