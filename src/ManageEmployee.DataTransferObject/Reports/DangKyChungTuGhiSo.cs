namespace ManageEmployee.DataTransferObject.Reports;

public class DangKyChungTuGhiSo
{
    public string? Type { get; set; }
    public string? VoucherNumber { get; set; }
    public DateTime OrginalBookDate { get; set; }
    public double Amount { get; set; }
    public string? OrginalDescription { get; set; }
    public string? MonthOfYear { get; set; }
}
