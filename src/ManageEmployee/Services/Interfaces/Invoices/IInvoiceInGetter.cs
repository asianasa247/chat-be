using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.LedgerEntities.VitaxEntities;

namespace ManageEmployee.Services.Interfaces.Invoices
{
    public interface IInvoiceInGetter
    {
        Task<IEnumerable<LedgerVitaxInvoiceInModel>> GetDetail(string id);
        Task<PagingResult<VintaxInvoiceIn>> GetInvoice(PagingRequestFilterDateModel param);
    }
}
