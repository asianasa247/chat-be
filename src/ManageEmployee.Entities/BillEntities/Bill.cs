using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.BillEntities;

public class Bill
{
    public int Id { get; set; }
    public int DeskId { get; set; }
    public int FloorId { get; set; }

    [StringLength(36)]
    public string? UserCode { get; set; }

    public int CustomerId { get; set; }

    [StringLength(255)]
    public string? CustomerName { get; set; }

    public int QuantityCustomer { get; set; }
    public double Vat { get; set; }
    public double VatRate { get; set; }
    public string? VatCode { get; set; }
    public double TotalAmount { get; set; }
    public double AmountReceivedByCus { get; set; }
    public double AmountSendToCus { get; set; }
    public double AmountRefund { get; set; } = 0;
    public double DiscountPrice { get; set; }

    [StringLength(36)]
    public string? DiscountType { get; set; }

    [StringLength(255)]
    public string? Note { get; set; }

    [StringLength(36)]
    public string? Status { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime UpdatedDate { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;

    [StringLength(36)]
    public string? TypePay { get; set; }// TM: tiền mặt; CN: công nợ

    public long? DisplayOrder { get; set; } = 0;
    public bool IsPrintBill { get; set; } = false;
    public bool IsPriority { get; set; }

    [StringLength(36)]
    public string? InvoiceNumber { get; set; }

    public int UserCreated { get; set; }
    public double? Surcharge { get; set; }
    public string? BillNumber { get; set; }
    public string? Type { get; set; }
    [StringLength(500)]
    public string? DescriptionForLedger { get; set; }
    public DateTime? Date { get; set; } = DateTime.Today;
    public double? PromotionAmount { get; set; }
    public int? OrderProduceProductId { get; set; }
    public int? BranchId { get; set; }
    public string ProjectCode { get; set; }
}