namespace ManageEmployee.DataTransferObject.BillModels;

public class BillDetailModel
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int GoodsId { get; set; }
    public int Quantity { get; set; }
    public double? UnitPrice { get; set; }
    public double? DiscountPrice { get; set; }
    public double? TaxVAT { get; set; } = 0;
    public string? DiscountType { get; set; }
    public string? Note { get; set; }
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string DeliveryCode { get; set; }
    public int? LotNumber { get; set; } // Nullable for cases where no promotion is applied
}
public class BillDetailReport
{
    public DateTime? CreatedDate { get; set; }
    public double Price { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public string? DiscountType { get; set; }

    public double PhaiThu { get; set; }
    public double TienMat { get; set; }
    public double NganHang { get; set; }
    public double CongNo { get; set; }

}

public class BillDetailViewPaging
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int GoodsId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public string? DiscountType { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public double Price { get; set; }
    public string? Note { get; set; }
    public string? ReasonForManager { get; set; }
    public string? GoodsName { get; set; }
    public double? Profit { get; set; }
    public string? GoodsCode { get; set; }
    public string? WareHouseName { get; set; }
    public double? OpenQuantity { get; set; }
    public double? InputQuantity { get; set; }
    public double? OutputQuantity { get; set; }
    public double? CloseQuantity { get; set; }
    public long MinStockLevel { get; set; }
    public long MaxStockLevel { get; set; }
    public string? Image1 { get; set; }
    public string UnitName { get; set; } = "";
    public IList<BillDetailQrInfo>? Suggestions { get; set; }
    public List<int>? GoodWareHouseIds { get; set; }
    public double? PricePay { get; set; }
    public double DiscountPriceBill { get; set; }
    public double SurchargeBill { get; set; }
    public double QuantityRefund { get; set; }
    public string? NoteRefund { get; set; }
    public string? Detail1 { get; set; }
    public string? Detail2 { get; set; }

}
public class BillDetailRefundModel
{
    public int Id { get; set; }
    public int BillId { get; set; }
    public int GoodsId { get; set; }
    public int QuantityRefund { get; set; }
    public string? NoteRefund { get; set; }
}

public class BillPromotionRefundModel
{
    public int Id { get; set; }
    public string? NoteRefund { get; set; }
    public double AmountRefund { get; set; }
    public double QuantityRefund { get; set; }
}
public class BillRefundModel
{
    public List<BillDetailRefundModel>? BillDetails { get; set; }
    public List<BillPromotionModel>? Promotion { get; set; }
}