using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.OrderEntities;

namespace ManageEmployee.Services.Interfaces.Orders
{
    public interface IOrderSuccessfulService
    {
        IEnumerable<OrderSuccessful> GetAll();
        Task<PagingResult<OrderSuccessful>> GetAll(int currentPage, int pagesize, string keyword);
        OrderSuccessful GetByID(int id);
        OrderSuccessful Create(OrderSuccessful parma);
        void Update(OrderSuccessful parma);
        void Delete(int id);
    }
}
