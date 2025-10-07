using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Services.CompanyServices;
using ManageEmployee.Services.GoodsServices;
using ManageEmployee.Services.Interfaces.AdditionWebServices;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.AdditionWebServices;

public class AdditionWebGoodsService : AdditionWebServiceBase, IAdditionWebGoodsService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public AdditionWebGoodsService(IDbContextFactory dbContextFactory, IMapper mapper, ApplicationDbContext dbContext)
        : base(dbContextFactory)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<AdditionWebGoodsPagingResult> GetAllGoodsByWebId(int webId, SearchViewModel param, int year)
    {
        var additionWeb = await _dbContext.AdditionWebs.FirstOrDefaultAsync(x => x.Id == webId)
            ?? throw new Exception("Không tìm thấy website");

        if (string.IsNullOrEmpty(additionWeb.DbName))
            throw new Exception("Website chưa cấu hình database");

        using var currentDbContext = GetApplicationDbContext(additionWeb);

        var goodsService = new GoodsService(
            currentDbContext,
            null,
            _mapper,
            null
        );

        var result = await goodsService.GetPaging(param, year);
        var goodsIds = result.Goods.Select(x => x.Id).ToList();

        if (goodsIds.Count == 0)
        {
            return new AdditionWebGoodsPagingResult
            {
                TotalItems = result.TotalItems,
                pageIndex = result.pageIndex,
                PageSize = result.PageSize,
                Goods = new List<AdditionWebGoodsResult>()
            };
        }

        var goodsIdsSelected = await _dbContext.AdditionWebGoods
                                .Where(x => goodsIds.Contains(x.GoodId) && x.AdditionWebId == webId)
                                .Select(x => x.GoodId)
                                .ToListAsync();

        return ToAdditionWebGoods(result, additionWeb, goodsIdsSelected);
    }

    public async Task<int> SaveGoodsToAdditionWebAsync(int webId, AdditionWebGoodsRequestModel request)
    {
        _ = await _dbContext.AdditionWebs.FirstOrDefaultAsync(x => x.Id == webId)
            ?? throw new Exception("Không tìm thấy website");

        var candidates = request.GoodsIds.Distinct().ToList();

        if (candidates.Count == 0) return 0;

        var existed = await _dbContext.AdditionWebGoods
            .Where(x => x.AdditionWebId == webId && candidates.Contains(x.GoodId))
            .Select(x => x.GoodId)
            .ToListAsync();

        var toAdd = candidates.Except(existed).ToList();
        if (toAdd.Count == 0) return 0;

        var rows = toAdd.Select(id => new AdditionWebGoods
        {
            AdditionWebId = webId,
            GoodId = id
        });

        await _dbContext.AdditionWebGoods.AddRangeAsync(rows);
        await _dbContext.SaveChangesAsync();

        return toAdd.Count;
    }

    public async Task<AdditionWebGoodsPagingResult> GetAllGoodsSelectedByWebId(int webId, SearchViewModel param, int year)
    {
        var pageIndex = param.Page <= 0 ? 1 : param.Page;
        var pageSize = param.Page <= 0 ? 20 : param.PageSize;

        var additionWeb = await _dbContext.AdditionWebs.FirstOrDefaultAsync(x => x.Id == webId)
            ?? throw new Exception("Không tìm thấy website");

        if (string.IsNullOrWhiteSpace(additionWeb.DbName))
            throw new Exception("Website chưa cấu hình database");

        var idsQuery = _dbContext.AdditionWebGoods
            .Where(x => x.AdditionWebId == webId)
            .OrderBy(x => x.GoodId)
            .Select(x => x.GoodId);

        var totalItems = await idsQuery.CountAsync();

        var goodsId = await idsQuery
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (goodsId.Count == 0)
        {
            return new AdditionWebGoodsPagingResult
            {
                TotalItems = totalItems,
                pageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        using var currentDbContext = GetApplicationDbContext(additionWeb);

        var goodsService = new GoodsService(
            currentDbContext,
            null,
            _mapper,
            null
        );

        var webSearchViewModel = _mapper.Map<WebSearchViewModel>(param);

        webSearchViewModel.GoodsIds = goodsId;

        var result = await goodsService.GetPaging(webSearchViewModel, year);

        return ToAdditionWebGoods(result, additionWeb, goodsId);
    }

    public async Task RemoveGoods(int webId, int goodsId)
    {
        var additionWebGoods = await _dbContext.AdditionWebGoods
            .FirstOrDefaultAsync(x => x.AdditionWebId == webId && x.GoodId == goodsId);

        if (additionWebGoods == null)
        {
            return;
        }

        _dbContext.AdditionWebGoods.Remove(additionWebGoods);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveGoodsSelected(int webId, List<int> goodsIds)
    {
        if (goodsIds == null || goodsIds.Count == 0) return;

        var additionWebGoods = await _dbContext.AdditionWebGoods
            .Where(x => x.AdditionWebId == webId && goodsIds.Contains(x.GoodId))
            .ToListAsync();

        if (additionWebGoods.Count == 0)
            return;

        _dbContext.AdditionWebGoods.RemoveRange(additionWebGoods);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<AdditionWebSelectedGroupResult>> GetAllGoodsSelectedAsync(SearchViewModel param, int year)
    {
       var additionWeb = await _dbContext.AdditionWebs
            .Join(
            _dbContext.AdditionWebGoods,
            x => x.Id,
            x => x.AdditionWebId,
            (web, webGoods) => new
            {
                AdditionWeb = web,
                GoodsId = webGoods.GoodId
            }).ToListAsync();

        var additionWebDict = additionWeb.GroupBy(x => x.AdditionWeb)
             .ToDictionary(x => x.Key,
                           x => x.Select(s => s.GoodsId));

        var additionWebSelectedTask = new List<Task<AdditionWebSelectedGroupResult>>();

        foreach (var web in additionWebDict)
        {
            var goodsIds = web.Value
                .Distinct()
                .ToList();

            var currentWeb = web.Key;

            additionWebSelectedTask.Add(GetAdditionWebSelected(goodsIds, currentWeb, param, year));
        }

        var results = await Task.WhenAll(additionWebSelectedTask);

        return results.ToList();
    }

    private async Task<AdditionWebSelectedGroupResult> GetAdditionWebSelected(List<int> goodsIds, AdditionWeb currentWeb, SearchViewModel param, int year)
    {
        using var currentDbContext = GetApplicationDbContext(currentWeb);

        var companyService = new CompanyService(currentDbContext, null);

        var goodsService = new GoodsService(
            currentDbContext,
            null,
            _mapper,
            null
        );

        var webSearch = _mapper.Map<WebSearchViewModel>(param);

        webSearch.GoodsIds = goodsIds;
        webSearch.Page = 1;
        webSearch.PageSize = goodsIds.Count;

        var additionWebs = await goodsService.GetPaging(webSearch, year);

        var additionWebGoods = ToAdditionWebGoods(additionWebs, currentWeb, goodsIds);

        var companyInfo = await companyService.GetCompany();

        return new AdditionWebSelectedGroupResult
        {
            AdditionWebId = currentWeb.Id,
            UrlWeb = currentWeb.UrlWeb,
            CompanyInfo = ToCompanyResult(companyInfo, currentWeb),
            Goods = additionWebGoods.Goods
        };
    }

    private AdditionWebGoodsPagingResult ToAdditionWebGoods(GoodsPagingResult goodsPagingResult, AdditionWeb additionWeb, List<int> goodsIdsSelected)
    {
        var additionWebGoodsPagingResult = _mapper.Map<AdditionWebGoodsPagingResult>(goodsPagingResult);

        additionWebGoodsPagingResult.Goods = goodsPagingResult.Goods.Select(s =>
        {
            var goods = _mapper.Map<AdditionWebGoodsResult>(s);
            goods.IsSelected = goodsIdsSelected != null && goodsIdsSelected.Contains(s.Id);

            if (string.IsNullOrWhiteSpace(additionWeb.ImageHost))
            {
                return goods;
            }

            goods.FullImage1 = !string.IsNullOrWhiteSpace(s.Image1) ? $"{additionWeb.ImageHost}/{s.Image1}" : string.Empty;
            goods.FullImage2 = !string.IsNullOrWhiteSpace(s.Image2) ? $"{additionWeb.ImageHost}/{s.Image2}" : string.Empty;
            goods.FullImage3 = !string.IsNullOrWhiteSpace(s.Image3) ? $"{additionWeb.ImageHost}/{s.Image3}" : string.Empty;
            goods.FullImage4 = !string.IsNullOrWhiteSpace(s.Image4) ? $"{additionWeb.ImageHost}/{s.Image4}" : string.Empty;
            goods.FullImage5 = !string.IsNullOrWhiteSpace(s.Image5) ? $"{additionWeb.ImageHost}/{s.Image5}" : string.Empty;

            return goods;
        });

        return additionWebGoodsPagingResult;
    }

    private AdditionWebCompanyResult ToCompanyResult(Company company, AdditionWeb additionWeb)
    {
        if(company == null)
        {
            return null;
        }

        var result = _mapper.Map<AdditionWebCompanyResult>(company);
        if(string.IsNullOrWhiteSpace(additionWeb.ImageHost))
        {
            return result;
        }

        result.FullLogo = !string.IsNullOrWhiteSpace(result.FileLogo) ? $"{additionWeb.ImageHost}/{result.FileLogo}" : string.Empty;

        return result;
    }
}
