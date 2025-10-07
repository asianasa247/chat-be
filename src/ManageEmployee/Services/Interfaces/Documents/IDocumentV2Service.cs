using ManageEmployee.DataTransferObject.DocumentModels;

namespace ManageEmployee.Services.Interfaces.Documents;

public interface IDocumentV2Service
{
    Task<DocumentV2Model> GetByIdAsync(int id, int year);
    Task<List<DocumentV2Model>> GetByUserAsync(string userId, int year);
}
