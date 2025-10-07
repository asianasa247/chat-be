using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.ConfigurationEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Configs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class ConfigDiscountService: IConfigDiscountService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ConfigDiscountService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<ConfigDiscountModel>> GetPaging(PagingRequestModel form)
    {
        var query = _context.ConfigDiscounts
                .Select(x => _mapper.Map<ConfigDiscountModel>(x));
        var result = new PagingResult<ConfigDiscountModel>()
        {
            CurrentPage = form.Page,
            PageSize = form.PageSize,
            Data = await query.Skip((form.Page) * form.PageSize).Take(form.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
        return result;
    }

    public async Task Create(ConfigDiscountModel request)
    {
        var item = _mapper.Map<ConfigDiscount>(request);
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        _context.ConfigDiscounts.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<ConfigDiscountModel> GetById(int id)
    {
        var item = await _context.ConfigDiscounts.FindAsync(id);
        return _mapper.Map<ConfigDiscountModel>(item);
    }

    public async Task Update(ConfigDiscountModel request)
    {
        var item = await _context.ConfigDiscounts.FindAsync(request.Id);
        if ((item.Code == ConfigDiscountConst.CustomerOld || item.Code == ConfigDiscountConst.CustomerNew)
            && item.Code != request.Code)
        {
            throw new ErrorException(ErrorMessages.AccessDenined);
        }

        item.PositionDetailId = request.PositionDetailId;
        item.Note = request.Note;
        item.Code = request.Code;
        item.PercentAdvanceDiscountMonth = request.PercentAdvanceDiscountMonth;
        item.DiscountReceivedMonth = request.DiscountReceivedMonth;
        item.DiscountReceivedYear = request.DiscountReceivedYear;
        item.UpdatedAt = DateTime.Now;
        _context.ConfigDiscounts.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _context.ConfigDiscounts.FindAsync(id);
        _context.ConfigDiscounts.Remove(item);
        await _context.SaveChangesAsync();
    }
}
