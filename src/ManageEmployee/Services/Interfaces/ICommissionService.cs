using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;


namespace ManageEmployee.Services
{
    public interface ICommissionService
    {
        /// <summary>
        /// Lấy danh sách tất cả Commission
        /// </summary>
        Task<IEnumerable<Commission>> GetAllAsync();

        /// <summary>
        /// Lấy thông tin Commission theo ID
        /// </summary>
        Task<Commission?> GetByIdAsync(int id);

        /// <summary>
        /// Thêm mới hoặc cập nhật Commission theo Code
        /// </summary>
        Task<Commission> AddOrUpdateAsync(CommissionModels model);

        /// <summary>
        /// Cập nhật Commission theo ID
        /// </summary>
        Task<Commission?> UpdateAsync(int id, CommissionModels model);

        /// <summary>
        /// Xóa Commission theo ID
        /// </summary>
        Task DeleteAsync(int id);
    }
}
