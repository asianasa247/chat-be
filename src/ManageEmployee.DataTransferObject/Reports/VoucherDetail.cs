namespace ManageEmployee.DataTransferObject.Reports;

public class VoucherDetail
{
    public DateTime? BookDate { get; set; }
    public int Month { get; set; }
    public string? OrginalVoucherNumber { get; set; }
    public DateTime OrginalBookDate { get; set; } // Ngày chứng từ
    public string? VoucherDescription { get; set; } // Diễn giải
    public string? DebitCode { get; set; } // Tài khoản Nợ
    public string? CreditCode { get; set; } // Tài khoản Có
    public double Quantity { get; set; } // Số lượng
    public double Amount { get; set; } // Thành tiền
    public string? RefInvoice { get; set; } // Hoá đơn liên quan
    public string? RefVoucher { get; set; } // Phiếu nhập liên quan
}
