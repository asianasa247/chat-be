// File: ManageEmployee/Services/CustomerServices/EventCustomerService.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Errors; // để dùng ErrorException
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.EntityFrameworkCore;
using System;
using ManageEmployee.Helpers;

namespace ManageEmployee.Services.CustomerServices
{
    public class EventCustomerService : IEventCustomerService
    {
        private readonly ApplicationDbContext _context;

        public EventCustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventCustomer>> GetAllAsync()
        {
            return await _context.EventCustomers
                .AsNoTracking()
                .OrderByDescending(x => x.StartDate)
                .ThenByDescending(x => x.Id)
                .ToListAsync();
        }

        public async Task<EventCustomer?> GetByIdAsync(int id)
        {
            return await _context.EventCustomers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<EventCustomer> CreateAsync(EventCustomer model)
        {
            // Validate ngày
            if (model.EndDate.HasValue && model.EndDate.Value < model.StartDate)
                throw new ErrorException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            await _context.EventCustomers.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<EventCustomer> UpdateAsync(EventCustomer model)
        {
            var old = await _context.EventCustomers.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (old == null)
                throw new ErrorException("Không tìm thấy sự kiện.");

            if (model.EndDate.HasValue && model.EndDate.Value < model.StartDate)
                throw new ErrorException("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");

            old.StartDate = model.StartDate;
            old.EndDate = model.EndDate;
            old.EventCode = model.EventCode;
            old.EventName = model.EventName;
            old.Supervisor = model.Supervisor;
            old.Note = model.Note;

            _context.EventCustomers.Update(old);
            await _context.SaveChangesAsync();
            return old;
        }

        public async Task DeleteAsync(int id)
        {
            var old = await _context.EventCustomers.FirstOrDefaultAsync(x => x.Id == id);
            if (old == null) return;

            _context.EventCustomers.Remove(old);
            await _context.SaveChangesAsync();
        }
    }
}
