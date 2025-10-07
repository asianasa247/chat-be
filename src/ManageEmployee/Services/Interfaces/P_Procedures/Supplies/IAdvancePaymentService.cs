using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IAdvancePaymentService
{
    Task Accept(int id, int userId);

    Task<ProcedureCheckButton> CheckButton(int id, int userId);

    Task Create(AdvancePaymentModel form, int userId);

    Task Delete(int id);
    Task<string> Export(int advancePaymentId);
    Task<AdvancePaymentModel> GetDetail(int id);
    Task<IEnumerable<SelectListModel>> GetList();
    Task<PagingResult<AdvancePaymentPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);

    Task<string> GetProcedureNumber();
    Task NotAccept(int id, int userId);
    Task Update(AdvancePaymentModel form, int userId);
}
