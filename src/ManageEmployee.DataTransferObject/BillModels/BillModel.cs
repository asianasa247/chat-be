namespace ManageEmployee.DataTransferObject.BillModels;

public class BillModel
{
    public int Id { get; set; }
    public int DeskId { get; set; }
    public int FloorId { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? UserCode { get; set; }
    public string? UserType { get; set; }
    public int QuantityCustomer { get; set; }
    public double TotalAmount { get; set; }
    public double AmountReceivedByCus { get; set; }
    public double AmountSendToCus { get; set; }
    public double DiscountPrice { get; set; }
    public string? DiscountType { get; set; }
    public string? Note { get; set; }
    public bool IsPayment { get; set; } = false;
    public List<BillDetailModel>? Products { get; set; }
    public string? TypePay { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrintBill { get; set; } = false;
    public bool IsPriority { get; set; } = false;
    public int UserCreated { get; set; }
    public double? Surcharge { get; set; }
    public string? BillNumber { get; set; }
    public string? Type { get; set; }
    public double Vat { get; set; }
    public string? VatCode { get; set; }
    public double VatRate { get; set; }
    public string? DescriptionForLedger { get; set; }
    public List<BillPromotionModel>? BillPromotions { get; set; }
    public DateTime? Date { get; set; } = DateTime.Today;
    public string? Status { get; set; }
    public string ProjectCode { get; set; }
}
