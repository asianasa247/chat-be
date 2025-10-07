using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Requests;
using ManageEmployee.DataTransferObject.Response;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.WebEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Web;

public class WebCartService : IWebCartService
{
    private readonly ApplicationDbContext _context;

    public WebCartService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CountCart(int userId)
    {
        var count = await _context.Cart.Where(x => !x.IsDelete && x.State == CartStateEnum.WAIT && x.CustomerId == userId).CountAsync();
        return count;
    }

    /// <summary>
    /// Thêm mới vào giỏ hàng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<CommonWebResponse> Create(CartEditViewModel request, int userId)
    {
        try
        {
            var entity = await _context.Cart.FirstOrDefaultAsync(x => x.CustomerId == userId && x.GoodId == request.GoodId && x.State == CartStateEnum.WAIT && x.IsDelete);
            if (entity != null)
            {
                entity.Quantity += request.Quantity ?? 0;
                _context.Cart.Update(entity);
            }
            else
            {
                entity = new Cart();
                entity.GoodId = request.GoodId ?? 0;
                entity.CustomerId = userId;
                entity.UpdatedAt = DateTime.Now;
                entity.Quantity = request.Quantity ?? 0;
                entity.State = CartStateEnum.WAIT;
                await _context.Cart.AddAsync(entity);
            }

            await _context.SaveChangesAsync();
            return new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "Thêm vào giỏ hàng thành công!"
            };
        }
        catch
        {
            return new CommonWebResponse()
            {
                State = false,
                Code = 400,
                Message = "Thêm vào giỏ hàng thất bại!"
            };
        }
    }

    /// <summary>
    /// Xóa sản phẩm khỏi giỏ hàng
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<CommonWebResponse> Delete(int id, int userId)
    {
        try
        {
            var entity = await _context.Cart.FirstOrDefaultAsync(x => x.Id == id && x.CustomerId == userId && !x.IsDelete);
            entity.IsDelete = true;
            entity.State = CartStateEnum.DELETED;
            _context.Cart.Update(entity);
            await _context.SaveChangesAsync();
            return new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "Đã xóa khỏi giỏ hàng!"
            };
        }
        catch
        {
            return new CommonWebResponse()
            {
                State = false,
                Code = 400,
                Message = "Lỗi thất bại!"
            };
        }
    }

    public async Task<CommonWebResponse> DeleteAll(int userId)
    {
        try
        {
            var carts = await _context.Cart.Where(x => x.CustomerId == userId && x.State == CartStateEnum.WAIT).ToListAsync();
            foreach (var item in carts)
            {
                await Delete(item.Id, userId);
            }
            return new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "Đã xóa khỏi giỏ hàng!"
            };
        }
        catch
        {
            return new CommonWebResponse()
            {
                State = false,
                Code = 400,
                Message = "Lỗi thất bại!"
            };
        }
    }

    /// <summary>
    /// Lấy danh sách sản phẩm theo Id khách hàng (Admin)
    /// </summary>
    /// <param name="cusId"></param>
    /// <returns></returns>
    public async Task<CommonWebResponse> GetByCustomerId(int cusId)
    {
        try
        {
            var products = await _context.Cart.Where(x => x.CustomerId == cusId && x.State == CartStateEnum.WAIT && !x.IsDelete).ToListAsync();
            var result = new List<CartViewModel>();
            var taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
            taxRates = taxRates.Where(x => x.Code.StartsWith('R')).ToList();
            foreach (var item in products)
            {
                var good = await _context.Goods.FirstOrDefaultAsync(x => x.Id == item.GoodId);
                var cart = new CartViewModel();
                cart.GoodId = good?.Id ?? 0;
                cart.Id = item.Id;
                cart.Quantity = item.Quantity;
                cart.State = item.State;
                cart.GoodCode = string.IsNullOrEmpty(good?.Detail2) ? (!string.IsNullOrEmpty(good?.Detail1) ? good?.Detail1 : "") : good?.Detail2;
                cart.GoodName = string.IsNullOrEmpty(good?.DetailName2) ? (!string.IsNullOrEmpty(good?.DetailName1) ? good?.DetailName1 : "") : good?.DetailName2;

                var taxRate = taxRates.Find(x => x.Id == good.TaxRateId);
                var taxVat = (good?.SalePrice ?? 0) * (taxRate?.Percent ?? 0) / 100;
                cart.Price = (good?.SalePrice ?? 0) + (taxVat);

                cart.PriceDiscount = good?.DiscountPrice ?? 0;
                cart.TotalPrice = cart.Price * item.Quantity;
                cart.Images = new List<string>();
                if (good?.Image1 != null)
                {
                    cart.Images.Add(good?.Image1);
                }
                if (good?.Image2 != null)
                {
                    cart.Images.Add(good?.Image2);
                }
                if (good?.Image3 != null)
                {
                    cart.Images.Add(good?.Image3);
                }
                if (good?.Image4 != null)
                {
                    cart.Images.Add(good?.Image4);
                }
                if (good?.Image5 != null)
                {
                    cart.Images.Add(good?.Image5);
                }
                result.Add(cart);
            }
            return new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "",
                Data = result
            };
        }
        catch
        {
            return new CommonWebResponse()
            {
                State = false,
                Code = 400,
                Message = "Lỗi lấy thông tin giỏ hàng",
                Data = null
            };
        }
    }

    /// <summary>
    /// Get product info input from client
    /// </summary>
    /// <param name="cartProducts"></param>
    /// <returns></returns>
    public async Task<List<ShoppingCartProductInfoResponse>> GetCartProductInfoAsync(
        List<ShoppingCartProductInfoRequest> cartProducts)
    {
        var goodIds = cartProducts.Select(x => x.GoodId);
        var taxRates = _context.TaxRates.Where(x => x.Code.StartsWith("R"));
        var goods = await (from good in _context.Goods
            join taxRate in taxRates on good.TaxRateId equals taxRate.Id into taxRateDefault
            from taxRate in taxRateDefault.DefaultIfEmpty()
            // where goodIds.Contains(good.Id)
            select new
            {
                good.Id,
                good.Detail2,
                good.Detail1,
                good.DetailName2,
                good.DetailName1,
                good.SalePrice,
                good.DiscountPrice,
                TaxVat = good.SalePrice * (taxRate != null ? taxRate.Percent : 0) / 10,
                Images = new List<string>
                {
                    good.Image1,
                    good.Image2,
                    good.Image3,
                    good.Image4,
                    good.Image5
                }
            }).ToListAsync();

        var result = (from g in goods
            join cartProduct in cartProducts on g.Id equals cartProduct.GoodId
            select new ShoppingCartProductInfoResponse
            {
                Id = g.Id,
                Code = g.Detail2.DefaultIfNullOrEmpty(g.Detail1),
                Name = g.DetailName2.DefaultIfNullOrEmpty(g.DetailName1),
                Price = g.SalePrice + g.TaxVat,
                PriceDiscount = g.DiscountPrice,
                TaxVat = g.TaxVat,
                Images = g.Images.Where(p => !p.IsNullOrEmpty()),
                Quantity = cartProduct.Quantity
            }).ToList();

        return result;
    }

    /// <summary>
    /// Cập nhật số lượng hàng trong giỏ hàng
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<CommonWebResponse> Update(CartEditViewModel request, int userId)
    {
        try
        {
            var entity = await _context.Cart.FirstOrDefaultAsync(x => x.CustomerId == userId && x.Id == request.Id && !x.IsDelete);
            if (entity is null)
                throw new ErrorException("Cập nhật giỏ hàng thất bại!");

            entity.UpdatedAt = DateTime.Now;
            entity.Quantity = request.Quantity ?? 0;
            _context.Cart.Update(entity);
            await _context.SaveChangesAsync();
            return new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "Cập nhật giỏ hàng thành công!"
            };
        }
        catch
        {
            throw new ErrorException("Cập nhật giỏ hàng thất bại!");
        }
    }
}