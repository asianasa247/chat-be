using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class ChartOfAccountGroupLinkService : IChartOfAccountGroupLinkService
{
    private readonly ApplicationDbContext _context;

    public ChartOfAccountGroupLinkService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<ChartOfAccountGroupLink> GetAll(int currentPage, int pageSize, int year)
    {
        if (pageSize == 0)
        {
            return _context.GetChartOfAccountGroupLink(year);
        }
        return _context.GetChartOfAccountGroupLink(year)
             .Skip(pageSize * (currentPage - 1))
             .Take(pageSize);
    }

    public IEnumerable<ChartOfAccountGroupLink> GetAll(int year)
    {
        return _context.GetChartOfAccountGroupLink(year);
    }

    public ChartOfAccountGroupLink GetById(int id)
    {
        return _context.ChartOfAccountGroupLinks.Find(id);
    }

    public ChartOfAccountGroupLink Create(ChartOfAccountGroupLink param, int year)
    {
        _context.ChartOfAccountGroupLinks.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(ChartOfAccountGroupLink param, int year)
    {
        var ChartOfAccountGroupLink = _context.ChartOfAccountGroupLinks.Find(param.Id);

        if (ChartOfAccountGroupLink == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        ChartOfAccountGroupLink.Year = year;

        _context.ChartOfAccountGroupLinks.Update(ChartOfAccountGroupLink);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ChartOfAccountGroupLink = _context.ChartOfAccountGroupLinks.Find(id);
        if (ChartOfAccountGroupLink != null)
        {
            _context.ChartOfAccountGroupLinks.Remove(ChartOfAccountGroupLink);
            _context.SaveChanges();
        }
    }

    public async Task<List<SelectListModel>> GetAllAccountGroupLinks(int year)
    {
        try
        {
            IQueryable<SelectListModel> query =
                from coa in _context.GetChartOfAccount(year)
                where coa.Type < 5
                && coa.DisplayInsert == true
                //&& coa.HasDetails == false
                //&& !(_context.ChartOfAccountGroupLinks.Select(l => l.CodeChartOfAccount).ToArray()).Contains(coa.Code)
                select new SelectListModel()
                {
                    Code = coa.Code,
                    Name = coa.Name,
                };

            var res = await query
                .OrderBy(x => x.Code)
                .ToListAsync();

            return res;
        }
        catch
        {
            return new List<SelectListModel>();
        }
    }
}