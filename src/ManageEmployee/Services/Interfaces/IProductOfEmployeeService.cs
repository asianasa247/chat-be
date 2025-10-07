using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageEmployee.Services
{
    public interface IProductOfEmployeeService
    {
        Task<IEnumerable<ProductOfEmployee>> GetAllAsync();
        Task<ProductOfEmployee> GetByIdAsync(int id);
        Task<ProductOfEmployee> AddOrUpdateAsync(ProductOfEmployeeModels product);
        Task<ProductOfEmployee> UpdateAsync(int id,ProductOfEmployeeModels product);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductOfEmployee>> GetProductByEmployeeIdAsync(int id);
        Task<IEnumerable<ProductOfEmployee>> GetProductByCommIdAsync(int id, int employeeId);
        Task<IEnumerable<ProductOfEmployee>> GetProductByGoodIdAsync(int id);
        Task<IEnumerable<ProductOfEmployee>> GetProductByGoodIdAndEmployeeIdAsync(int id, int empId, int commissionId);
    }
}
