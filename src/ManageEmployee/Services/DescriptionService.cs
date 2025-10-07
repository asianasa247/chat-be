using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Handlers;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Descriptions;
using ManageEmployee.Entities;

namespace ManageEmployee.Services;

public class DescriptionService : IDescriptionService
{
    private readonly ApplicationDbContext _context;

    public DescriptionService(ApplicationDbContext context)
    {
        _context = context;
    }
    public IEnumerable<Description> GetAll()
    {
        return _context.Descriptions.ToList();
    }
    public async Task<List<Description>> GetPage(int currentPage, int pageSize, string query = "")
    {
        var searchQuery = string.IsNullOrEmpty(query) ? "" : query.Trim();
        if (currentPage == 0)
            currentPage = 1;
        var linqQuery = _context.Descriptions
            .Where(p =>
                p.Name.Contains(searchQuery));

        if (pageSize > 0)
        {
            linqQuery = linqQuery.Skip(0 * pageSize).Take(pageSize);
        }

        var data = await linqQuery
            //.OrderBy(x => x.Name.ToLower().IndexOf(query.ToLower()))
            //.OrderBy(x => Math.Abs(x.Name.Length - query.Length))
            .ToListAsync();
        return data ?? new List<Description>();
    }

    public async Task<int> CountAll()
    {
        return await _context.Descriptions.CountAsync();
    }

    public async Task<CustomActionResult<Description>> Create(Description entity)
    {
        if (await _context.Descriptions.AnyAsync(x => x.Name == entity.Name))
            return new CustomActionResult<Description>
            {
                IsSuccess = false,
                ErrorMessage = ErrorMessages.DescriptionNameAlreadyExist
            };
        _context.Descriptions.Add(entity);
        await _context.SaveChangesAsync();
        return new CustomActionResult<Description>
        {
            IsSuccess = true,
            SuccessData = entity
        };
    }

    public async Task<CustomActionResult<Description>> Update(Description entity)
    {
        if (await _context.Descriptions.AnyAsync(x => x.Name == entity.Name && x.Id != entity.Id))
            return new CustomActionResult<Description>
            {
                IsSuccess = false,
                ErrorMessage = ErrorMessages.DescriptionNameAlreadyExist
            };
        var existingEntity = await _context.Descriptions.FindAsync(entity.Id);
        if (existingEntity == null)
            return new CustomActionResult<Description>
            {
                IsSuccess = false,
                ErrorMessage = ErrorMessages.DataNotFound
            };
        _context.Descriptions.Update(entity);
        await _context.SaveChangesAsync();
        return new CustomActionResult<Description>
        {
            IsSuccess = true,
            SuccessData = entity
        };
    }

    public async Task<CustomActionResult<Description>> Delete(long id)
    {
        var entity = await _context.Descriptions.FindAsync(id);
        if (entity == null)
            return new CustomActionResult<Description>
            {
                IsSuccess = false,
                ErrorMessage = ErrorMessages.DataNotFound
            };
        _context.Descriptions.Remove(entity);
        await _context.SaveChangesAsync();
        return new CustomActionResult<Description>
        {
            IsSuccess = true,
            SuccessData = entity
        };
    }

    public string Delete(IEnumerable<long> ids)
    {
        string message = "";
        foreach (var id in ids)
        {
            var entity = _context.Descriptions.Find(id);
            if (entity != null)
            {
                _context.Descriptions.Remove(entity);
                _context.SaveChanges();
            }
            else
            {
                message += ErrorMessages.DataNotFound;
            }
        }
        return message;
    }
}