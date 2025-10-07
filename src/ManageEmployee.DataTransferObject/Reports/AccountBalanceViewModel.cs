namespace ManageEmployee.DataTransferObject.Reports;

public class AccountBalanceViewModel
{
    public string? Company { get; set; } // Tên công ty
    public string? Address { get; set; } // Địa chỉ
    public string? TaxId { get; set; } // Mã số thuế
    public List<AccountBalanceItemViewModel>? Items { get; set; }
    public string? PreparedBy { get; set; }
    public string ManagerName { get; internal set; } = string.Empty;
    public string ChiefAccountantName { get; internal set; } = string.Empty;
    public string NoteOfCEO { get; internal set; } = string.Empty;
    public string NoteOfChiefAccountant { get; internal set; } = string.Empty;
}
