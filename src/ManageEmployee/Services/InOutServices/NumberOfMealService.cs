using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.InOuts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.NumberOfMealModels;

namespace ManageEmployee.Services.InOutServices;

public class NumberOfMealService : INumberOfMealService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public NumberOfMealService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<NumberOfMealModel>> GetPaging(PagingRequestFilterDateModel param)
    {
        var query = _context.NumberOfMeals.Where(x => x.Id > 0);
        if (param.FromAt != null)
        {
            query = query.Where(x => x.Date >= param.FromAt);
        }

        if (param.ToAt != null)
        {
            var toAt = param.ToAt.Value.AddDays(1);
            query = query.Where(x => x.Date < toAt);
        }

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).Select(X => _mapper.Map<NumberOfMealModel>(X)).ToListAsync();
        var totalItem = await query.CountAsync();

        return new PagingResult<NumberOfMealModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task SetDetail(NumberOfMealDetailModel form)
    {
        var item = await _context.NumberOfMealDetails.FindAsync(form.Id);
        if (item is null)
        {
            item = new NumberOfMealDetail();
            item.CreatedAt = DateTime.Now;
        }

        item.UserId = form.UserId;
        item.UserName = form.UserName;
        item.CustomerName = form.CustomerName;
        item.Address = form.Address;
        item.Date = new DateTime(form.Date.Year, form.Date.Month, form.Date.Day);
        item.QuantityAdd = form.QuantityAdd;
        item.Note = form.Note;
        item.Type = form.Type;
        item.UpdatedAt = DateTime.Now;
        _context.NumberOfMealDetails.Update(item);

        var numberOfMeal = await _context.NumberOfMeals.Where(x => x.Date == item.Date && x.TimeType == form.TimeType).FirstOrDefaultAsync();
        if (numberOfMeal is null)
            numberOfMeal = new NumberOfMeal
            {
                Date = item.Date,
                TimeType = form.TimeType,
            };
        numberOfMeal.QuantityAdd += form.QuantityAdd;
        _context.NumberOfMeals.Update(numberOfMeal);

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<NumberOfMealDetailModel>> GetDetail(DateTime date, string timeType)
    {
        var meal = await _context.NumberOfMeals.FirstOrDefaultAsync(x => x.Date == date && x.TimeType == timeType);
        if (meal is null)
        {
            return new List<NumberOfMealDetailModel>();
        }

        var itemOuts = await _context.NumberOfMealDetails.Where(x => x.Date == date && x.Type == timeType)
            .Select(x => _mapper.Map<NumberOfMealDetailModel>(x)).ToListAsync();
        foreach (var itemOut in itemOuts)
        {
            itemOut.UserName = await _context.Users.Where(x => x.Id == itemOut.UserId).Select(x => x.FullName).FirstOrDefaultAsync();
        }

        if (!string.IsNullOrEmpty(meal.UserIdStr))
        {
            var userIds = JsonConvert.DeserializeObject<List<int>>(meal.UserIdStr);
            var detailInouts = await _context.Users.Where(x => userIds.Contains(x.Id))
                .Select(x => new NumberOfMealDetailModel
                {
                    Date = date,
                    TimeType = timeType,
                    Type = nameof(NumberOfMealDetailType.inout),
                    UserName = x.FullName,
                    QuantityAdd = 1
                }).ToListAsync();
            itemOuts.AddRange(detailInouts);
        }

        return itemOuts;
    }

    public async Task<bool> DeleteMealDetail(int mealDetailId)
    {
        var meal = await _context.NumberOfMealDetails.FindAsync(mealDetailId);
        if (meal == null)
        {
            return false;
        }
        _context.NumberOfMealDetails.Remove(meal);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteMealDetails(List<int> mealDetailIds)
    {
        var meal = await _context.NumberOfMealDetails
                                .Where(x => mealDetailIds.Contains(x.Id))
                                .ToListAsync();
        if (!meal.Any())
        {
            return false;
        }

        _context.NumberOfMealDetails.RemoveRange(meal);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteMeal(int mealId)
    {
        var meal = await _context.NumberOfMeals.FindAsync(mealId);
        if (meal == null)
        {
            return false;
        }
        _context.NumberOfMeals.Remove(meal);

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteMeals(List<int> mealIds)
    {
        var meal = await _context.NumberOfMeals
                                .Where(x => mealIds.Contains(x.Id))
                                .ToListAsync();
        if (!meal.Any())
        {
            return false;
        }

        _context.NumberOfMeals.RemoveRange(meal);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task UpdateMeal(DateTime date)
    {
        var meals = await _context.NumberOfMeals.Where(x => x.Date == date).ToListAsync();
        var lunchMeal = meals.FirstOrDefault(x => x.TimeType == TimeTypeConst.lunch);
        var afternoonMeal = meals.FirstOrDefault(x => x.TimeType == TimeTypeConst.afternoon);
        var dinnerMeal = meals.FirstOrDefault(x => x.TimeType == TimeTypeConst.dinner);

        var inoutQuery = GetInoutForMeal(date);

        // Lunch meals number today
        if (lunchMeal == null)
        {
            lunchMeal = new NumberOfMeal
            {
                TimeType = TimeTypeConst.lunch,
                Date = date,
            };
            meals.Add(lunchMeal);
        }
        lunchMeal.QuantityFromInOut = (await inoutQuery.Where(x => x.TimeIn.Value.Hour >= 6 && x.TimeIn.Value.Hour <= 9 && (x.NumberOfMeals == 1 || x.NumberOfMeals == 2)).CountAsync())
                                    + (await _context.NumberOfMealDetails.Where(x => x.Date == date && x.TimeType == TimeTypeConst.lunch).CountAsync());
        var userIds = await inoutQuery.Where(x => x.TimeIn.Value.Hour >= 6 && x.TimeIn.Value.Hour <= 9 && (x.NumberOfMeals == 1 || x.NumberOfMeals == 2)).Select(x => x.UserId).Distinct().ToListAsync();
        lunchMeal.UserIdStr = JsonConvert.SerializeObject(userIds);

        // Afternoon meals number today
        if (afternoonMeal == null)
        {
            afternoonMeal = new NumberOfMeal
            {
                TimeType = TimeTypeConst.afternoon,
                Date = date,
            };
            meals.Add(afternoonMeal);
        }
        afternoonMeal.QuantityFromInOut = (await inoutQuery.Where(x => x.TimeIn.Value.Hour >= 6 && x.TimeIn.Value.Hour <= 9 && x.NumberOfMeals == 2).CountAsync())
                                            + (await _context.NumberOfMealDetails.Where(x => x.Date == date && x.TimeType == TimeTypeConst.afternoon).CountAsync());
        userIds = await inoutQuery.Where(x => x.TimeIn.Value.Hour >= 6 && x.TimeIn.Value.Hour <= 9 && x.NumberOfMeals == 2).Select(x => x.UserId).Distinct().ToListAsync();
        afternoonMeal.UserIdStr = JsonConvert.SerializeObject(userIds);

        // Dinner meals number
        if (dinnerMeal == null)
        {
            dinnerMeal = new NumberOfMeal
            {
                TimeType = TimeTypeConst.dinner,
                Date = date,
            };
            meals.Add(dinnerMeal);

        }
        dinnerMeal.QuantityFromInOut = (await inoutQuery.Where(x => x.TimeIn.Value.Hour >= 16 && x.TimeIn.Value.Hour <= 21 && (x.NumberOfMeals == 1 || x.NumberOfMeals == 2)).CountAsync())
                                        + (await _context.NumberOfMealDetails.Where(x => x.Date == date && x.TimeType == TimeTypeConst.dinner).CountAsync());
        userIds = await inoutQuery.Where(x => x.TimeIn.Value.Hour >= 16 && x.TimeIn.Value.Hour <= 21 && (x.NumberOfMeals == 1 || x.NumberOfMeals == 2)).Select(x => x.UserId).Distinct().ToListAsync();
        dinnerMeal.UserIdStr = JsonConvert.SerializeObject(userIds);


        // Morning meals number for tomorrow
        var morningMeal = meals.FirstOrDefault(x => x.TimeType == TimeTypeConst.morning);
        if (morningMeal == null)
        {
            morningMeal = new NumberOfMeal
            {
                TimeType = TimeTypeConst.morning,
                Date = date,
            };
            meals.Add(morningMeal);
        }

        var inoutMorningQuery = GetInoutForMeal(date.AddDays(-1));

        morningMeal.QuantityFromInOut = (await inoutMorningQuery.Where(x => x.TimeIn.Value.Hour >= 16 && x.TimeIn.Value.Hour <= 21 && x.NumberOfMeals == 2).CountAsync())
                                        + (await _context.NumberOfMealDetails.Where(x => x.Date == date && x.TimeType == TimeTypeConst.morning).CountAsync());
        userIds = await inoutMorningQuery.Where(x => x.TimeIn.Value.Hour >= 16 && x.TimeIn.Value.Hour <= 21 && x.NumberOfMeals == 2).Select(x => x.UserId).Distinct().ToListAsync();
        morningMeal.UserIdStr = JsonConvert.SerializeObject(userIds);

        _context.NumberOfMeals.UpdateRange(meals);
        await _context.SaveChangesAsync();
    }
    private IQueryable<InoutForMealModel> GetInoutForMeal(DateTime date)
    {
        return _context.InOutHistories
           .Join(_context.Users,
                   b => b.UserId,
                   d => d.Id,
                   (b, d) => new
                   {
                       inout = b,
                       user = d
                   })
           .Where(x => x.inout.TimeIn.Value.Year == date.Year && x.inout.TimeIn.Value.Month == date.Month && x.inout.TimeIn.Value.Day == date.Day)
           .Select(x => new InoutForMealModel
           {
               TimeIn = x.inout.TimeIn,
               NumberOfMeals = x.user.NumberOfMeals,
               Id = x.inout.Id,
               UserId =x.inout.UserId
           }
           ).Distinct();
    }
}