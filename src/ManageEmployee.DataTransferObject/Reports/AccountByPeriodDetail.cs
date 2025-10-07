namespace ManageEmployee.DataTransferObject.Reports;

/// <summary>
/// Thông tin tài khoản theo khoảng thời gian
/// </summary>
public class AccountByPeriodDetail
{
    /// <summary>
    /// Từ ngày
    /// </summary>
    public DateTime From { get; set; }
    /// <summary>
    /// Đến ngày
    /// </summary>
    public DateTime To { get; set; }

    /// <summary>
    /// Nợ đầu kỳ
    /// </summary>
    public double OpeningDebit { get; set; }
    /// <summary>
    /// Có đầu kỳ
    /// </summary>
    public double OpeningCredit { get; set; }
    /// <summary>
    /// Phát sinh nợ trong kỳ
    /// </summary>
    public double ArisingDebit { get; set; }
    /// <summary>
    /// Phát sinh có trong kỳ
    /// </summary>
    public double ArisingCredit { get; set; }
    /// <summary>
    /// Luỹ kế nợ đầu kỳ
    /// </summary>
    public double OpenAccumulatingDebit { get; set; }
    /// <summary>
    /// Luỹ kế có đầu kỳ
    /// </summary>
    public double OpenAccumulatingCredit { get; set; }
    /// <summary>
    /// Luỹ kế nợ phát sinh (đến cuối kỳ)
    /// </summary>
    public double AcumulatingDebit { get; set; }
    /// <summary>
    /// Luỹ kế có phát sinh (đến cuối kỳ)
    /// </summary>
    public double AcumulatingCredit { get; set; }
    /// <summary>
    /// Nợ cuối kỳ
    /// </summary>
    public double ClosingDebit { get; set; }
    /// <summary>s
    /// Có cuối kỳ
    /// </summary>
    public double ClosingCredit { get; set; }
    public int Month { get; set; }

    public List<LedgerDetail> LedgerDetails { get; set; } = new List<LedgerDetail>();
}
