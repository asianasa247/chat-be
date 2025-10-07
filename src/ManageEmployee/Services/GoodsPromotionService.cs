using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services;
public class GoodsPromotionService: IGoodsPromotionService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;

    public GoodsPromotionService(ApplicationDbContext context, IMapper mapper, IFileService fileService, IChartOfAccountV2Service chartOfAccountV2Service)
    {
        _context = context;
        _mapper = mapper;
        _fileService = fileService;
        _chartOfAccountV2Service = chartOfAccountV2Service;
    }
    public async Task<IEnumerable<GoodsPromotionGetListModel>> GetList()
    {
        return await _context.GoodsPromotions.Select(x => _mapper.Map<GoodsPromotionGetListModel>(x)).ToListAsync();
    }

    public async Task<PagingResult<GoodsPromotionModel>> GetPaging(PagingRequestModel param)
    {
        var query = _context.GoodsPromotions
                    .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.Contains(param.SearchText) || x.Note.Contains(param.SearchText) || x.Code.Contains(param.SearchText))
                    .Select(x => _mapper.Map<GoodsPromotionModel>(x));

        return new PagingResult<GoodsPromotionModel>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            Data = await query.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync(),
            TotalItems = await query.CountAsync()
        };
    }

    public async Task Create(GoodsPromotionSetterModel param)
    {
        var stationery = _mapper.Map<GoodsPromotion>(param);
        stationery.FileLink = "";
        if (param.File != null && param.File.Any())
        {
            stationery.FileLink = JsonConvert.SerializeObject(param.File).ToString();
        }
        await _context.GoodsPromotions.AddAsync(stationery);
        await _context.SaveChangesAsync();
        var itemDetails = new List<GoodsPromotionDetail>();

        foreach(var item in param.Items)
        {
            var itemDetail = _mapper.Map<GoodsPromotionDetail>(item);
            itemDetail.GoodsPromotionId = stationery.Id;
            itemDetails.Add(itemDetail);
        }
        await _context.GoodsPromotionDetails.AddRangeAsync(itemDetails);
        await _context.SaveChangesAsync();
    }

    public async Task Update(GoodsPromotionSetterModel param)
    {
        var stationery = await _context.GoodsPromotions.FindAsync(param.Id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        stationery = _mapper.Map<GoodsPromotion>(param);
        stationery.UpdatedAt = DateTime.Now;

        stationery.FileLink = "";
        if (param.File != null && param.File.Any())
        {
            stationery.FileLink = JsonConvert.SerializeObject(param.File).ToString();
        }

        _context.GoodsPromotions.Update(stationery);

        var itemDetailDels = await _context.GoodsPromotionDetails.Where(x => x.GoodsPromotionId == param.Id).ToListAsync();
        _context.GoodsPromotionDetails.RemoveRange(itemDetailDels);

        var itemDetails = new List<GoodsPromotionDetail>();

        foreach (var item in param.Items)
        {
            var itemDetail = _mapper.Map<GoodsPromotionDetail>(item);
            itemDetail.GoodsPromotionId = stationery.Id;
            itemDetails.Add(itemDetail);
        }
        await _context.GoodsPromotionDetails.AddRangeAsync(itemDetails);

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var stationery = await _context.GoodsPromotions.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        _context.GoodsPromotions.Remove(stationery);
        var itemDetailDels = await _context.GoodsPromotionDetails.Where(x => x.GoodsPromotionId == id).ToListAsync();
        _context.GoodsPromotionDetails.RemoveRange(itemDetailDels);

        await _context.SaveChangesAsync();
    }

    public async Task<GoodsPromotionGetDetailModel> GetById(int id, int year)
    {
        var stationery = await _context.GoodsPromotions.FindAsync(id);
        if (stationery is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var itemOut = _mapper.Map<GoodsPromotionGetDetailModel>(stationery);
        itemOut.File = JsonConvert.DeserializeObject<List<FileDetailModel>>(stationery.FileLink);

        var itemDetails = await _context.GoodsPromotionDetails.Where(x => x.GoodsPromotionId == id).ToListAsync();
        itemOut.Items = new List<GoodsPromotionDetailModel>();
        foreach(var item in itemDetails)
        {
            var itemDetail = _mapper.Map<GoodsPromotionDetailModel>(item);
            itemDetail.AccountObj = await _chartOfAccountV2Service.FindAccount(item.Account, string.Empty, year);
            itemDetail.Detail1Obj = await _chartOfAccountV2Service.FindAccount(item.Detail1, item.Account, year);
            if (!string.IsNullOrEmpty(item.Detail2))
            {
                itemDetail.Detail2Obj = await _chartOfAccountV2Service.FindAccount(item.Detail2, item.Account + ":" + item.Detail1, year);
            }
            itemOut.Items.Add(itemDetail);
        }
        return itemOut;
    }

    public async Task<IEnumerable<GoodsPromotionDetailForSaleModel>> GetListForSale()
    {
        var date = DateTime.Today;
        var goodsPromotions = await _context.GoodsPromotions.Where(x => x.FromAt <= date && x.ToAt >= date).ToListAsync();
        var goodsPromotionIds = goodsPromotions.Select(x => x.Id).ToList();
        var goodsPromotionDetails = await _context.GoodsPromotionDetails.Where(x => goodsPromotionIds.Contains(x.GoodsPromotionId)).ToListAsync();

        var listOut = new List<GoodsPromotionDetailForSaleModel>();
        foreach (var goodsPromotionDetail in goodsPromotionDetails)
        {
            var goodsPromotion = goodsPromotions.FirstOrDefault(x => x.Id == goodsPromotionDetail.GoodsPromotionId);
            listOut.Add(new GoodsPromotionDetailForSaleModel
                {
                    Id = goodsPromotionDetail.Id,
                    Standard = goodsPromotionDetail.Standard,
                    Code = goodsPromotion.Code,
                    Name = goodsPromotion.Name,
                    Qty = goodsPromotionDetail.Qty,          
                    Discount = goodsPromotionDetail.Discount,
                    Note = goodsPromotion.Note
            });
        }
        return listOut;
    }
}
