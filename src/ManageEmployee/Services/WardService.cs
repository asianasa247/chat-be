using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.AddressEntities;

namespace ManageEmployee.Services;

public class WardService : IWardService
{
    private readonly ApplicationDbContext _context;

    public WardService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Ward> GetAll(int currentPage, int pageSize)
    {
        return _context.Wards
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortCode)
            .Skip(pageSize * (currentPage - 1))
            .Take(pageSize);
    }

    public async Task<List<Ward>> GetAll(Expression<Func<Ward, bool>> where)
    {
        return await _context.Wards.AsNoTracking()
            .Where(where)
            .Where(x => !x.IsDeleted)
            .Take(200)
            .ToListAsync();
    }

    public IEnumerable<Ward> GetAllByDistrictId(int districtId)
    {
        return _context.Wards
            .Where(x => !x.IsDeleted && x.DistrictId == districtId)
            .OrderBy(x => x.SortCode);
    }

    public Ward GetById(int id)
    {
        return _context.Wards.Find(id);
    }

    public Ward Create(Ward param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Wards.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(Ward param)
    {
        var ward = _context.Wards.Find(param.Id);

        if (ward == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        ward.Name = param.Name;
        ward.Type = param.Type;
        ward.DistrictId = param.DistrictId;
        ward.SortCode = param.SortCode;

        _context.Wards.Update(ward);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ward = _context.Wards.Find(id);
        if (ward != null)
        {
            ward.IsDeleted = true;
            _context.Wards.Update(ward);
            _context.SaveChanges();
        }
    }

    public int Count(Expression<Func<Ward, bool>> where = null)
    {
        if (where != null)
        {
            return _context.Wards.Where(where).Count(x => !x.IsDeleted);
        }
        return _context.Wards.Count(x => !x.IsDeleted);
    }
}