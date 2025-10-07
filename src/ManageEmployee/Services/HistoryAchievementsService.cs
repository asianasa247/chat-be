using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.HistoryAchievements;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class HistoryAchievementsService : IHistoryAchievementsService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public HistoryAchievementsService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<HistoryAchievement>> GetAllActive()
    {
        return await _context.HistoryAchievements.Where(x => !x.IsDelete).ToListAsync();
    }

    public async Task<PagingResult<HistoryAchievementModel>> GetAll(HistoryAchievementViewModel param)
    {
        var query = _context.HistoryAchievements
            .Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query.Where(x => x.Code.ToLower().Contains(param.SearchText.Trim().ToLower()) ||
                    x.Name.Trim().ToLower().Contains(param.SearchText.Trim().ToLower()) ||
                                              x.Note.Trim().ToLower().Contains(param.SearchText.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().Contains(param.SearchText.Trim().ToLower())
                                              );
        }

        if (param.UserId != null && param.UserId.Value != 0)
        {
            query = query.Where(x => x.UserId == param.UserId);
        }

        if (param.StartDate != null)
        {
            query = query.Where(x => x.Date >= param.StartDate);
        }
        if (param.EndDate != null)
        {
            query = query.Where(x => x.Date <= param.EndDate);
        }
        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).Select(x => _mapper.Map<HistoryAchievementModel>(x)).ToListAsync();
        foreach (var item in data)
        {
            if (item.UserId > 0)
            {
                item.FullName = await _context.Users.Where(x => x.Id == item.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
            }
        }
        return new PagingResult<HistoryAchievementModel>
        {
            Data = data,
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<HistoryAchievement> GetById(int id)
    {
        return await _context.HistoryAchievements.FindAsync(id);
    }

    public async Task Create(HistoryAchievement historyAchievement)
    {
        // validation
        _context.HistoryAchievements.Add(historyAchievement);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(HistoryAchievement historyAchievementParam)
    {
        var historyAchievement = await _context.HistoryAchievements.AsNoTracking().FirstOrDefaultAsync(x => x.Id == historyAchievementParam.Id);
        HistoryAchievement submitHistoryAchievement;
        if (historyAchievement == null)
            throw new ErrorException(ResultErrorConstants.USER_EMPTY_OR_DELETE);

        submitHistoryAchievement = _mapper.Map<HistoryAchievement>(historyAchievementParam);
        submitHistoryAchievement.Id = historyAchievementParam.Id;
        submitHistoryAchievement.UserId = historyAchievementParam.UserId;
        submitHistoryAchievement.Name = historyAchievementParam.Name;
        submitHistoryAchievement.Code = historyAchievementParam.Code;
        submitHistoryAchievement.Note = historyAchievementParam.Note;
        submitHistoryAchievement.Description = historyAchievementParam.Description;
        submitHistoryAchievement.Date = historyAchievementParam.Date;
        submitHistoryAchievement.UpdateAt = DateTime.Now;

        _context.HistoryAchievements.Update(submitHistoryAchievement);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var historyAchievement = await _context.HistoryAchievements.FindAsync(id);
        if (historyAchievement != null)
        {
            historyAchievement.IsDelete = true;
            _context.HistoryAchievements.Update(historyAchievement);
            await _context.SaveChangesAsync();
        }
    }
}