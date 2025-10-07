using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.ExpenditurePlans;

public interface IExpenditurePlanService
{
    Task Accept(int id, int userId);
    Task Create(ExpenditurePlanSetterModel form, int userId);
    Task Delete(int id);
    Task<ExpenditurePlanModel> GetDetail(int id);
    Task<IEnumerable<ExpenditurePlanGetListModel>> GetList();
    Task<PagingResult<ExpenditurePlanPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
    Task NotAccept(int id, int userId);
    Task Update(ExpenditurePlanSetterModel form, int userId);
    Task UpdateExpenditure(ExpenditurePlanModel form, int userId);
}
