using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AllowanceModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.AllowanceEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Allowances;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class AllowanceService : IAllowanceService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AllowanceService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<AllowanceViewModel>> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.Allowances
            .Where(x => !x.IsDelete)
            .Select(x => _mapper.Map< AllowanceViewModel>(x));
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower())
                                              );
        }

        return new PagingResult<AllowanceViewModel>()
        {
            Data = await query.Skip(pageSize * currentPage).Take(pageSize).ToListAsync(),
            TotalItems = await query.CountAsync(),
            CurrentPage = currentPage,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<AllowanceSelectList>> GetAll()
    {
        return await _context.Allowances
            .Where(x => !x.IsDelete)
                .Select(x => new AllowanceSelectList
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .OrderBy(x => x.Name)
                .ToListAsync();
    }

    public async Task<AllowanceViewModel> GetById(int id)
    {
        var item = await _context.Allowances.FindAsync(id);
        return _mapper.Map<AllowanceViewModel>(item);
    }

    public async Task Create(AllowanceViewModel param, int userId)
    {
        var allowance = _mapper.Map<Allowance>(param);

        if (_context.Allowances.Where(u => u.Name == param.Name).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        allowance.UserCreated = userId;
        allowance.UserUpdated = userId;
        _context.Allowances.Add(allowance);
        await _context.SaveChangesAsync();
    }

    public async Task Update(AllowanceViewModel param, int userId)
    {
        var allowance = await _context.Allowances.FindAsync(param.Id);

        if (allowance == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        allowance.Code = param.Code;
        allowance.Name = param.Name;
        allowance.CompanyId = param.CompanyId;

        allowance.UpdatedAt = DateTime.Now;
        allowance.UserUpdated = userId;

        _context.Allowances.Update(allowance);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var allowance = await _context.Allowances.FindAsync(id);
        if (allowance != null)
        {
            allowance.IsDelete = true;
            allowance.DeleteAt = DateTime.Now;
            _context.Allowances.Update(allowance);
            await _context.SaveChangesAsync();
        }
    }

}