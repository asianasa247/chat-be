using AutoMapper;
using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.CategoryEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NuGet.Packaging;
using System.Text.Json;
using System.Xml.Linq;

namespace ManageEmployee.Services.Web;

public class WebProductService : IWebProductService
{
    private readonly ApplicationDbContext _context;
    private ApplicationDbContext dbContext;
    private readonly IMapper _mapper;
    private readonly IConnectionStringProvider _connectionStringProvider;
    private readonly IDbContextFactory _dbContextFactory;
    public WebProductService(ApplicationDbContext context, IMapper mapper, IConnectionStringProvider connectionStringProvider , IDbContextFactory dbContextFactory)
    {
        _context = context;
        _mapper = mapper;
        _connectionStringProvider = connectionStringProvider;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<ProductsDetailResponse> GetByIdAsync(int id)
    {
        var good = await _context.Goods.Where(x => x.Id == id && !x.IsDeleted).Select(x => _mapper.Map<ProductsDetailResponse>(x)).FirstOrDefaultAsync();
        if (good is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        // check size
        var goodClassifications = new List<string>();
        if (!string.IsNullOrEmpty(good.Detail2))
        {
            goodClassifications = good.Detail2.Split(";").ToList();
        }
        else
        {
            goodClassifications = good.Detail1.Split(";").ToList();
        }

        var goodChecks = await _context.Goods.Where(x => x.Account == good.Account
                                                        && x.Detail1 == good.Detail1 && x.Detail2.Contains(goodClassifications[0])).ToArrayAsync();

        good.ProductCategoryDetails = new List<ProductCategoryDetailModel>();

        foreach (var check in goodChecks)
        {
            var category = new ProductCategoryDetailModel
            {
                Id = check.Id,
            };

            if (!string.IsNullOrEmpty(good.Detail2))
            {
                goodClassifications = good.Detail2.Split(";").ToList();
            }
            else
            {
                goodClassifications = good.Detail1.Split(";").ToList();
            }
            if (goodClassifications.Count > 1)
            {
                category.Size = goodClassifications[1];
            }

            if (goodClassifications.Count > 2)
            {
                category.Color = goodClassifications[2];
            }
            if (!string.IsNullOrEmpty(category.Color) || !string.IsNullOrEmpty(category.Size))
            {
                good.ProductCategoryDetails.Add(category);
            }
        }


        return good;
    }

    public async Task<Category> GetCategoryByCodeAsync(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return default;
        }

        var category = await _context.Categories.Where(x => x.Code == code && !x.IsDeleted).FirstOrDefaultAsync();
        return category;
    }

    public async Task<CommonWebResponse> GetProduct(ProductSearchModel search)
    {
        if (search.Page < 1)
            search.Page = 1;

        var query = _context.Goods.Where(x => !x.IsDeleted && x.PriceList == "BGC")
            .Where(x => string.IsNullOrEmpty(search.SearchText) ||
                        (!string.IsNullOrEmpty(x.DetailName2) &&
                         x.DetailName2.ToLower().Contains(search.SearchText.ToLower())) ||
                        (!string.IsNullOrEmpty(x.DetailName1) &&
                         x.DetailName1.ToLower().Contains(search.SearchText.ToLower())))
            .Where(x => string.IsNullOrEmpty(search.CategoryCode) ||
                        x.MenuType.ToLower() == search.CategoryCode.ToLower())
            .Where(x => search.AmountFrom == null || x.SalePrice >= search.AmountFrom)
            .Where(x => search.AmountTo == null || x.SalePrice <= search.AmountTo);
        List<Goods> goods = new();
        switch (search.SortType)
        {
            case SortType.INCREASE_PRICE:
                goods = await query.OrderBy(x => x.Price).Skip((search.Page - 1) * search.PageSize).Take(search.PageSize)
            .ToListAsync();
                break;

            case SortType.REDUCE_PRICE:
                goods = await query.OrderByDescending(x => x.Price).Skip((search.Page - 1) * search.PageSize).Take(search.PageSize)
            .ToListAsync();
                break;

            default:
                break;
        }

        var result = new PagingResult<Goods>
        {
            CurrentPage = search.Page,
            PageSize = search.PageSize,
            TotalItems = await query.CountAsync(),
            Data = goods
        };
        return new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = result
        };
    }

    /// <summary>
    /// Danh sách sản phẩm bán chạy
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public async Task<List<Goods>> GetTopProductSell()
    {
        return await _context.Goods.Take(10).ToListAsync();
    }

