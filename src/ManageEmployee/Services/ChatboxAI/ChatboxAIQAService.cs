using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChatboxAI;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChatboxAI
{
    public class ChatboxAIQAService : IChatboxAIQAService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChatboxAIQAService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Paged
        public async Task<PagingResult<ChatboxAIQA>> GetAll(
            int pageIndex, int pageSize, string keyword, int? topicId = null)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageIndex < 1) pageIndex = 1;

            IQueryable<ChatboxAIQA> query = _context.ChatboxAIQAs.AsNoTracking();

            if (topicId.HasValue)
                query = query.Where(x => x.TopicId == topicId.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x =>
                    x.Question.Contains(keyword) ||
                    x.Answer.Contains(keyword) ||
                    x.Topic.TopicName.Contains(keyword) ||
                    x.Topic.TopicCode.Contains(keyword));

            var result = new PagingResult<ChatboxAIQA>
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = await query.CountAsync(),
                Data = await query
                    .Include(x => x.Topic)                        // Include CHỈ ở bước cuối
                    .OrderBy(x => x.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };

            return result;
        }

        // Not paged (list)
        public IEnumerable<ChatboxAIQA> GetAll()
        {
            return _context.ChatboxAIQAs
                .AsNoTracking()
                .Include(x => x.Topic)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public async Task<string> Create(ChatboxAIQA request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                // Topic phải tồn tại
                var topic = await _context.ChatboxAITopics.FindAsync(request.TopicId);
                if (topic == null)
                    return "TopicNotFound";

                // Chống trùng câu hỏi trong cùng Topic
                var exists = await _context.ChatboxAIQAs
                    .AnyAsync(x => x.TopicId == request.TopicId && x.Question.ToLower() == request.Question.ToLower());
                if (exists)
                    return "QA_QuestionAlreadyExist";

                var entity = _mapper.Map<ChatboxAIQA>(request);
                _context.ChatboxAIQAs.Add(entity);
                await _context.SaveChangesAsync();

                await _context.Database.CommitTransactionAsync();
                return string.Empty;
            }
            catch
            {
                await _context.Database.RollbackTransactionAsync();
                throw;
            }
        }

        public ChatboxAIQA GetById(int id)
        {
            return _context.ChatboxAIQAs.Find(id)!;
        }

        public async Task<string> Update(ChatboxAIQA request)
        {
            try
            {
                request.Topic = null; // tránh EF insert Topic

                var entity = await _context.ChatboxAIQAs
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null) return "QA_NotFound";

                // Topic phải tồn tại
                var topicExists = await _context.ChatboxAITopics
                    .AnyAsync(t => t.Id == request.TopicId);
                if (!topicExists) return "TopicNotFound";

                // Không trùng câu hỏi (trừ bản ghi hiện tại)
                var dup = await _context.ChatboxAIQAs.AnyAsync(x =>
                    x.Id != request.Id &&
                    x.TopicId == request.TopicId &&
                    x.Question.ToLower() == request.Question.ToLower());
                if (dup) return "QA_QuestionAlreadyExist";

                // Patch
                entity.TopicId = request.TopicId;
                entity.Question = request.Question;
                entity.Answer = request.Answer;

                // Đánh dấu đã đổi (đề phòng EF không bắt được)
                _context.Entry(entity).Property(x => x.TopicId).IsModified = true;
                _context.Entry(entity).Property(x => x.Question).IsModified = true;
                _context.Entry(entity).Property(x => x.Answer).IsModified = true;

                var affected = await _context.SaveChangesAsync();
                return affected > 0 ? string.Empty : "NoChangeDetected";
            }
            catch (DbUpdateException ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }



        public string Delete(int id)
        {
            var entity = _context.ChatboxAIQAs.Find(id);
            if (entity != null)
            {
                _context.ChatboxAIQAs.Remove(entity);
                _context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
