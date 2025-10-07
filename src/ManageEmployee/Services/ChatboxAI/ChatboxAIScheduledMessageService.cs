using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChatboxAI;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChatboxAI
{
    public class ChatboxAIScheduledMessageService : IChatboxAIScheduledMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChatboxAIScheduledMessageService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Paged
        public async Task<PagingResult<ChatboxAIScheduledMessage>> GetAll(
            int pageIndex, int pageSize, string keyword, int? topicId = null)
        {
            if (pageSize <= 0) pageSize = 20;
            if (pageIndex < 1) pageIndex = 1;

            IQueryable<ChatboxAIScheduledMessage> query = _context.ChatboxAIScheduledMessages.AsNoTracking();

            if (topicId.HasValue)
                query = query.Where(x => x.TopicId == topicId.Value);

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(x =>
                    x.Message.Contains(keyword) ||
                    x.Topic.TopicName.Contains(keyword) ||
                    x.Topic.TopicCode.Contains(keyword));

            var result = new PagingResult<ChatboxAIScheduledMessage>
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = await query.CountAsync(),
                Data = await query
                    .Include(x => x.Topic)                        // Include ở bước cuối
                    .OrderBy(x => x.Id)
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync()
            };

            return result;
        }

        // Not paged (list)
        public IEnumerable<ChatboxAIScheduledMessage> GetAll()
        {
            return _context.ChatboxAIScheduledMessages
                .AsNoTracking()
                .Include(x => x.Topic)
                .OrderBy(x => x.Id)
                .ToList();
        }

        public async Task<string> Create(ChatboxAIScheduledMessage request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();

                // Topic phải tồn tại
                var topic = await _context.ChatboxAITopics.FindAsync(request.TopicId);
                if (topic == null)
                    return "TopicNotFound";

                // Tránh trùng khung giờ trong cùng Topic (tùy chính sách)
                var exists = await _context.ChatboxAIScheduledMessages
                    .AnyAsync(x => x.TopicId == request.TopicId && x.SendTime == request.SendTime);
                if (exists)
                    return "ScheduledMessage_TimeAlreadyExist";

                var entity = _mapper.Map<ChatboxAIScheduledMessage>(request);
                _context.ChatboxAIScheduledMessages.Add(entity);
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

        public ChatboxAIScheduledMessage GetById(int id)
        {
            return _context.ChatboxAIScheduledMessages.Find(id)!;
        }

        public async Task<string> Update(ChatboxAIScheduledMessage request)
        {
            try
            {
                // Không cho EF cố insert Topic nếu client lỡ gửi kèm
                request.Topic = null;

                // 1) Tìm bản ghi hiện có
                var entity = await _context.ChatboxAIScheduledMessages
                    .FirstOrDefaultAsync(x => x.Id == request.Id);
                if (entity == null) return "ScheduledMessage_NotFound";

                // 2) Kiểm tra Topic tồn tại
                var topicExists = await _context.ChatboxAITopics
                    .AnyAsync(t => t.Id == request.TopicId);
                if (!topicExists) return "TopicNotFound";

                // 3) Ràng buộc: không trùng khung giờ trong cùng Topic (tuỳ chính sách)
                var dup = await _context.ChatboxAIScheduledMessages.AnyAsync(x =>
                    x.Id != request.Id &&
                    x.TopicId == request.TopicId &&
                    x.SendTime == request.SendTime
                // Nếu muốn xét cả DaysOfWeek, bỏ comment dòng dưới:
                // && (x.DaysOfWeek ?? "Daily") == (request.DaysOfWeek ?? "Daily")
                );
                if (dup) return "ScheduledMessage_TimeAlreadyExist";

                // 4) Patch các trường cho phép sửa
                entity.TopicId = request.TopicId;
                entity.Message = request.Message;
                entity.SendTime = request.SendTime;
                entity.DaysOfWeek = request.DaysOfWeek;

                // Nếu client có gửi LastSentAt và bạn muốn cho phép cập nhật:
                if (request.LastSentAt.HasValue)
                    entity.LastSentAt = request.LastSentAt;

                // 5) Đánh dấu đã thay đổi (đề phòng EF không detect)
                _context.Entry(entity).Property(x => x.TopicId).IsModified = true;
                _context.Entry(entity).Property(x => x.Message).IsModified = true;
                _context.Entry(entity).Property(x => x.SendTime).IsModified = true;
                _context.Entry(entity).Property(x => x.DaysOfWeek).IsModified = true;
                if (request.LastSentAt.HasValue)
                    _context.Entry(entity).Property(x => x.LastSentAt).IsModified = true;

                var affected = await _context.SaveChangesAsync();
                return affected > 0 ? string.Empty : "NoChangeDetected";
            }
            catch (DbUpdateException ex)
            {
                // Trả về thông tin lỗi gốc để debug dễ hơn
                return ex.InnerException?.Message ?? ex.Message;
            }
        }


        public string Delete(int id)
        {
            var entity = _context.ChatboxAIScheduledMessages.Find(id);
            if (entity != null)
            {
                _context.ChatboxAIScheduledMessages.Remove(entity);
                _context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
