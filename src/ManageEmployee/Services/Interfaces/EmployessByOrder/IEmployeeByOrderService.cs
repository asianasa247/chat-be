using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.EmployessByOrder
{
    public interface IEmployeeByOrderService
    {
        Task<EmployeeByOrder> Add(EmployeeByOrderModels model);
        Task<EmployeeByOrder> Update(int id,EmployeeByOrderModels model);
        Task<bool> Delete(int id);
        Task<EmployeeByOrder> GetById(int id);
        Task<List<EmployeeByOrder>> GetByEmployeeId(int id);
        Task<int> GetByOrderId(int id);

    }
}
