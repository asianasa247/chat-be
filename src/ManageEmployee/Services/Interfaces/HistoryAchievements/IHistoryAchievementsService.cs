using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.HistoryAchievements;

public interface IHistoryAchievementsService
{
    Task<IEnumerable<HistoryAchievement>> GetAllActive();

    Task<PagingResult<HistoryAchievementModel>> GetAll(HistoryAchievementViewModel param);

    Task<HistoryAchievement> GetById(int id);

    Task Create(HistoryAchievement historyAchievement);

    Task UpdateAsync(HistoryAchievement historyAchievementParam);

    Task Delete(int id);
}
