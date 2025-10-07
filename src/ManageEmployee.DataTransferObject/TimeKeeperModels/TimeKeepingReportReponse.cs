using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingReportReponse
{
    public List<TimeKeepingReportV2Model> Data { get; set; }
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public object? DataTotal { get; set; }
    public int NextStt { get; set; }
    public TimeKeepingReportDateModel Date { get; set; }
}
