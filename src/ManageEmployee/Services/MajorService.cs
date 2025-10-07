using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Majors;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class MajorService : IMajorService
{
    private readonly ApplicationDbContext _context;

    public MajorService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<Major>> GetAll(int currentPage, int pageSize, string keyword)
    {
        if (pageSize <= 0)
            pageSize = 20;

        if (currentPage < 0)
            currentPage = 1;


        var result = new PagingResult<Major>()
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
        };

        var query = _context.Majors
            .Where(x => !x.isDelete)
            .Where(x => string.IsNullOrEmpty(keyword) || x.Code.Contains(keyword)
            || x.Name.Contains(keyword));
        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        return result;
    }

    public async Task<IEnumerable<Major>> GetAll()
    { 
        return await _context.Majors
            .Where(x => !x.isDelete)
                .OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<Major> GetById(int id)
    {
        return await _context.Majors.FindAsync(id);
    }

    public async Task Create(Major param)
    {
        if (await _context.Majors.Where(u => u.Code == param.Code).AnyAsync())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }
        if (await _context.Majors.Where(u => u.Name == param.Name).AnyAsync())
        {
            throw new ErrorException(ResultErrorConstants.NAME_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Majors.Add(param);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Major param)
    {
        var major = await _context.Majors.FindAsync(param.Id);

        if (major == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        if (await _context.Majors.Where(m => m.Id != param.Id && m.Code == param.Code).AnyAsync())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        major.Name = param.Name;
        major.Note = param.Note;
        major.Code = param.Code;
        _context.Majors.Update(major);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var major = await _context.Majors.FindAsync(id);
        if (major != null)
        {
            major.isDelete = true;
            _context.Majors.Update(major);
            await _context.SaveChangesAsync();
        }
    }
}