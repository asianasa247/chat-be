using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Degrees;

namespace ManageEmployee.Services;

public class DegreeService : IDegreeService
{
    private readonly ApplicationDbContext _context;

    public DegreeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Degree> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.Degrees
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

    public IEnumerable<Degree> GetAll()
    {
        return _context.Degrees
            .Where(x => !x.IsDelete)
                .OrderBy(x => x.Name);
    }

    public Degree GetById(int id)
    {
        return _context.Degrees.Find(id);
    }

    public Degree Create(Degree param)
    {
        if (_context.Degrees.Where(u => u.Name == param.Name).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Degrees.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(Degree param)
    {
        var degree = _context.Degrees.Find(param.Id);

        if (degree == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        degree.Name = param.Name;
        degree.Description = param.Description;
        degree.Status = param.Status;
        degree.CompanyId = param.CompanyId;

        degree.UpdatedAt = DateTime.Now;
        degree.UserUpdated = param.UserUpdated;

        _context.Degrees.Update(degree);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var degree = _context.Degrees.Find(id);
        if (degree != null)
        {
            degree.IsDelete = true;
            degree.DeleteAt = DateTime.Now;
            _context.Degrees.Update(degree);
            _context.SaveChanges();
        }
    }

    public int Count(string keyword)
    {
        var query = _context.Degrees.Where(x => !x.IsDelete);

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
}