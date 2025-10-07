using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Entities.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.Email
{
    public class MailTemplateService: IMailTemplateService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public MailTemplateService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<MailTemplate>> GetAll()
        {
            var result = await _context.MailTemplates.OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<MailTemplate>> GetMany(Expression<Func<MailTemplate, bool>> filter = null)
        {
            var query = _context.MailTemplates.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<List<MailTemplate>> GetByListId(List<int> ids)
        {
            var result = await _context.MailTemplates.Where(x => ids.Contains(x.Id)).ToListAsync();
            return result;
        }
        public async Task<MailTemplate> GetById(int id)
        {
            var result = await _context.MailTemplates.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<MailTemplate> Create(MailTemplateModel model)
        {
            var entity = _mapper.Map<MailTemplate>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.MailTemplates.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<MailTemplate> Update(MailTemplateModel model)
        {
            var entity = await _context.MailTemplates.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.TemplateName = model.TemplateName;
            entity.Subject = model.Subject;
            entity.Content = model.Content;
            entity.UpdatedAt = DateTime.Now;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.MailTemplates.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.MailTemplates.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
