using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Webs;

namespace ManageEmployee.Services.Web;
public class IsoftHistoryService : IIsoftHistoryService
{
    private ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public IsoftHistoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<string> Create(IsoftHistory request)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            _context.IsoftHistory.Add(request);
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

    public void Delete(int id)
    {
        var itemDelete = _context.IsoftHistory.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefault();
        if (itemDelete != null)
        {
            itemDelete.IsDelete = true;
            itemDelete.DeleteAt = DateTime.Now;
            _context.IsoftHistory.Update(itemDelete);
            _context.SaveChanges();
        }
    }

    public PagingResult<IsoftHistory> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var histories = _context.IsoftHistory.Where(x => x.IsDelete != true && x.Id != 0)
                                         .Where(x => string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(keyword.ToLower())))
                                         .Select(x => new IsoftHistory
                                         {
                                             Id = x.Id,
                                             Title = x.Title,
                                             Content = x.Content,
                                             CreatedAt = x.CreatedAt,
                                             Name = x.Name,
                                             ClassName = x.ClassName,
                                             Order = x.Order,
                                         })
                                         .ToList();

            return new PagingResult<IsoftHistory>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = histories.Count,
                Data = histories.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList()
            };
        }
        catch
        {
            return new PagingResult<IsoftHistory>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<IsoftHistory>()
            };
        }
    }

    public IsoftHistory GetById(int id)
    {
        try
        {
            var itemData = _context.IsoftHistory.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefault();
            if (itemData != null)
            {
                return new IsoftHistory
                {
                    Id = itemData.Id,
                    Title = itemData.Title,
                    Content = itemData.Content,
                    CreatedAt = itemData.CreatedAt,
                    Name = itemData.Name,
                    ClassName = itemData.ClassName,
                    Order = itemData.Order
                };
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<string> Update(IsoftHistory request)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var itemUpdate = _context.IsoftHistory.Where(x => x.Id == request.Id && x.IsDelete != true).FirstOrDefault();
            if (itemUpdate == null)
            {
                return ErrorMessages.DataNotFound;
            }
            itemUpdate.Title = request.Title;
            itemUpdate.Content = request.Content;
            itemUpdate.UpdatedAt = DateTime.Now;
            itemUpdate.Name = request.Name;
            itemUpdate.Order = request.Order;
            _context.IsoftHistory.Update(itemUpdate);
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

    public HistoryByClassNameModel GetHistoryByClassName(string className, string keyword)
    {
        var data = _context.IsoftHistory.Where(x => x.IsDelete != true && x.ClassName == className &&
                                                (string.IsNullOrEmpty(keyword) || x.Name.ToLower().Contains(keyword.ToLower()) ||
                                                    x.Title.ToLower().Contains(keyword.ToLower())
                                                )).Select(x => new IsoftHistoryViewModel
                                                {
                                                    Id = x.Id,
                                                    Name = x.Name,
                                                    Title = x.Title,
                                                    ClassName = x.ClassName,
                                                    Content = x.Content,
                                                    CreateAt = x.CreatedAt,
                                                    Order = x.Order
                                                }).ToList();
        return new HistoryByClassNameModel
        {
            ClassName = (data != null && data.Count > 0) ? data[0].ClassName : String.Empty,
            FirstExercise = (data != null && data.Count > 0) ? data[0] : new IsoftHistoryViewModel(),
            Exercises = data.Select(x => new OptionItem
            {
                Id = x.Id,
                Code = String.Empty,
                Name = x.Name,
                Order = x.Order
            }).OrderBy(o => o.Order).ToList()
        };
    }
}