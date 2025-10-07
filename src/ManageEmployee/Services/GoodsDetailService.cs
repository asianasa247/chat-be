using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class GoodsDetailService : IGoodsDetailService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GoodsDetailService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<GoodsDetailModel>> GetAllByGood(int goodID)
    {
        var goodsDetails = await  _context.GoodDetails.Where(x => !(x.IsDeleted ?? false)
                    && x.GoodID == goodID)
                .ToListAsync();
        return _mapper.Map<List<GoodsDetailModel>>(goodsDetails);
    }

    public async Task<List<GoodsDetailModel>> CreateList(List<GoodsDetailModel> param)
    {
        var goodDetails = _mapper.Map<List<GoodDetail>>(param);
        await _context.GoodDetails.AddRangeAsync(goodDetails);
        await _context.SaveChangesAsync();
        return param;
    }

    public async Task Update(GoodsDetailModel param)
    {
        var productNorm = await _context.GoodDetails.FindAsync(param.ID);
        if (productNorm == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        productNorm.Quantity = param.Quantity;
        productNorm.UnitPrice = param.UnitPrice;
        productNorm.Amount = param.Amount;
        
        _context.GoodDetails.Update(productNorm);
       await  _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var productNorm = await _context.GoodDetails.FindAsync(id);
        if (productNorm != null)
        {
            _context.GoodDetails.Remove(productNorm);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        }
    }
}