namespace ManageEmployee.Services.Interfaces.Invoices;

public interface IInvoiceAuthorize
{
    Task<string> PerformAsync();
}
