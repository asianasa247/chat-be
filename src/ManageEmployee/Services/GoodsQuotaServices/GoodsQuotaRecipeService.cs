using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsQuotaServices;
public class GoodsQuotaRecipeService : IGoodsQuotaRecipeService
{
    private readonly ApplicationDbContext _context;

    public GoodsQuotaRecipeService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task Create(GoodsQuotaRecipe request)
    {
        await _context.GoodsQuotaRecipes.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var itemDelete = await _context.GoodsQuotaRecipes.FindAsync(id);
        if (itemDelete != null)
        {
            _context.GoodsQuotaRecipes.Remove(itemDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<GoodsQuotaRecipe>> GetAll()
    {
        return await _context.GoodsQuotaRecipes.ToListAsync();
    }

    public async Task<PagingResult<GoodsQuotaRecipe>> GetPaging(PagingRequestModel searchRequest)
    {
        var news = _context.GoodsQuotaRecipes
                                     .Where(x => string.IsNullOrEmpty(searchRequest.SearchText) || x.Code.ToLower().Contains(searchRequest.SearchText.ToLower()));

        return new PagingResult<GoodsQuotaRecipe>()
        {
            CurrentPage = searchRequest.Page,
            PageSize = searchRequest.PageSize,
            TotalItems = await news.CountAsync(),
            Data = await news.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync()
        };
    }

    public async Task Update(GoodsQuotaRecipe request)
    {
        var itemUpdate = await _context.GoodsQuotaRecipes.FindAsync(request.Id);
        if (itemUpdate == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        itemUpdate.Code = request.Code;
        itemUpdate.Name = request.Name;
        _context.GoodsQuotaRecipes.Update(itemUpdate);
        await _context.SaveChangesAsync();
    }
}
