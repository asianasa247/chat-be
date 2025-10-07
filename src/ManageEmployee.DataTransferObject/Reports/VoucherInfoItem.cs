namespace ManageEmployee.DataTransferObject.Reports;

public class VoucherInforItem
{
    public string? VoucherNumber { get; set; } // Số chứng từ
    public DateTime ClosedDate { get; set; } // Ngày kết sổ (cuối tháng)
    public List<VoucherDetail> Vouchers { get; set; } = new List<VoucherDetail>();

}
