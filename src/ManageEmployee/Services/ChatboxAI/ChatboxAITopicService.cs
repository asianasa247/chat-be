using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ChatboxAI;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChatboxAI
{
    public class ChatboxAITopicService : IChatboxAITopicService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ChatboxAITopicService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<ChatboxAITopic> GetAll()
        {

            var data = _context.ChatboxAITopics.Select(x => new ChatboxAITopic
            {
                Id = x.Id,
                TopicName = x.TopicName,
                TopicCode = x.TopicCode
            }).ToList();
            return data;
        }

        public async Task<PagingResult<ChatboxAITopic>> GetAll(int pageIndex, int pageSize, string keyword)
        {
            try
            {
                if (pageSize <= 0)
                    pageSize = 20;

                if (pageIndex < 0)
                    pageIndex = 0;

                var datas = _context.ChatboxAITopics
                    .Select(x => new ChatboxAITopic
                    {
                        Id = x.Id,
                        TopicCode = x.TopicCode,
                        TopicName = x.TopicName
                    });

                var result = new PagingResult<ChatboxAITopic>()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    Data = await datas.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(),
                    TotalItems = await datas.CountAsync()
                };
                return result;
            }
            catch
            {
                return new PagingResult<ChatboxAITopic>()
                {
                    CurrentPage = pageIndex,
                    PageSize = pageSize,
                    TotalItems = 0,
                    Data = new List<ChatboxAITopic>()
                };
            }
        }

        public async Task<string> Create(ChatboxAITopic request)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var existCode = _context.ChatboxAITopics.Where(
                    x => x.TopicCode.ToLower() == request.TopicCode.ToLower()).FirstOrDefault();
                if (existCode != null)
                {
                    return ErrorMessages.ChatboxAITopicCodeAlreadyExist;
                }
                ChatboxAITopic ChatboxAITopic = _mapper.Map<ChatboxAITopic>(request);
                _context.ChatboxAITopics.Add(ChatboxAITopic);
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();

                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public ChatboxAITopic GetById(int id)
        {
            var ChatboxAITopic = _context.ChatboxAITopics.Find(id);
            return ChatboxAITopic;
        }

        public async Task<string> Update(ChatboxAITopic request)
        {
            var checkChatboxAITopicCode = await _context.ChatboxAITopics.Where(x => x.TopicCode.ToLower() == request.TopicCode.ToLower() && x.Id != request.Id).FirstOrDefaultAsync();
            if (checkChatboxAITopicCode != null && checkChatboxAITopicCode.Id != request.Id)
            {
                return ErrorMessages.ChatboxAITopicCodeAlreadyExist;
            }
            _context.ChatboxAITopics.Update(request);

            await _context.SaveChangesAsync();
            return string.Empty;
        }

        public string Delete(int id)
        {
            var ChatboxAITopic = _context.ChatboxAITopics.Find(id);
            if (ChatboxAITopic != null)
            {
                _context.ChatboxAITopics.Remove(ChatboxAITopic);
                _context.SaveChanges();
            }
            return string.Empty;
        }
    }
}
