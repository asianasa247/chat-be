using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Inventorys;

public interface IInvoiceDeclarationService
{
    Task<IEnumerable<InvoiceDeclarationModel>> GetAll();
    Task<PagingResult<InvoiceDeclarationModel>> GetAll(int pageIndex, int pageSize, string keyword);
    Task<string> Create(InvoiceDeclarationModel request);
    InvoiceDeclarationModel GetById(int id);
    Task<InvoiceDeclarationModel> Update(InvoiceDeclarationModel request);
    Task Delete(int id);
    Task<InvoiceDeclarationModel> UpdateInvoice(int id, int year);
    Task<InvoiceDeclarationModel> ResetInvoice(int id);
}
