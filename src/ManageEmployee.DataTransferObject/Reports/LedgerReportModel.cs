namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportModel
{
    public string? Company { get; set; } // Tên công ty
    public int MethodCalcExportPrice { get; set; } = 0;
    public string? Address { get; set; } // Địa chỉ
    public string? TaxId { get; set; } // Mã số thuế
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string? CEOName { get; set; }
    public string? ChiefAccountantName { get; set; }

    public string? CEONote { get; set; }
    public string? ChiefAccountantNote { get; set; }
    public List<LedgerReportViewModel>? Items { get; set; }
    public List<LedgerReportTonSLViewModel>? ItemSLTons { get; set; }
    public List<SoChiTietViewModel>? BookDetails { get; set; }
    public LedgerReportCalculator? LedgerCalculator { get; set; }
    public LedgerReportSumRow? InfoSum { get; set; }
    public IDictionary<long, List<SoChiTietThuChiViewModel>>? SumItem_SCT_ThuChi { get; set; }
    public IDictionary<string, List<SoChiTietThuChiViewModel>>? listAccoutCodeThuChi { get; set; }
}
