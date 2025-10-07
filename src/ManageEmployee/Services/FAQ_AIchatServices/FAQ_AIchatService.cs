using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FAQ_AIchat;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.FAQ_AI_Chatbot;
using ManageEmployee.Services.Interfaces.FAQ_AIchat;
using ManageEmployee.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.FAQ_AIchatServices
{
    public class FAQ_AIchatService : IFAQ_AIchatService
    {
        private readonly ApplicationDbContext _context;

        public FAQ_AIchatService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FAQ_AIchatModel>> GetAll()
        {
            return await _context.FAQ_AIchats
                .AsNoTracking()
                .Select(x => new FAQ_AIchatModel
                {
                    UserId = x.UserId,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,
                    Department = x.Department,
                    FirstTopic = x.FirstTopic
                })
                .ToListAsync();
        }

        public async Task<PagingResult<FAQ_AIchatModel>> GetAll(FAQ_AIchatModel param)
        {
            var query = _context.FAQ_AIchats.AsNoTracking();

            if (!string.IsNullOrEmpty(param.UserId))
                query = query.Where(x => x.UserId == param.UserId);
            if (!string.IsNullOrEmpty(param.Department))
                query = query.Where(x => x.Department == param.Department);
            if (!string.IsNullOrEmpty(param.FirstTopic))
                query = query.Where(x => x.FirstTopic == param.FirstTopic);

            var totalItems = await query.CountAsync();
            var data = await query
                .OrderByDescending(x => x.CreateAt)
                .Select(x => new FAQ_AIchatModel
                {
                    UserId = x.UserId,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,
                    Department = x.Department,
                    FirstTopic = x.FirstTopic
                })
                .ToListAsync();

            return new PagingResult<FAQ_AIchatModel>
            {
                CurrentPage = 1,
                PageSize = totalItems,
                TotalItems = totalItems,
                Data = data
            };
        }

        public async Task<FAQ_AIchat> Create(FAQ_AIchatModel request)
        {
            var entity = new FAQ_AIchat
            {
                UserId = request.UserId,
                CreateAt = request.CreateAt,
                UpdateAt = request.UpdateAt,
                Department = request.Department,
                FirstTopic = request.FirstTopic
            };
            _context.FAQ_AIchats.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<FAQ_AIchatModel> GetById(int id)
        {
            var entity = await _context.FAQ_AIchats.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;
            return new FAQ_AIchatModel
            {
                UserId = entity.UserId,
                CreateAt = entity.CreateAt,
                UpdateAt = entity.UpdateAt,
                Department = entity.Department,
                FirstTopic = entity.FirstTopic
            };
        }

        public async Task<FAQ_AIchat> Update(FAQ_AIchatModel request)
        {
            var entity = await _context.FAQ_AIchats.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Department == request.Department && x.FirstTopic == request.FirstTopic);
            if (entity == null)
                throw new Exception("FAQ_AIchat not found");

            entity.UpdateAt = request.UpdateAt;
            entity.Department = request.Department;
            entity.FirstTopic = request.FirstTopic;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var chatDetails = await _context.FAQ_AIchatDetails
                .Where(p => p.FAQ_AIchatId == id)
                .ToListAsync();

            if (chatDetails.Any())
            {
                _context.FAQ_AIchatDetails.RemoveRange(chatDetails);
            }

            var entity = await _context.FAQ_AIchats.FindAsync(id);
            if (entity == null)
                return false;

            _context.FAQ_AIchats.Remove(entity);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FAQ_AIchat>> GetAll(int currentPage, int pageSize)
        {
            return await _context.FAQ_AIchats.OrderByDescending(x => x.CreateAt)
                .Skip(pageSize * (currentPage - 1)).Take(pageSize).ToListAsync();
        }

        public async Task<int> TotalChat()
        {
            return await _context.FAQ_AIchats
                .AsNoTracking()
                .CountAsync();
        }
    }
}
