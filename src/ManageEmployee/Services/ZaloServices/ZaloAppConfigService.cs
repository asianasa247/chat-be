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
    public class ZaloAppConfigService: IZaloAppConfigService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public ZaloAppConfigService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<ZaloAppConfig>> GetAll()
        {
            var result = await _context.ZaloAppConfigs.Where(x=>!x.IsDelete).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return result;
        }
        public async Task<IEnumerable<ZaloAppConfig>> GetMany(Expression<Func<ZaloAppConfig, bool>> filter = null)
        {
            var query = _context.ZaloAppConfigs.AsQueryable();
            if (filter != null)
                query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<List<ZaloAppConfig>> GetByListId(List<int> ids)
        {
            var result = await _context.ZaloAppConfigs.Where(x => ids.Contains(x.Id)).ToListAsync();
            return result;
        }
        public async Task<ZaloAppConfig> GetById(int id)
        {
            var result = await _context.ZaloAppConfigs.FirstOrDefaultAsync(x => x.Id == id);
            return result;
        }
        public async Task<ZaloAppConfig> Create(ZaloAppConfigModel model)
        {
            var entity = _mapper.Map<ZaloAppConfig>(model);
            entity.CreatedAt = DateTime.Now;
            await _context.ZaloAppConfigs.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<ZaloAppConfig> Update(ZaloAppConfigModel model)
        {
            var entity = await _context.ZaloAppConfigs.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (entity == null)
                return null;
            entity.AppId = model.AppId;
            entity.AppSecret = model.AppSecret;
            entity.AppName = model.AppName;
            entity.RefreshToken = model.RefreshToken;
            entity.UpdatedAt = DateTime.Now;
            entity.AccessToken = model.AccessToken;
            entity.CallbackUrl = model.CallbackUrl;
            entity.OauthCode = model.OauthCode;
            entity.ExpiredAt = model.ExpiredAt;
            _context.ZaloAppConfigs.Add(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task<bool> Delete(int id)
        {
            var entity = await _context.ZaloAppConfigs.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return false;
            _context.ZaloAppConfigs.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
