using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.DocumentEntities;

namespace ManageEmployee.Services.Interfaces.Documents;

public interface IDocumentType1Service
{
    Task<PagingResult<DocumentType1>> GetAll(DocumentType1RequestModel param);

    Task CreateAsync(DocumentType1 param);

    Task UpdateAsync(DocumentType1 param);

    Task DeleteAsync(int id);

    Task<DocumentType1> GetByIdAsync(int id);
}
