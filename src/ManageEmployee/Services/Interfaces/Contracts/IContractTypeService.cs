using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ContractEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Services.Interfaces.Contracts;

public interface IContractTypeService
{
    Task<List<ContractTypeListModel>> GetAll(TypeContractEnum type);

    Task<PagingResult<ContractType>> GetAll(ContractTypePagingRequestModel param);

    Task Create(ContractTypeModel param);

    Task Update(ContractTypeModel param);

    Task Delete(int id);

    Task<ContractType> GetById(int id);
}
