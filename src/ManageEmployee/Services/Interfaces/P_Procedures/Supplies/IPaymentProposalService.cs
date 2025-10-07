using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Entities.SupplyEntities;

namespace ManageEmployee.Services.Interfaces.P_Procedures.Supplies;

public interface IPaymentProposalService
{
    Task Accept(int id, int userId);

    Task<ProcedureCheckButton> CheckButton(int id, int userId);

    Task Create(PaymentProposalModel form, int userId, int? tableId = null, string tableName = null);

    Task Delete(int id);

    Task<string> Export(int paymentProposalId, bool shouldExport = false);

    Task<PaymentProposalModel> GetDetail(int id);
    Task<List<PaymentProposal>> GetListForTableName(string tableName, IEnumerable<int> tableIds);
    Task<PagingResult<PaymentProposalPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);

    Task<string> GetProcedureNumber();

    Task NotAccept(int id, int userId);
    Task ResetTableId(PaymentProposal data, int tableId);
    Task SetForOtherTable(PaymentProposal data, int userId, int? tableId, string tableName);
    Task Update(PaymentProposalModel form, int userId);
}
