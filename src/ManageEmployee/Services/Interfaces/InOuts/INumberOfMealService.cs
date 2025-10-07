using ManageEmployee.DataTransferObject.NumberOfMealModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.InOuts;

public interface INumberOfMealService
{
    Task<IEnumerable<NumberOfMealDetailModel>> GetDetail(DateTime date, string timeType);

    Task<PagingResult<NumberOfMealModel>> GetPaging(PagingRequestFilterDateModel param);

    Task SetDetail(NumberOfMealDetailModel form);

    Task<bool> DeleteMealDetail(int mealDetailId);

    Task<bool> DeleteMealDetails(List<int> mealDetailIds);

    Task<bool> DeleteMeal(int mealId);

    Task<bool> DeleteMeals(List<int> mealIds);

    Task UpdateMeal(DateTime date);
}
