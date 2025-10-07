namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportCalculator_ForeignAmount
{
    public double TyGia { get; set; }
    public bool PhatSinhNo { get; set; } = false;
    public double ExchangeRate { get; set; }
    public double OriginalCurrency { get; set; }

    /// <summary>
    /// 1; Dư đầu kỳ
    /// 2: Cộng phát sinh
    /// 3: Lũy kế phát sinh
    /// 4: Dư cuối tháng
    /// </summary>
    public int RowType { get; set; }
    public long MonthYear { get; set; }
}
