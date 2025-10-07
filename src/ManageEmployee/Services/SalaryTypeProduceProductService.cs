using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SalaryModels;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Services.Interfaces.Salarys;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class SalaryTypeProduceProductService: ISalaryTypeProduceProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public SalaryTypeProduceProductService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<SalaryTypeProduceProductPagingModel>> GetPaging(PagingRequestModel form)
    {
        var query = _context.SalaryTypeProduceProducts.Where(x => x.Id > 0);
        var data = await query.Skip((form.Page) * form.PageSize).Take(form.PageSize).ToListAsync();
        var listOut = new List<SalaryTypeProduceProductPagingModel>();

        foreach(var item in data)
        {
            var itemOut = _mapper.Map<SalaryTypeProduceProductPagingModel>(item);
            var userIds = await _context.SalaryTypeProduceProductDetails.Where(x => x.SalaryTypeProduceProductId == item.Id).Select(x => x.UserId).Distinct().ToListAsync();
            itemOut.Users = String.Join("; ", await _context.Users.Where(x => userIds.Contains(x.Id)).Select(x => x.FullName).ToListAsync());
            listOut.Add(itemOut);
        }

        var result = new PagingResult<SalaryTypeProduceProductPagingModel>()
        {
            CurrentPage = form.Page,
            PageSize = form.PageSize,
            Data = listOut,
            TotalItems = await query.CountAsync()
        };
        return result;
    }

    public async Task Create(SalaryTypeProduceProductModel request, int userId)
    {
        var item = _mapper.Map<SalaryTypeProduceProduct>(request);
        item.ProduceProductCode = await _context.ProduceProducts.Where(x => x.Id ==request.ProduceProductId).Select(x => x.Code).FirstOrDefaultAsync();
        item.CreatedAt = DateTime.Now;
        item.UpdatedAt = DateTime.Now;
        item.UserCreated = userId;
        item.UserUpdated = userId;

        _context.SalaryTypeProduceProducts.Add(item);
        await _context.SaveChangesAsync();

        await AddDetail(request, item.Id);
    }

    public async Task<SalaryTypeProduceProductModel> GetById(int id)
    {
        var item = await _context.SalaryTypeProduceProducts.FindAsync(id);
        var itemOut = _mapper.Map<SalaryTypeProduceProductModel>(item);
        itemOut.Items = await _context.SalaryTypeProduceProductDetails.Where(X => X.SalaryTypeProduceProductId == id)
                        .Select( x => _mapper.Map<SalaryTypeProduceProductDetailModel>(x)).ToListAsync();
        return itemOut;
    }

    public async Task Update(SalaryTypeProduceProductModel request, int userId)
    {
        var item = await _context.SalaryTypeProduceProducts.FindAsync(request.Id);
        item.Quantity = request.Quantity;
        item.Note = request.Note;
        item.UpdatedAt = DateTime.Now;
        item.UserUpdated = userId;
        item.SalaryTypeId = request.SalaryTypeId;

        _context.SalaryTypeProduceProducts.Update(item);
        await _context.SaveChangesAsync();
        await AddDetail(request, item.Id);
    }
    private async Task AddDetail(SalaryTypeProduceProductModel request, int id)
    {
        if (request.Items is null)
        {
            return;
        }    

        var detailUpdateIds = request.Items.Select(x => x.Id).Distinct();
        var detailDels = await _context.SalaryTypeProduceProductDetails.Where(x => x.SalaryTypeProduceProductId == id && !detailUpdateIds.Contains(x.Id)).ToListAsync();
        if (detailDels.Any())
        {
            _context.SalaryTypeProduceProductDetails.RemoveRange(detailDels);
        }
        var itemAdds = _mapper.Map<List<SalaryTypeProduceProductDetail>>(request.Items);
        _context.SalaryTypeProduceProductDetails.UpdateRange(itemAdds);
        await _context.SaveChangesAsync();
    }
    public async Task Delete(int id)
    {
        var item = await _context.SalaryTypeProduceProducts.FindAsync(id);
        _context.SalaryTypeProduceProducts.Remove(item);
        await _context.SaveChangesAsync();
    }
}
