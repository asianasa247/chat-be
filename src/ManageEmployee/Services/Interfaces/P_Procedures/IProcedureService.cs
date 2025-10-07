using ManageEmployee.DataTransferObject.MenuModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureService
{
    Task<IEnumerable<P_Procedure>> GetAll();

    Task<P_ProcedureViewModel> GetById(int id);

    Task Create(P_ProcedureViewModel param);

    Task Update(P_ProcedureViewModel param);

    Task Delete(int id);

    Task<PagingResult<P_Procedure>> GetPaging(PagingRequestModel param);
    Task<List<MenuCheckRole>> GetProcedureNeedAccept(int userId);
    Task ResetProcedureCountAsync(int userId, string procedureCode);
}
