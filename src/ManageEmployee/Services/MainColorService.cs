using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.MainColors;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class MainColorService : IMainColorService
{
    private readonly ApplicationDbContext _context;

    public MainColorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<MainColor>> GetAll(int pageIndex, int pageSize, string keyword, int userId)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var result = new PagingResult<MainColor>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
            };

            var branchs = _context.MainColors.Where(x => x.UserId == userId)
                            .AsNoTracking();
            result.TotalItems = await branchs.CountAsync();
            result.Data = await branchs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return result;
        }
        catch
        {
            return new PagingResult<MainColor>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<MainColor>()
            };
        }
    }

    public async Task<string> Create(MainColor request, int userId)
    {
        var item = _context.MainColors.FirstOrDefault(x => x.UserId == userId);
        if (item != null)
        {
            item.IsDark = request.IsDark;
            item.Color = request.Color;
            item.Theme = request.Theme;
            _context.MainColors.Update(item);
        }
        else
        {
            request.UserId = userId;
            _context.MainColors.Add(request);
        }
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<MainColor> GetById(int id)
    {
        try
        {
            var item = await _context.MainColors.FirstOrDefaultAsync(x => x.Id == id);
            if (item != null)
            {
                return item;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<string> Update(MainColor request)
    {
        var branch = _context.MainColors.Find(request.Id);
        if (branch == null)
        {
            return ErrorMessages.DataNotFound;
        }
        var user = _context.Users.FirstOrDefault(x => x.BranchId == request.Id && !x.Status);
        if (user != null)
            return ErrorMessages.DataExist;

        branch.IsDark = request.IsDark;
        branch.Color = request.Color;
        branch.Theme = request.Theme;
        _context.MainColors.Update(branch);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Delete(int id)
    {
        var branch = await _context.MainColors.FindAsync(id);
        if (branch != null)
        {
            _context.MainColors.Remove(branch);
            await _context.SaveChangesAsync();
        }
        return string.Empty;
    }

    public async Task<MainColor> GetByUserId(int userId)
    {
        try
        {
            var item = await _context.MainColors.FirstOrDefaultAsync(x => x.UserId == userId);
            if (item != null)
            {
                return item;
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }
}