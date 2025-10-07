using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Users.Salaries;

namespace ManageEmployee.Services.UserServices.SalaryServices;

public class SalaryLevelService : ISalaryLevelService
{
    private ApplicationDbContext _context;

    public SalaryLevelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SalaryLevel> GetAll(int currentPage, int pageSize, string keyword = "")
    {
        var query = _context.SalaryLevel.Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                            x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                            x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                                 x.PositionName.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                            x.PositionName.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                            x.PositionName.Trim().ToLower().Equals(keyword.Trim().ToLower())
                                            );
        }

        return query
            .Skip(pageSize * currentPage)
            .Take(pageSize).ToList();
    }

    public SalaryLevel Create(SalaryLevel param)
    {
        _context.SalaryLevel.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(SalaryLevel param)
    {
        var position = _context.SalaryLevel.Find(param.Id);

        if (position == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        position.Name = param.Name;
        position.PositionId = param.PositionId;
        position.PositionName = param.PositionName;
        position.SalaryCost = param.SalaryCost;
        position.Amount = param.Amount;
        position.Date = param.Date;
        position.Coefficient = param.Coefficient;
        position.Note = param.Note;
        position.UpdatedAt = DateTime.Now;
        position.UserUpdated = param.UserUpdated;

        _context.SalaryLevel.Update(position);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var position = _context.SalaryLevel.Find(id);
        if (position != null)
        {
            position.IsDelete = true;
            position.DeleteAt = DateTime.Now;
            _context.SalaryLevel.Update(position);
            _context.SaveChanges();
        }
    }

    public int Count(string keyword = "")
    {
        var query = _context.SalaryLevel.Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                            x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                            x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                                 x.PositionName.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                            x.PositionName.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                            x.PositionName.Trim().ToLower().Equals(keyword.Trim().ToLower())
                                            );
        }
        return query.Count();
    }
}