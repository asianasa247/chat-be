using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Smtp;
using ManageEmployee.Entities.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Email
{
    public class SmtpService: ISmtpService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public SmtpService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<Smtp>> GetAll()
        {
            var result = await _context.Smtps.OrderByDescending(x=>x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<Smtp>>GetMany(Expression<Func<Smtp, bool>> filter = null)
        {
            var query = _context.Smtps.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<List<Smtp>> GetByListId(List<int> ids)
        {
            var result = await _context.Smtps.Where(x => ids.Contains(x.Id)).ToListAsync();
            return result;
        }
        public async Task<Smtp> GetById(int id)
        {
            var result = await _context.Smtps.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<Smtp> Create(SmtpModel model)
        {
            var entity = _mapper.Map<Smtp>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.Smtps.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<Smtp> Update(SmtpModel model)
        {
            var entity = await _context.Smtps.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.SmtpServer = model.SmtpServer;
            entity.Name = model.Name;
            entity.Port = model.Port;
            entity.Ssl = model.Ssl;
            entity.UseDefaultCredentials = model.UseDefaultCredentials;
            entity.EnableSsl = model.EnableSsl;
            entity.EnableTls = model.EnableTls;
            entity.RequiresAuthentication = model.RequiresAuthentication;
            entity.UpdatedAt = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.Smtps.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.Smtps.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
