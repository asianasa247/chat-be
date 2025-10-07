using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.Entities.DocumentEntities;

namespace ManageEmployee.Services.Interfaces.Documents;

public interface IDocumentService
{
    IEnumerable<Document> GetAll();

    IEnumerable<DocumentModel.MapDocument> GetAll(string keyword);

    Document GetById(int id);

    int GetLastIdentity();

    Document Create(Document param);

    void Update(Document param);

    void Delete(int id);

    string GetDocumentTypeName(string voucherType);

    IEnumerable<Document> GetAllByUser(string userId);
}
