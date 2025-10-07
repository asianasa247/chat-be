namespace ManageEmployee.JobSchedules.Interface
{
    public interface IVitaxInvoiceGetterJob
    {
        Task<(bool success, string msg)> GetInvoice(DateTime? fromAt, DateTime? toAt);

        Task RunGetInvoiceJobWrapper();
    }
}
