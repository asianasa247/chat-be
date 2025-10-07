using System.Collections.Generic;
using System.Threading.Tasks;
using ManageEmployee.DataTransferObject.EventModels;

namespace ManageEmployee.Services.Interfaces.Customers
{
    public interface IEventRegistrationService
    {
        Task<EventRegistrationResponse> RegisterAsync(EventRegistrationRequest dto);

        Task<(IEnumerable<EventRegistrationListItem> Items, int Total)>
            GetAllAsync(int pageIndex, int pageSize);

        Task<(IEnumerable<EventRegistrationListItem> Items, int Total)>
            GetByEventAsync(int eventId, int pageIndex, int pageSize);

        Task<EventRegistrationDetail?> GetByCustomerAsync(int customerId);

        // NEW: check phone (no side-effect)
        Task<(bool Exists, int? CustomerId)> CheckPhoneAsync(string phone);
    }
}
