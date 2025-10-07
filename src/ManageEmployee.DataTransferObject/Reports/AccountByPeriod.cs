namespace ManageEmployee.DataTransferObject.Reports;

public class AccountByPeriod
{
    /// <summary>
    /// Từ ngày
    /// </summary>
    public DateTime StartDate { get; set; }
    /// <summary>
    /// Đến ngày
    /// </summary>
    public DateTime EndDate { get; set; }

    /// <summary>
    /// Tài khoản đang xét
    /// </summary>
    public string? AccountCode { get; set; }
    /// <summary>
    /// Chi tiết 1
    /// </summary>
    public string? DetailOneCode { get; set; }


    /// <summary>
    /// Chi tiết 2
    /// </summary>
    public string? DetailTwoCode { get; set; }

    /// <summary>
    /// Chi tiết tháng
    /// </summary>

    public List<AccountByPeriodDetail> accountByPeriodDetails { get; set; } = new List<AccountByPeriodDetail>();
}
