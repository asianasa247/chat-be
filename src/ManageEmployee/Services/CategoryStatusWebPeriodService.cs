using AutoMapper;
using Common.Errors;
using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Categories;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class CategoryStatusWebPeriodService : ICategoryStatusWebPeriodService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoryStatusWebPeriodService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<CategoryStatusWebPeriodModel>> GetAll(int pageIndex, int pageSize)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var categories = _context.CategoryStatusWebPeriods.Include(x => x.Category).OrderBy(x => x.FromAt);

            return new PagingResult<CategoryStatusWebPeriodModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = await categories.CountAsync(),
                Data = categories.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable()
                .Select(x =>
                {
                    var cate = _mapper.Map<CategoryStatusWebPeriodModel>(x);
                    cate.CategoryName = x.Category?.Name;
                    return cate;
                }).ToList()
            };
        }
        catch
        {
            return new PagingResult<CategoryStatusWebPeriodModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<CategoryStatusWebPeriodModel>()
            };
        }
    }

    public async Task<IEnumerable<CategoryStatusWebPeriod>> GetAll()
    {
        return await _context.CategoryStatusWebPeriods.Where(x => x.FromAt > DateTime.Now).OrderBy(x => x.FromAt).ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetListCategoryStatusWeb()
    {
        return await _context.Categories.Where(x => !x.IsDeleted && (x.Type == (int)CategoryEnum.MenuWeb || x.Type == (int)CategoryEnum.MenuWebOnePage || x.Type == (int)CategoryEnum.DetailNameGood)).OrderBy(x => x.NumberItem).ToListAsync();
    }

    public async Task<CategoryStatusWebPeriodModel> GetById(int id)
    {
        var result = await _context.CategoryStatusWebPeriods.AsNoTracking()
            .Where(x => x.Id == id)
            .Include(x => x.CategoryStatusWebPeriodGoods)
            .ThenInclude(x => x.Goods)
            .OrderBy(x => x.FromAt)
            .Select(x => new CategoryStatusWebPeriodModel()
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                FromAt = x.FromAt,
                ToAt = x.ToAt,
                Items = x.CategoryStatusWebPeriodGoods.Select(p => new CategoryStatusWebPeriodGoodShowWebModel
                {
                    Id = p.Id,
                    GoodId = p.Goods.Id,
                    Code = p.Goods.Detail2.DefaultIfNullOrEmpty(p.Goods.Detail1),
                    Name = p.Goods.DetailName2.DefaultIfNullOrEmpty(p.Goods.DetailName1),
                    Image1 = p.Goods.Image1,
                    Image2 = p.Goods.Image2.FilePathAsUrl(),
                    Price = p.Goods.Price,
                    SalePrice = p.Goods.SalePrice,
                }).ToList()
            }).FirstOrDefaultAsync();

        if (result is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        return result;
    }

    public async Task Create(CategoryStatusWebPeriodModel form)
    {
        var category = _mapper.Map<CategoryStatusWebPeriod>(form);
        _context.CategoryStatusWebPeriods.Add(category);
        category.CategoryId = form.CategoryId;

        await _context.SaveChangesAsync();
        if (form.Items != null && form.Items.Count > 0)
        {
            foreach (var categoryGood in form.Items)
            {
                CategoryStatusWebPeriodGood item = new CategoryStatusWebPeriodGood()
                {
                    Id = 0,
                    CategoryStatusWebPeriodId = category.Id,
                    GoodId = categoryGood.GoodId
                };
                _context.CategoryStatusWebPeriodGoods.Add(item);
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task Update(CategoryStatusWebPeriodModel form)
    {
        var category = await _context.CategoryStatusWebPeriods.FirstOrDefaultAsync(x => x.Id == form.Id);
        if (category is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        var categoryGoods = await _context.CategoryStatusWebPeriodGoods.Where(x => x.CategoryStatusWebPeriodId == form.Id).ToListAsync();
        _context.RemoveRange(categoryGoods);
        if (form.Items != null && form.Items.Count > 0)
        {
            foreach (var categoryGood in form.Items)
            {
                CategoryStatusWebPeriodGood item = new CategoryStatusWebPeriodGood()
                {
                    Id = 0,
                    CategoryStatusWebPeriodId = form.Id,
                    GoodId = categoryGood.GoodId
                };
                _context.Add(item);
            }
        }
        category.FromAt = form.FromAt;
        category.ToAt = form.ToAt;
        category.CategoryId = form.CategoryId;
        category.UserId = form.UserId;
        _context.CategoryStatusWebPeriods.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var category = await _context.CategoryStatusWebPeriods.FirstOrDefaultAsync(x => x.Id == id);
        if (category is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        _context.CategoryStatusWebPeriods.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryStatusWebPeriodGoodShowWebModel>> GetGoodShowWeb(string code)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Code == code && (x.Type == (int)CategoryEnum.MenuWeb || x.Type == (int)CategoryEnum.MenuWebOnePage));
        if (category is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        var categoryWeb = await _context.CategoryStatusWebPeriods.FirstOrDefaultAsync(x => x.CategoryId == category.Id);
        if (categoryWeb is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

        return await (from cat in _context.CategoryStatusWebPeriodGoods
                      join good in _context.Goods on cat.GoodId equals good.Id
                      where cat.CategoryStatusWebPeriodId == categoryWeb.Id
                      select new CategoryStatusWebPeriodGoodShowWebModel()
                      {
                          Id = good.Id,
                          Code = !string.IsNullOrEmpty(good.Detail2) ? good.Detail2 : good.Detail1,
                          Name = !string.IsNullOrEmpty(good.DetailName2) ? good.DetailName2 : good.DetailName1,
                          Image1 = good.Image1,
                          Price = good.Price,
                          SalePrice = good.SalePrice,
                      }
            )
        .Take(10).ToListAsync();
    }

    public async Task<CategoryStatusWebPeriodModel> GetDealsOfDay()
    {
        var current = DateTime.Now.Date;
        var promotion = await _context.CategoryStatusWebPeriods.AsNoTracking()
            .Where(x => current >= x.FromAt.Date && current <= x.ToAt.Date)
            .Include(x => x.CategoryStatusWebPeriodGoods)
            .ThenInclude(x => x.Goods)
            .OrderBy(x => x.FromAt)
            .Select(x => new CategoryStatusWebPeriodModel()
            {
                FromAt = x.FromAt,
                ToAt = x.ToAt,
                Items = x.CategoryStatusWebPeriodGoods.Select(p => new CategoryStatusWebPeriodGoodShowWebModel
                {
                    Id = p.Id,
                    GoodId = p.Goods.Id,
                    Code = p.Goods.Detail2.DefaultIfNullOrEmpty(p.Goods.Detail1),
                    Name = p.Goods.DetailName2.DefaultIfNullOrEmpty(p.Goods.DetailName1),
                    Price = p.Goods.Price,
                    SalePrice = p.Goods.SalePrice,
                    Image1 = p.Goods.Image1.FilePathAsUrl(),
                    Image2 = p.Goods.Image2.FilePathAsUrl(),
                }).ToList()
            }).FirstOrDefaultAsync();
        return promotion;
    }
}