    /// <summary>
    /// Danh sách sản phẩm theo danh mục
    /// </summary>
    /// <returns></returns>
    public async Task<List<WebProductByCategory>> GetProductCategory()
    {
        var result = new List<WebProductByCategory>();
        var categories = await _context.Categories.Where(x =>
                x.IsDeleted != true && x.Type == (int)CategoryEnum.MenuWeb && string.IsNullOrEmpty(x.CodeParent))
            .Take(10).ToListAsync();
        if (categories?.Count > 0)
        {
            foreach (var category in categories)
            {
                var productByCategory = new WebProductByCategory();
                productByCategory.CategoryName = category.Name;
                productByCategory.CategoryCode = category.Code;
                productByCategory.CategoryImages = category.Image != null
                && category.Image != "" ? JsonSerializer.Deserialize<List<UserTaskFileModel>>(category.Image) : new List<UserTaskFileModel>();

                var categoryWeb = await _context.CategoryStatusWebPeriods.FirstOrDefaultAsync(x => x.CategoryId == category.Id);
                if (categoryWeb is null)
                    continue;
                var goodIds = await _context.CategoryStatusWebPeriodGoods.Where(x => x.CategoryStatusWebPeriodId == categoryWeb.Id).Select(x => x.GoodId).ToListAsync();
                if (goodIds.Any())
                    continue;
                productByCategory.Products = await _context.Goods.Where(g => goodIds.Contains(g.Id))
                    .Take(5).ToListAsync();
                result.Add(productByCategory);
            }
        }

        return result;
    }

    public async Task<List<Goods>> GetProductsByMenuTypeAsync(string menuType)
    {
        return await _context.Goods.Where(x => x.MenuType == menuType && x.Status == 1)
        .OrderBy(x => x.NumberItem ?? int.MaxValue)
        .ToListAsync();
    }

    public async Task<List<GoodsWebs>> GetProductsByMenuWebAsync(string name)
    {
        return await _context.GoodsWebs
        .ToListAsync();
    }

    public async Task<List<GoodsWebs>> GetProductsByMenuTypeAndAdditionAsync(string name)
    {
        var connectionStr = _connectionStringProvider.GetConnectionString(name);
        using var dbContext = _dbContextFactory.GetDbContext(connectionStr);

        // lấy toàn bộ goods trong DB
        var goods = await dbContext.Goods.ToListAsync();

        // lấy toàn bộ goodsWebs (status = 1)
        var goodWebs = await _context.GoodsWebs
            .ToListAsync();

        var newGoods = new List<GoodsWebs>();

        foreach (var item in goods)
        {
            bool exists = goodWebs.Any(g =>
                g.Account == item.Account &&
                g.Detail1 == item.Detail1 &&
                g.Detail2 == item.Detail2
            );
            if (!exists) // nếu goods chưa có thì add
            {
                newGoods.Add(new GoodsWebs
                {
                    Account = item.Account,
                    AccountName = item.AccountName,
                    Delivery = item.Delivery,
                    Detail1 = item.Detail1,
                    Detail2 = item.Detail2,
                    DetailName1 = item.DetailName1,
                    DetailName2 = item.DetailName2,
                    GoodsType = item.GoodsType,
                    Inventory = item.Inventory,
                    IsDeleted = item.IsDeleted,
                    MaxStockLevel = item.MaxStockLevel,
                    MinStockLevel = item.MinStockLevel,
                    MenuType = item.MenuType,
                    Position = item.Position,
                    Price = item.Price,
                    PriceList = item.PriceList,
                    SalePrice = item.SalePrice,
                    Status = item.Status,
                    Warehouse = item.Warehouse,
                    WarehouseName = item.WarehouseName,
                    Image1 = item.Image1,
                    Image2 = item.Image2,
                    Image3 = item.Image3,
                    Image4 = item.Image4,
                    Image5 = item.Image5,
                    WebGoodNameVietNam = item.WebGoodNameVietNam,
                    WebPriceVietNam = item.WebPriceVietNam,
                    WebDiscountVietNam = item.WebDiscountVietNam,
                    TitleVietNam = item.TitleVietNam,
                    ContentVietNam = item.ContentVietNam,
                    WebGoodNameKorea = item.WebGoodNameKorea,
                    WebPriceKorea = item.WebPriceKorea,
                    WebDiscountKorea = item.WebDiscountKorea,
                    TitleKorea = item.TitleKorea,
                    ContentKorea = item.ContentKorea,
                    WebGoodNameEnglish = item.WebGoodNameEnglish,
                    WebPriceEnglish = item.WebPriceEnglish,
                    WebDiscountEnglish = item.WebDiscountEnglish,
                    TitleEnglish = item.TitleEnglish,
                    ContentEnglish = item.ContentEnglish,
                });
            }
        }

        // Nếu có item mới thì insert
        if (newGoods.Any())
        {
            await dbContext.GoodsWebs.AddRangeAsync(newGoods);
            await dbContext.SaveChangesAsync();
        }

        return goodWebs;
    }





