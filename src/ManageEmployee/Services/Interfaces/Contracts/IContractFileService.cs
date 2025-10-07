using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ContractEntities;

namespace ManageEmployee.Services.Interfaces.Contracts;

public interface IContractFileService
{
    Task<List<ContractFileListModel>> GetAll(int contractTypeId);
    Task<PagingResult<ContractFile>> GetAll(PagingRequestModel param);
    Task Create(ContractFileModel param);
    Task Update(ContractFileModel param);
    Task Delete(int id);
    Task<ContractFileModel> GetById(int id);
}
