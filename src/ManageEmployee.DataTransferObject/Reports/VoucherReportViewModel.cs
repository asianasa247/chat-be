namespace ManageEmployee.DataTransferObject.Reports;

public class VoucherReportViewModel
{
    public string? Company { get; set; } // Tên công ty
    public string? Address { get; set; } // Địa chỉ
    public string? TaxId { get; set; } // Mã số thuế
    public string? Type { get; set; } // Loại chứng từ
    public string? TypeName { get; set; } // Tên loại chứng từ
    public List<VoucherReportItem> Items { get; set; } = new List<VoucherReportItem>(); // Trích yếu
    public string? ChiefAccountantName { get; set; } // Tên kế toán trưởng
    public string? NoteChiefAccountantName { get; set; } // Tên kế toán trưởng
    public string? CEOOfName { get; set; } // Tên giám đốc
    public string? CEOOfNote { get; set; } // Tên giám đốc
    public string? VoteMaker { get; set; }
    public int MethodCalcExportPrice { get; set; } = 0;
}
