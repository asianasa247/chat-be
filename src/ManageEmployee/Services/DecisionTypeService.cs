using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DecideEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.DecisionTypes;

namespace ManageEmployee.Services;

public class DecisionTypeService : IDecisionTypeService
{
    private ApplicationDbContext _context;

    public DecisionTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<DecisionType> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.DecisionType
            .Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                                   x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
                                              );
        }

        return query
            .Skip(pageSize * currentPage)
            .Take(pageSize);
    }

    public DecisionType Create(DecisionType param)
    {
        if (_context.DecisionType.Where(u => u.Name == param.Name).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.DecisionType.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(DecisionType param)
    {
        var data = _context.DecisionType.Find(param.Id);

        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        data.Name = param.Name;
        data.Description = param.Description;
        data.Status = param.Status;

        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = param.UserUpdated;

        _context.DecisionType.Update(data);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var data = _context.DecisionType.Find(id);
        if (data != null)
        {
            data.IsDelete = true;
            data.DeleteAt = DateTime.Now;
            _context.DecisionType.Update(data);
            _context.SaveChanges();
        }
    }

    public int Count(string keyword)
    {
        var query = _context.DecisionType.Where(x => !x.IsDelete);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                                   x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
                                              );
        }
        return query.Count();
    }

    public IEnumerable<DecisionType> GetAllList()
    {
        return _context.DecisionType.Where(x => !x.IsDelete);
    }
}