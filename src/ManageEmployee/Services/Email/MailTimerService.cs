using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Email
{
    public class MailTimerService:IMailTimerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MailTimerService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<MailTimer>> GetAll()
        {
            var result = await _context.MailTimers.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<MailTimer>> GetMany(Expression<Func<MailTimer, bool>> filter = null)
        {
            var query = _context.MailTimers.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<MailTimer> GetById(int id)
        {
            var result = await _context.MailTimers.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<MailTimer> Create(MailTimerModel model)
        {
            var entity = _mapper.Map<MailTimer>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.MailTimers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<MailTimer> Update(MailTimerModel model)
        {
            var entity = await _context.MailTimers.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.EmailSend = model.EmailSend;
            entity.Begin = model.Begin;
            entity.UpdatedAt = DateTime.Now;
            entity.TypeRepeat = model.TypeRepeat;
            entity.RepeatInterval = model.RepeatInterval;
            entity.MailTo = model.MailTo;
            entity.MailCc = model.MailCc;
            entity.MailBcc = model.MailBcc;
            entity.Content = model.Content;
            entity.IsRunning = model.IsRunning;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.MailTimers.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.MailTimers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
