using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.EmployessByOrder;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.DataTransferObject;

namespace ManageEmployee.Services
{
    public class EmployeeByOrderService : IEmployeeByOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EmployeeByOrderService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<EmployeeByOrder> Add(EmployeeByOrderModels model)
        {
            var entity = _mapper.Map<EmployeeByOrder>(model);
            await _context.EmployeeByOrders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<EmployeeByOrder> Update(int id, EmployeeByOrderModels model)
        {
            var entity = await _context.EmployeeByOrders.FindAsync(id);
            if (entity == null) return null;

            _mapper.Map(model, entity);
            _context.EmployeeByOrders.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.EmployeeByOrders.FindAsync(id);
            if (entity == null) return false;

            _context.EmployeeByOrders.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<EmployeeByOrder> GetById(int id)
        {
            return await _context.EmployeeByOrders.FindAsync(id);
        }
        public async Task<List<EmployeeByOrder>> GetByEmployeeId(int id)
        {
            return await _context.EmployeeByOrders.Where(e => e.EmployeeId == id).ToListAsync();
        }
        public async Task<int> GetByOrderId(int id)
        {
            return await _context.EmployeeByOrders.Where(e => e.OrderId == id).CountAsync();
        }
    }
}
