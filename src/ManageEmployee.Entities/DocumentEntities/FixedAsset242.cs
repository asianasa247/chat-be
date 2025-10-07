using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.DocumentEntities;

public partial class FixedAsset242
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    public double? HistoricalCost { get; set; }
    [StringLength(36)]
    public string? VoucherNumber { get; set; }
    public DateTime? UsedDate { get; set; }
    public DateTime? EndOfDepreciation { get; set; }
    public DateTime? LiquidationDate { get; set; }
    public int? TotalMonth { get; set; }
    public double? DepreciationOfOneDay { get; set; }
    public double? AccruedExpense { get; set; }
    public int? TotalDayDepreciationOfThisPeriod { get; set; }
    public double? DepreciationOfThisPeriod { get; set; }
    public double? CarryingAmountOfLiquidationAsset { get; set; }
    public double? CarryingAmount { get; set; }
    [StringLength(255)]
    public string? DepartmentManager { get; set; }
    [StringLength(255)]
    public string? UserManager { get; set; }
    public string? Type { get; set; }
    [StringLength(500)]
    public string? DebitCodeName { get; set; }
    [StringLength(500)]
    public string? DebitDetailCodeFirstName { get; set; }
    [StringLength(500)]
    public string? DebitDetailCodeSecondName { get; set; }
    [StringLength(500)]
    public string? CreditCodeName { get; set; }
    [StringLength(500)]
    public string? CreditDetailCodeFirstName { get; set; }
    [StringLength(500)]
    public string? CreditDetailCodeSecondName { get; set; }
    [StringLength(500)]
    public string? DebitCode { get; set; }
    [StringLength(50)]
    public string? DebitWarehouse { get; set; }
    [StringLength(50)]
    public string? DebitDetailCodeFirst { get; set; }
    [StringLength(50)]
    public string? DebitDetailCodeSecond { get; set; }
    [StringLength(50)]
    public string? CreditCode { get; set; }
    [StringLength(50)]
    public string? CreditWarehouse { get; set; }
    [StringLength(50)]
    public string? CreditDetailCodeFirst { get; set; }
    [StringLength(50)]
    public string? CreditDetailCodeSecond { get; set; }
    [StringLength(50)]
    public string? InvoiceNumber { get; set; }
    [StringLength(50)]
    public string? InvoiceTaxCode { get; set; }
    [StringLength(50)]
    public string? InvoiceSerial { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public short Use { get; set; }

    public int? DepartmentId { get; set; }
    public int? UserId { get; set; }


    public DateTime? BuyDate { get; set; }
    public double? Quantity { get; set; }
    public double? UnitPrice { get; set; }
    [StringLength(255)]
    public string? AttachVoucher { get; set; }
}
