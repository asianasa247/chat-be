namespace ManageEmployee.DataTransferObject.LedgerWarehouseModels;

public class LedgerWarehousePaging
{
    public int Id { get; set; }
    public double TotalAmount { get; set; }
    public string? OrginalVoucherNumber { get; set; }
    public DateTime? OrginalBookDate { get; set; }
    public string? OrginalDescription { get; set; }
    public int? CustomerId { get; set; }
    public int? Month { get; set; }
    public int? IsInternal { get; set; }
    public string? Type { get; set; }
}
