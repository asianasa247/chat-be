namespace ManageEmployee.DataTransferObject.Reports;

public class TransactionListViewModel
{
    public string? Company { get; set; } // Tên công ty
    public string? Address { get; set; } // Địa chỉ
    public string? TaxId { get; set; } // Mã số thuế
    public string? Type { get; set; } // Loại chứng từ
    public string? TypeName { get; set; } // Tên loại chứng từ
    public List<VoucherInforItem> VoucherInfors { get; set; } = new List<VoucherInforItem>();
    public string? ChiefAccountantName { get; set; } // Tên kế toán trưởng
    public string? ChiefAccountantNote { get; set; } // Tên kế toán trưởng
    public string? VoteMaker { get; set; }
    public bool isFillName { get; set; }
}
