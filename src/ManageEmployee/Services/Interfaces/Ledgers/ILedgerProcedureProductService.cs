using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerProcedureProductService
{
    Task Accept(int id, int userId, int year);

    Task Create(List<Ledger> ledger, int userId, string type, int year);

    Task Delete(int id);

    Task<LedgerProduceProductModel> GetDetail(int id);

    Task<PagingResult<LedgerProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId, string type);

    Task Update(LedgerProduceProductModel form, int userId);

    Task<IEnumerable<FileDetailModel>> UploadFile(List<IFormFile> files, int id);
    Task NotAccept(int id, int userId);
}
