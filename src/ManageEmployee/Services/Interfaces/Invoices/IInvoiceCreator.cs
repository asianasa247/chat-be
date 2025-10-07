namespace ManageEmployee.Services.Interfaces.Invoices;

public interface IInvoiceCreator
{
    Task PerformAsync(int billId);
}
