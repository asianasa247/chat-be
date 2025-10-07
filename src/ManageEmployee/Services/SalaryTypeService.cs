using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SalaryModels;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Services.Interfaces.Salarys;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class SalaryTypeService: ISalaryTypeService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public SalaryTypeService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<SalaryTypeModel>> GetPaging(PagingRequestModel form)
    {
        var query = _context.SalaryTypes
                .Select(x => _mapper.Map<SalaryTypeModel>(x));
        var result = new PagingResult<SalaryTypeModel>()
        {
            CurrentPage = form.Page,
            PageSize = form.PageSize,
            Data = await query.Skip((form.Page) * form.PageSize).Take(form.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
        return result;
    }

    public async Task Create(SalaryTypeModel request)
    {
        var item = _mapper.Map<SalaryType>(request);
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        _context.SalaryTypes.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<SalaryTypeModel> GetById(int id)
    {
        var item = await _context.SalaryTypes.FindAsync(id);
        return _mapper.Map<SalaryTypeModel>(item);
    }

    public async Task Update(SalaryTypeModel request)
    {
        var item = await _context.SalaryTypes.FindAsync(request.Id);
        item.AmountSpentMonthly = request.AmountSpentMonthly;
        item.Note = request.Note;
        item.AmountAtTheEndYear = request.AmountAtTheEndYear;
        item.AmountSpent = request.AmountSpent;
        item.Code = request.Code;
        item.Name = request.Name;
        item.UpdatedAt = DateTime.Now;
        _context.SalaryTypes.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _context.SalaryTypes.FindAsync(id);
        _context.SalaryTypes.Remove(item);
        await _context.SaveChangesAsync();
    }
}
