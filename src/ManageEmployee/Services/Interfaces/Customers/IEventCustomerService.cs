// File: ManageEmployee/Services/Interfaces/Customers/IEventCustomerService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Interfaces.Customers
{
    public interface IEventCustomerService
    {
        Task<IEnumerable<EventCustomer>> GetAllAsync();
        Task<EventCustomer?> GetByIdAsync(int id);
        Task<EventCustomer> CreateAsync(EventCustomer model);
        Task<EventCustomer> UpdateAsync(EventCustomer model);
        Task DeleteAsync(int id);
    }
}
