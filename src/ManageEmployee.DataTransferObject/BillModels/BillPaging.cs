namespace ManageEmployee.DataTransferObject.BillModels;

public class BillPaging
{
    public int Id { get; set; }
    public int DeskId { get; set; }
    public int FloorId { get; set; }
    public string? UserCode { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerTaxCode { get; set; }
    public string? CustomerAddress { get; set; }
    public int QuantityCustomer { get; set; }
    public double TotalAmount { get; set; }
    public double TotalAmountCN { get; set; }
    public double AmountReceivedByCus { get; set; }
    public double AmountSendToCus { get; set; }
    public double DiscountPrice { get; set; }
    public string? DiscountType { get; set; }
    public string? Note { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
    public string? TypePay { get; set; }// TM: tiền mặt; CN: công nợ
    public string? DisplayOrder { get; set; }
    public double TotalCustomer { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? UserName { get; set; }
}
