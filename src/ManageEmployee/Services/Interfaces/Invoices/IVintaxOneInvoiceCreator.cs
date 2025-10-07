
namespace ManageEmployee.Services.Interfaces.Invoices
{
    public interface IVintaxOneInvoiceCreator
    {
        Task CreateInvoice(int billId);
    }
}
