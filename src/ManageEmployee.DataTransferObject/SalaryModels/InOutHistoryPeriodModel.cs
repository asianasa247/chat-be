using Common.Helpers;

namespace ManageEmployee.DataTransferObject.SalaryModels;

public class InOutHistoryPeriodModel
{
    public double? NumberWorkdays { get; set; }
    public int UserId { get; set; }
    public DateTime TimeIn { get; set; }
    public DateTime TimeOut { get; set; }
    public int SymbolId { get; set; }
    public DateTime TimeFrameFrom { get; set; }
    public DateTime TimeFrameTo { get; set; }
    public DateTime AcceptedTimeIn => CommonHelper.GetMaxValue(TimeIn, TimeFrameFrom);
    public DateTime AcceptedTimeOut => CommonHelper.GetMinValue(TimeOut, TimeFrameTo);
    public DateTime TargetDate { get; set; }
}
