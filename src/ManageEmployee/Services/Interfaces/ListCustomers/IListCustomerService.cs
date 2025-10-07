namespace ManageEmployee.Services.Interfaces.ListCustomers
{
    public interface IListCustomerService
    {
        Task<byte[]> GetCustomerContactHistories(int customerId);
        
    }
}
