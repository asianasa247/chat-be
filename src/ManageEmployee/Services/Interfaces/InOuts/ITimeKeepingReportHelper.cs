using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.TimeKeeperModels;

namespace ManageEmployee.Services.Interfaces.InOuts
{
    public interface ITimeKeepingReportHelper
    {
        Task<TimeKeepingReportReponse> TimeKeepingReportV2(TimeKeepViewModel param, int userId, string roles);
    }
}