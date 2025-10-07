using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.EmployessByOrder;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.DataTransferObject;

namespace ManageEmployee.Services
{
    public class CommissionService : ICommissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CommissionService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách tất cả Commission
        /// </summary>
        public async Task<IEnumerable<Commission>> GetAllAsync()
        {
            return (await _context.Commissions.ToListAsync()) ?? new List<Commission>();
        }

        /// <summary>
        /// Lấy Commission theo ID
        /// </summary>
        public async Task<Commission> GetByIdAsync(int id)
        {
            return await _context.Commissions.FindAsync(id);
        }

        /// <summary>
        /// Thêm hoặc cập nhật Commission
        /// </summary>
        public async Task<Commission> AddOrUpdateAsync(CommissionModels commissionModel)
        {
            var commissionEntity = await _context.Commissions
                .FirstOrDefaultAsync(c => c.Code == commissionModel.Code);

            if (commissionEntity != null)
            {
                // Nếu đã tồn tại, cập nhật dữ liệu
                commissionEntity.Title = commissionModel.Title;
                commissionEntity.isAmount = commissionModel.isAmount;
                _context.Commissions.Update(commissionEntity);
            }
            else
            {
                // Nếu chưa có, tạo mới
                commissionEntity = _mapper.Map<Commission>(commissionModel);
                await _context.Commissions.AddAsync(commissionEntity);
            }

            await _context.SaveChangesAsync();
            return commissionEntity;
        }

        /// <summary>
        /// Cập nhật Commission theo ID
        /// </summary>
        public async Task<Commission> UpdateAsync(int id, CommissionModels commissionModel)
        {
            var existingCommission = await _context.Commissions.FindAsync(id);
            if (existingCommission == null) return null;

            _mapper.Map(commissionModel, existingCommission);
            await _context.SaveChangesAsync();
            return existingCommission;
        }

        /// <summary>
        /// Xóa Commission theo ID
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var commission = await _context.Commissions.FindAsync(id);
            if (commission != null)
            {
                _context.Commissions.Remove(commission);
                await _context.SaveChangesAsync();
            }
        }
    }
}
