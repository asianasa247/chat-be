using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Introduces;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.IntroduceEntities;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject;

namespace ManageEmployee.Services;
public class IntroduceTypeService: Interfaces.Introduces.IIntroduceTypeService
{
    private readonly ApplicationDbContext _context;

    public IntroduceTypeService(ApplicationDbContext context)
    {
        _context = context;
    }
   
    public async Task<IEnumerable<IntroduceType>> GetList()
    {
        return await _context.IntroduceTypes.ToListAsync();
    }

    public async Task<PagingResult<IntroduceType>> GetPaging(PagingRequestModel param)
    {
        var query = _context.IntroduceTypes
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText) || x.Note.Contains(param.SearchText));

        return new PagingResult<IntroduceType>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(IntroduceTypeModel param)
    {
        var introduceType = new IntroduceType
        {
            Code = param.Code,
            Name = param.Name,
            Note = param.Note,
            OrdinalNumber = param.OrdinalNumber,
            Types = param.Types
        };

        try
        {
            // Thêm đối tượng vào DbContext
            await _context.IntroduceTypes.AddAsync(introduceType);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            throw new ErrorException("An error occurred while saving data.");
        }
    }

    public async Task Update(IntroduceType param)
    {
        var stationery = await _context.IntroduceTypes.FindAsync(param.Id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        stationery.Code = param.Code;
        stationery.Name = param.Name;
        stationery.Note = param.Note;
        stationery.OrdinalNumber = param.OrdinalNumber;
        stationery.Types = param.Types;

        _context.IntroduceTypes.Update(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.IntroduceTypes.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.IntroduceTypes.Remove(stationery);
        await _context.SaveChangesAsync();
    }

    public async Task<IntroduceType> GetById(int id)
    {
        var itemOut = await _context.IntroduceTypes.FindAsync(id);
        if (itemOut is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        
        return itemOut;
    }

}
