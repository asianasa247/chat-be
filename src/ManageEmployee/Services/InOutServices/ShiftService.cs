using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.InOutServices;
public class ShiftService: IShiftService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ShiftService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<ShiftModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.Shifts;
        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize)
            .Select(x => _mapper.Map<ShiftModel>(x)).ToListAsync();
        var totalItem = await query.CountAsync();

        return new PagingResult<ShiftModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<ShiftModel> GetDetail(int id)
    {
        return await _context.Shifts.Where(x => x.Id == id).Select(x => _mapper.Map<ShiftModel>(x)).FirstOrDefaultAsync();
    }

    public async Task Create(ShiftModel form)
    {
        var shift = _mapper.Map<Shift>(form);
        await _context.AddAsync(shift);
        await _context.SaveChangesAsync();
    }

    public async Task Update(ShiftModel form)
    {
        var shift = await _context.Shifts.FindAsync(form.Id);
        shift.TimeIn = form.TimeIn;
        shift.TimeOut = form.TimeOut;
        shift.SymbolId = form.SymbolId;
        _context.Update(shift);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var shift = await _context.Shifts.FindAsync(id);
        if (shift is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        _context.Remove(shift);
        await _context.SaveChangesAsync();
    }
}
