using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Smtp;
using ManageEmployee.DataTransferObject.Zalo;
using ManageEmployee.Entities.Email;
using ManageEmployee.Entities.ZaloEntities;
using ManageEmployee.Services.Interfaces.Zalo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ManageEmployee.Services.ZaloServices
{
    public class ZaloUserService : IZaloUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ZaloUserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<ZaloUser>> GetAll()
        {
            var result = await _context.ZaloUsers.Where(x => !x.IsDelete).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<ZaloUser>> GetMany(Expression<Func<ZaloUser, bool>> filter = null)
        {
            var query = _context.ZaloUsers.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<List<ZaloUser>> GetByListId(List<int> ids)
        {
            var result = await _context.ZaloUsers.Where(x => ids.Contains(x.Id)).ToListAsync();
            return result;
        }
        public async Task<ZaloUser> GetById(int id)
        {
            var result = await _context.ZaloUsers.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<ZaloUser> Create(ZaloUserModel model)
        {
            var entity = _mapper.Map<ZaloUser>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.ZaloUsers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<ZaloUser> Update(ZaloUserModel model)
        {
            var entity = await _context.ZaloUsers.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.AppId = model.AppId;
            entity.Avatars = model.Avatars;
            entity.Avatar = model.Avatar;
            entity.DisplayName = model.DisplayName;
            entity.IsSensitive = model.IsSensitive;
            entity.SharedInfo = model.SharedInfo;
            entity.TagsAndNotesInfo = model.TagsAndNotesInfo;
            entity.UserAlias = model.UserAlias;
            entity.UserExternalId = model.UserExternalId;
            entity.UserIsFollower = model.UserIsFollower;
            entity.UserLastInteractionDate = model.UserLastInteractionDate;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.ZaloUsers.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.ZaloUsers.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
