using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Email
{
    public class MailSendService : IMailSendService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MailSendService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<MailSend>> GetAll()
        {
            var result = await _context.MailSends.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<MailSend>> GetMany(Expression<Func<MailSend, bool>> filter = null)
        {
            var query = _context.MailSends.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<MailSend> GetById(int id)
        {
            var result = await _context.MailSends.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<MailSend> GetByEmail(string email)
        {
            var result = await _context.MailSends.FirstOrDefaultAsync(x => x.Email == email);
            return result;
        }
        public async Task<MailSend> Create(MailSendModel model)
        {
            var entity = _mapper.Map<MailSend>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.MailSends.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<MailSend> Update(MailSendModel model)
        {
            var entity = await _context.MailSends.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.SmtpId = model.SmtpId;
            entity.ImapId = model.ImapId;
            entity.Password = model.Password;
            entity.UpdatedAt = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.MailSends.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.MailSends.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
