using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.DataTransferObject.AddressModels;

namespace ManageEmployee.Services;

public class DistrictService : IDistrictService
{
    private readonly ApplicationDbContext _context;

    public DistrictService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<District>> GetAll(int currentPage, int pageSize)
    {
        return await _context.Districts
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortCode)
            .Skip(pageSize * (currentPage - 1))
            .Take(pageSize).ToListAsync();
    }

    public async Task<List<DistrictGetListModel>> GetAll(Expression<Func<DistrictGetListModel, bool>> where)
    {
        return await _context.Districts
            .Where(x => !x.IsDeleted)
            .Select(x => new DistrictGetListModel
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                ProvinceId = x.ProvinceId
            })
            .Where(where)
            .OrderBy(x => x.ProvinceId)
            .ToListAsync();
    }

    public async Task<IEnumerable<District>> GetAllByProvinceId(int provinceId)
    {
        return await _context.Districts.AsNoTracking()
            .Where(x => !x.IsDeleted && x.ProvinceId == provinceId)
            .OrderBy(x => x.SortCode).ToListAsync();
    }

    public IEnumerable<District> GetMany(Expression<Func<District, bool>> where, int currentPage, int pageSize)
    {
        if (pageSize == 0)
        {
            return _context.Districts
                 .Where(where)
                 .OrderBy(x => x.SortCode).ThenBy(x => x.Name);
        }
        return _context.Districts
            .Where(where)
            .OrderBy(x => x.SortCode)
            .ThenBy(x => x.Name)
            .Skip(pageSize * currentPage)
            .Take(pageSize);
    }

    public IEnumerable<District> GetMany(Expression<Func<District, bool>> where)
    {
        return _context.Districts
            .Where(where);
    }

    public District GetById(int id)
    {
        return _context.Districts.Find(id);
    }

    public District Create(District param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Districts.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(District param)
    {
        var district = _context.Districts.Find(param.Id);

        if (district == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        district.Name = param.Name;
        district.Type = param.Type;
        district.ProvinceId = param.ProvinceId;
        district.SortCode = param.SortCode;
        _context.Districts.Update(district);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var district = _context.Districts.Find(id);
        if (district != null)
        {
            district.IsDeleted = true;
            _context.Districts.Update(district);
            _context.SaveChanges();
        }
    }

    public int Count(Expression<Func<District, bool>> where = null)
    {
        if (where != null)
        {
            return _context.Districts.Where(where).Count(x => !x.IsDeleted);
        }
        return _context.Districts.Count(x => !x.IsDeleted);
    }
}