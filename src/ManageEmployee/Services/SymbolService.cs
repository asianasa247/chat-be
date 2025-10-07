using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Symbols;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class SymbolService : ISymbolService
{
    private readonly ApplicationDbContext _context;

    public SymbolService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<Symbol>> GetAll(PagingRequestModel param)
    {
        var query = _context.Symbols.Where(x => string.IsNullOrEmpty(param.SearchText) ||  x.Name.ToLower().Contains(param.SearchText));
        
        return new PagingResult<Symbol>
        {
            Data = await query.Skip(param.PageSize * (param.Page - 1)).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync(),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public IEnumerable<Symbol> GetAll()
    {
        var query = _context.Symbols;
        return query
                .ToList();
    }

    public Symbol GetById(int id)
    {
        return _context.Symbols.Find(id);
    }

    public Symbol Create(Symbol param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Symbols.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(Symbol request)
    {
        var symbol = _context.Symbols.Find(request.Id);

        if (symbol == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        _context.Entry(symbol).CurrentValues.SetValues(request);
        _context.Symbols.Update(symbol);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var position = _context.Symbols.Find(id);
        if (position != null)
        {
            _context.Symbols.Remove(position);
            _context.SaveChanges();
        }
    }
}