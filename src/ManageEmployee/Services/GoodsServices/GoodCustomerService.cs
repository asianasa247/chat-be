using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.GoodsServices
{
    public class GoodCustomerService: IGoodCustomerService
    {
        private readonly ApplicationDbContext _context;

        public GoodCustomerService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(int goodId, int customerId)
        {
            var goodCustomer = new GoodCustomer
            {
                GoodId = goodId,
                CustomerId = customerId
            };
            await _context.AddAsync(goodCustomer);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAsync(int goodId, int customerId)
        {
            var goodCustomer = await _context.GoodCustomers.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.GoodId == goodId);
            if (goodCustomer is null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }
            _context.GoodCustomers.Remove(goodCustomer);
            await _context.SaveChangesAsync();
        }

        public async Task<PagingResult<WebGoodModel>> GetGoodForCustomer(PagingRequestModel searchModel, int customerId)
        {
            var query = _context.GoodCustomers
                .Join(_context.Goods,
                    goodCustomer => goodCustomer.GoodId,
                    good => good.Id,
                    (goodCustomer, good) => new { good = good, CustomerId = goodCustomer.CustomerId })
                .Where(x => x.CustomerId == customerId)
                .Where(x => string.IsNullOrEmpty(searchModel.SearchText) ||
                                                (!string.IsNullOrEmpty(x.good.Detail2) && x.good.Detail2.ToLower().Contains(searchModel.SearchText.ToLower())) ||
                                                (!string.IsNullOrEmpty(x.good.DetailName2) && x.good.DetailName2.ToLower().Contains(searchModel.SearchText.ToLower())) ||
                                                (!string.IsNullOrEmpty(x.good.DetailName1) && x.good.DetailName1.ToLower().Contains(searchModel.SearchText.ToLower())) ||
                                                (!string.IsNullOrEmpty(x.good.Detail1) && x.good.Detail1.ToLower().Contains(searchModel.SearchText.ToLower())));

            var goods = await query.Skip((searchModel.Page) * searchModel.PageSize).Take(searchModel.PageSize)
                .Select(x => new WebGoodModel
                {
                    Id = x.good.Id,
                    Code = GoodNameGetter.GetCodeFromGood(x.good),
                    Name = GoodNameGetter.GetNameFromGood(x.good),
                    Price = x.good.SalePrice,
                })
                .ToListAsync();

            return new PagingResult<WebGoodModel>()
            {
                CurrentPage = searchModel.Page,
                PageSize = searchModel.PageSize,
                TotalItems = await query.CountAsync(),
                Data = goods
            };
        }
    }
}
