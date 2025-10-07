using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class ProvinceService : IProvinceService
{
    private readonly ApplicationDbContext _context;

    public ProvinceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Province> GetAll(int currentPage, int pageSize)
    {
        if (pageSize == 0)
        {
            return _context.Provinces
                          .Where(x => !x.IsDeleted)
                          .OrderBy(x => x.SortCode)
                          .ThenBy(x => x.Name);
        }
        return _context.Provinces
             .Where(x => !x.IsDeleted)
             .OrderBy(x => x.SortCode)
             .ThenBy(x => x.Name)
             .Skip(pageSize * (currentPage - 1))
             .Take(pageSize);
    }

    public async Task<IEnumerable<Province>> GetAll()
    {
        return await _context.Provinces
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.SortCode).ToListAsync();
    }

    public Province GetById(int id)
    {
        return _context.Provinces.Find(id);
    }

    public async Task Create(Province param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        await _context.Provinces.AddAsync(param);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Province param)
    {
        var province = await _context.Provinces.FindAsync(param.Id);

        if (province == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        province.Name = param.Name;
        province.Type = param.Type;
        province.ZipCode = param.ZipCode;
        province.SortCode = param.SortCode;

        _context.Provinces.Update(province);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var province = await _context.Provinces.FindAsync(id);
        if (province != null)
        {
            province.IsDeleted = true;
            _context.Provinces.Update(province);
           await  _context.SaveChangesAsync();
        }
    }

    public int Count()
    {
        return _context.Provinces.Count(x => !x.IsDeleted);
    }
}