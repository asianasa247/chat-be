using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsQuotaServices;
public class GoodsQuotaStepService: IGoodsQuotaStepService
{
    private readonly ApplicationDbContext _context;

    public GoodsQuotaStepService(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }

    public async Task<GoodsQuotaStep> Create(GoodsQuotaStepModel request)
    {
        var goodsQuotaStep = new GoodsQuotaStep
        {
            Code = request.Code,
            Name = request.Name,
            UserIds = string.Join(",", request.UserIds),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        await _context.GoodsQuotaSteps.AddAsync(goodsQuotaStep);
        await _context.SaveChangesAsync();
        return goodsQuotaStep;
    }

    public async Task Delete(int id)
    {
        var itemDelete = await _context.GoodsQuotaSteps.FindAsync(id);
        if (itemDelete != null)
        {
            _context.GoodsQuotaSteps.Remove(itemDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<GoodsQuotaStepListModel>> GetAll()
    {
        return await _context.GoodsQuotaSteps.Select(x => new GoodsQuotaStepListModel
        {
            Id = x.Id,
            Code = x.Code,
            Name= x.Name,
        }).ToListAsync();
    }

    public async Task<PagingResult<GoodsQuotaStep>> GetPaging(PagingRequestModel searchRequest)
    {
        var news = _context.GoodsQuotaSteps
                                     .Where(x => string.IsNullOrEmpty(searchRequest.SearchText) || x.Code.ToLower().Contains(searchRequest.SearchText.ToLower()));

        return new PagingResult<GoodsQuotaStep>()
        {
            CurrentPage = searchRequest.Page,
            PageSize = searchRequest.PageSize,
            TotalItems = await news.CountAsync(),
            Data = await news.Skip((searchRequest.Page) * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync()
        };
    }

    public async Task Update(GoodsQuotaStepModel request)
    {
        var itemUpdate = await _context.GoodsQuotaSteps.FindAsync(request.Id);
        if (itemUpdate == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        itemUpdate.Code = request.Code;
        itemUpdate.Name = request.Name;
        itemUpdate.UserIds = string.Join(",", request.UserIds);
        itemUpdate.UpdatedAt = DateTime.Now;

        _context.GoodsQuotaSteps.Update(itemUpdate);
        await _context.SaveChangesAsync();
    }

    public async Task<GoodsQuotaStepModel> GetDetail(int id)
    {
        var item = await _context.GoodsQuotaSteps.FindAsync(id);
        return new GoodsQuotaStepModel
        {
            Id = id,
            Code = item.Code,
            Name = item.Name,
            UserIds = item.UserIds?.Split(',').Select(x => int.Parse(x)).ToList()
        };
    }
}
