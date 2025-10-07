using ManageEmployee.Entities.DocumentEntities;

namespace ManageEmployee.Services.Interfaces.Documents;

public interface IDocumentTypeService
{
    IEnumerable<DocumentType> GetAll(int currentPage, int pageSize, string keyword);

    DocumentType Create(DocumentType param);

    void Update(DocumentType param);

    void Delete(int id);

    int Count(string keyword);

    IEnumerable<DocumentType> GetAllByActive();
}
