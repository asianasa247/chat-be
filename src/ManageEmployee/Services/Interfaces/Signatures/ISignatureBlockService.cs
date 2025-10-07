using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SignatureBlockModels;

namespace ManageEmployee.Services.Interfaces.Signatures;

public interface ISignatureBlockService
{
    Task Accept(int id, int userId);

    Task Create(SignatureBlockModel form, int userId);

    Task Delete(int id);

    Task<string> Export(int id);

    Task<SignatureBlockModel> GetById(int id);

    Task<PagingResult<SignatureBlockPagingModel>> GetPaging(ProcedurePagingRequestModel param);

    Task NotAccept(int id, int userId);

    Task Update(SignatureBlockModel form, int userId);
}
