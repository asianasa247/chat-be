using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Email
{
    public class MailLogService:IMailLogService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MailLogService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<MailLog>> GetAll()
        {
            var result = await _context.MailLogs.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<MailLog>> GetMany(Expression<Func<MailLog, bool>> filter = null)
        {
            var query = _context.MailLogs.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<MailLog> GetById(int id)
        {
            var result = await _context.MailLogs.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<MailLog> Create(MailLogModel model)
        {
            var entity = _mapper.Map<MailLog>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.MailLogs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<MailLog> Update(MailLogModel model)
        {
            var entity = await _context.MailLogs.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.Content = model.Content;
            entity.UpdatedAt = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.MailLogs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.MailLogs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
