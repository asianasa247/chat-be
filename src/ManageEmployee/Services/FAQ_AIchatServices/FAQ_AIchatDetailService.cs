using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FAQ_AIchat;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.FAQ_AI_Chatbot;
using ManageEmployee.Services.Interfaces.FAQ_AIchat;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.FAQ_AIchatServices
{
    public class FAQ_AIchatDetailService : IFAQ_AIchatDetailService
    {
        private readonly ApplicationDbContext _context;

        public FAQ_AIchatDetailService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FAQ_AIchatDetailModel>> GetAll()
        {
            return await _context.FAQ_AIchatDetails
                .AsNoTracking()
                .Select(x => new FAQ_AIchatDetailModel
                {
                    FAQ_AIchatId = x.FAQ_AIchatId,
                    Question = x.Question,
                    Answer = x.Answer,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,
                    Topic = x.Topic
                })
                .ToListAsync();
        }

        public async Task<PagingResult<FAQ_AIchatDetailModel>> GetAll(FAQ_AIchatDetailModel param)
        {
            var query = _context.FAQ_AIchatDetails.AsNoTracking();

            if (param.FAQ_AIchatId != 0)
                query = query.Where(x => x.FAQ_AIchatId == param.FAQ_AIchatId);
            if (!string.IsNullOrEmpty(param.Question))
                query = query.Where(x => x.Question.Contains(param.Question));
            if (!string.IsNullOrEmpty(param.Topic))
                query = query.Where(x => x.Topic == param.Topic);

            var totalItems = await query.CountAsync();
            var data = await query
                .OrderByDescending(x => x.CreateAt)
                .Select(x => new FAQ_AIchatDetailModel
                {
                    FAQ_AIchatId = x.FAQ_AIchatId,
                    Question = x.Question,
                    Answer = x.Answer,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,
                    Topic = x.Topic
                })
                .ToListAsync();

            return new PagingResult<FAQ_AIchatDetailModel>
            {
                CurrentPage = 1,
                PageSize = totalItems,
                TotalItems = totalItems,
                Data = data
            };
        }

        public async Task<FAQ_AIchatDetail> Create(FAQ_AIchatDetailModel request)
        {
            var entity = new FAQ_AIchatDetail
            {
                FAQ_AIchatId = request.FAQ_AIchatId,
                Question = request.Question,
                Answer = request.Answer,
                CreateAt = request.CreateAt,
                UpdateAt = request.UpdateAt,
                Topic = request.Topic
            };
            _context.FAQ_AIchatDetails.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<FAQ_AIchatDetailModel> GetById(int id)
        {
            var entity = await _context.FAQ_AIchatDetails.FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return null;
            return new FAQ_AIchatDetailModel
            {
                FAQ_AIchatId = entity.FAQ_AIchatId,
                Question = entity.Question,
                Answer = entity.Answer,
                CreateAt = entity.CreateAt,
                UpdateAt = entity.UpdateAt,
                Topic = entity.Topic
            };
        }

        public async Task<FAQ_AIchatDetail> Update(FAQ_AIchatDetailModel request)
        {
            var entity = await _context.FAQ_AIchatDetails.FirstOrDefaultAsync(x => x.FAQ_AIchatId == request.FAQ_AIchatId && x.Question == request.Question && x.Topic == request.Topic);
            if (entity == null)
                throw new Exception("FAQ_AIchatDetail not found");

            entity.Answer = request.Answer;
            entity.UpdateAt = request.UpdateAt;
            entity.Topic = request.Topic;
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Delete(int id)
        {
            var entity = await _context.FAQ_AIchatDetails.FindAsync(id);
            if (entity == null)
                return false;
            _context.FAQ_AIchatDetails.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FAQ_AIchatDetailModel>> GetAllbyChatId(int FaqAiChatId)
        {
            return await _context.FAQ_AIchatDetails
                .AsNoTracking()
                .Where(x => x.FAQ_AIchatId == FaqAiChatId)
                .Select(x => new FAQ_AIchatDetailModel
                {
                    FAQ_AIchatId = x.FAQ_AIchatId,
                    Question = x.Question,
                    Answer = x.Answer,
                    CreateAt = x.CreateAt,
                    UpdateAt = x.UpdateAt,
                    Topic = x.Topic
                })
                .ToListAsync();
        }

        public async Task<List<FAQ_AIchatDetail>> GetAllByChatID(int currentPage, int pageSize, int chatId)
        {
            return await _context.FAQ_AIchatDetails
                .Where(x => x.FAQ_AIchatId == chatId)
                .OrderByDescending(x => x.CreateAt)
                .Skip(pageSize * (currentPage - 1))
                .Take(pageSize).ToListAsync();
        }
    }
}
