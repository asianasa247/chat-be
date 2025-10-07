using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.DocumentEntities;

namespace ManageEmployee.Services.Interfaces.Documents;

public interface IDocumentType2Service
{
    Task<PagingResult<DocumentType2>> GetAll(DocumentType1RequestModel param);

    Task CreateAsync(DocumentType2 param);

    Task UpdateAsync(DocumentType2 param);

    Task DeleteAsync(int id);


    Task<DocumentType2> GetByIdAsyncd(int id);
}