    public async Task<List<ProductsByMenuTypeResponse>> GetProductsByMenuTypeAsyncV2(string menuType)
    {
        //var dbAddtions= _context.AdditionWebs.ToListAsync();

        var goods = await _context.Goods.Where(x => x.MenuType == menuType && x.Status == 1)
        .OrderBy(x => x.NumberItem ?? int.MaxValue)
        .Select(x => _mapper.Map<ProductsByMenuTypeResponse>(x))
        .ToListAsync();
        //foreach (var item in dbAddtions.Result)
        //{
        //    var connectionStr = _connectionStringProvider.GetConnectionString(item.DbName);

        //    dbContext = _dbContextFactory.GetDbContext(connectionStr);
        //    goods.AddRange(await dbContext.Goods.Where(x => x.MenuType == menuType && x.Status == 1).OrderBy(x => x.NumberItem ?? int.MaxValue)
        //.Select(x => _mapper.Map<ProductsByMenuTypeResponse>(x))
        //.ToListAsync());
        //}

        var goodOuts = new List<ProductsByMenuTypeResponse>();
        foreach (var good in goods)
        {
            var goodClassifications = new List<string>();
            if (!string.IsNullOrEmpty(good.Detail2))
            {
                goodClassifications = good.Detail2.Split(";").ToList();
            }
            else
            {
                goodClassifications = good.Detail1.Split(";").ToList();
            }
            
            good.Detail2 = goodClassifications[0];

            var goodCheck = goodOuts.FirstOrDefault(x => x.Account == good.Account
                                                                && x.Detail1 == good.Detail1 && x.Detail2.Contains(goodClassifications[0]));
            var ProductCategoryDetail = new ProductCategoryDetailModel();
            if (goodClassifications.Count > 1)
            {
                ProductCategoryDetail.Size = goodClassifications[1];
            }

            if (goodClassifications.Count > 2)
            {
                ProductCategoryDetail.Color = goodClassifications[2];
            }
            ProductCategoryDetail.Id = good.Id;

            if (goodCheck is null)
            {
                if (good.ProductCategoryDetails is null)
                {
                    good.ProductCategoryDetails = new List<ProductCategoryDetailModel>();
                }

                if (!string.IsNullOrEmpty(ProductCategoryDetail.Color) || !string.IsNullOrEmpty(ProductCategoryDetail.Size))
                {
                    good.ProductCategoryDetails.Add(ProductCategoryDetail);
                }
                goodOuts.Add(good);
            }
            else
            {
                if(goodCheck.ProductCategoryDetails is null)
                {
                    goodCheck.ProductCategoryDetails = new List<ProductCategoryDetailModel>();
                }
                if (!string.IsNullOrEmpty(ProductCategoryDetail.Color) || !string.IsNullOrEmpty(ProductCategoryDetail.Size))
                {
                    goodCheck.ProductCategoryDetails.Add(ProductCategoryDetail);
                }
            }
        }
        return goodOuts;
    }
 
    public async Task<ProductPagging> GetProductsPagging(int pageNum = 0, int pageSize = 10, string q = "")
    { var goods = _context.Goods.Where(t => (string.IsNullOrEmpty(q)
        || t.TitleVietNam.Contains(q)
        || t.TitleEnglish.Contains(q)
        || t.TitleKorea.Contains(q)
        || t.WebGoodNameEnglish.Contains(q)
        || t.WebGoodNameKorea.Contains(q)
        || t.WebGoodNameVietNam.Contains(q)
        ));
       var count =goods.Count();
        var lst = await goods.Skip(pageNum*pageSize).Take(pageSize).ToListAsync();
        return new ProductPagging()
        {
            Count = count,
            Products = lst
        };
    }
    public async Task<PagingResult<Goods>> GetProductsByMenuTypeAsync(string menuType, PagingRequestModel param, bool isService)
    {
        if (param.Page < 0)
            param.Page = 0;
        var query = _context.Goods.Where(x => (string.IsNullOrEmpty(menuType) || x.MenuType == menuType) && x.Status == 1 && x.IsService == isService);


        var data = new PagingResult<Goods>()
        {
            Data = await query.Skip(param.Page* param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync(),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };

        return data;
    }
}
