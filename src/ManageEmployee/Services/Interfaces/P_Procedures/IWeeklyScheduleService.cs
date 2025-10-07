using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WeeklyScheduleModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures
{
    public interface IWeeklyScheduleService
    {
        Task Accept(int id, int userId);
        Task Create(WeeklyScheduleModel form, int userId);
        Task Delete(int id);
        Task<WeeklyScheduleModel> GetDetail(int id, int userId);
        Task<PagingResult<WeeklySchedulePagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
        Task<string> GetProcedureNumber();
        Task NotAccept(int id, int userId);
        Task Update(WeeklyScheduleModel form, int userId);
    }
}
