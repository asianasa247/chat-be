using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;

namespace ManageEmployee.Services;

public class ChartOfAccountGroupService : IChartOfAccountGroupService
{
    private readonly ApplicationDbContext _context;

    public ChartOfAccountGroupService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ChartOfAccountGroup> GetAll(int currentPage, int pageSize, int year)
    {
        if (pageSize == 0)
        {
            return _context.GetChartOfAccountGroup(year);
        }
        return _context.GetChartOfAccountGroup(year)
             .Skip(pageSize * (currentPage - 1))
             .Take(pageSize);
    }

    public IEnumerable<ChartOfAccountGroup> GetAll(int year)
    {
        return _context.GetChartOfAccountGroup(year);
    }

    public ChartOfAccountGroup GetById(int id)
    {
        return _context.ChartOfAccountGroups.Find(id);
    }

    public ChartOfAccountGroup Create(ChartOfAccountGroup param, int year)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        param.Year = year;

        _context.ChartOfAccountGroups.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(ChartOfAccountGroup param)
    {
        var ChartOfAccountGroup = _context.ChartOfAccountGroups.Find(param.Id);

        if (ChartOfAccountGroup == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        ChartOfAccountGroup.Name = param.Name;
        _context.ChartOfAccountGroups.Update(ChartOfAccountGroup);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ChartOfAccountGroup = _context.ChartOfAccountGroups.Find(id);
        if (ChartOfAccountGroup != null)
        {
            _context.ChartOfAccountGroups.Remove(ChartOfAccountGroup);
            _context.SaveChanges();
        }
    }
}