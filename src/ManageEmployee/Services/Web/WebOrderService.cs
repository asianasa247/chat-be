using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Mails;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Web;

public class WebOrderService : IWebOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly ISendMailService _sendMailService;
    private readonly ICompanyService _companyService;
    public WebOrderService(ApplicationDbContext context, ISendMailService sendMailService, ICompanyService companyService)
    {
        _context = context;
        _sendMailService = sendMailService;
        _companyService = companyService;
    }

    /// <summary>
    /// Thêm/Tạo mới đơn hàng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task Create(OrderViewModel request, int? userId = 0)
    {
        var order = new Order();
        if (!request.Goods.Any())
        {
            throw new ErrorException("Bạn chưa chọn sản phẩm nào!");
        }
        var taxRates = await _context.TaxRates.Where(x => x.Code.StartsWith("R")).ToListAsync();
        foreach (var item in request.Goods)
        {
            var good = await _context.Goods.Where(x => x.Id == item.GoodId).FirstOrDefaultAsync();
            if (good == null)
            {
                continue;
            }
            var taxRate = taxRates.Find(x => x.Id == good.TaxRateId);
            var taxVat = good.SalePrice * (taxRate?.Percent ?? 0) / 100;
            order.TotalPrice += item.Quantity * (good.SalePrice + taxVat);
        }
        var customer = await _context.Customers.FindAsync(userId);
        var newCustomerId = 0;
        if (customer == null)
        {
            var model = new Entities.CustomerEntities.Customer
            {
                ProvinceId = request.ProvinceId,
                WardId = request.WardId,
                DistrictId = request.DistrictId,
                Phone = request.Tell,
                Address = request.ShippingAddress,
                Name = request.FullName,
                Type = 2
            };
            _context.Customers.Add(model);
            await _context.SaveChangesAsync();
             newCustomerId = model.Id;
        }
        order.ShippingAddress = request.ShippingAddress;
        order.Status = OrderStatus.CREATED;
        order.Tell = request.Tell;
        order.FullName = request.FullName;
        order.CreatedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;
        order.CustomerId = newCustomerId;
        order.ToAt = request.ToAt;
        order.FromAt = request.FromAt;
        order.IsPayment = request.IsPayment;
        order.PaymentAt = request.PaymentAt;
        order.Identifier = request.Identifier;
        order.Broker = request.Broker;
        order.TotalPrice = request.TotalPrice;
        order.TotalPriceDiscount = request.TotalPriceDiscount;
        
        order.TotalPricePaid = order.TotalPrice - order.TotalPriceDiscount;

        order.Email = request.Email;
        order.Date = request.Date;
        order.PhoneNumber = request.PhoneNumber;
        order.PaymentMethod = request.PaymentMethod;
        order.Promotion = request.Promotion;

        await _context.Order.AddAsync(order);
        await _context.SaveChangesAsync();

        

        var orderDetails = new List<OrderDetail>();

        // delete quantity
        var goodIds = request.Goods.Select(x => x.GoodId);
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var roomTypes = await _context.GoodRoomTypes.Where(x => goodIds.Contains(x.GoodId)).ToListAsync();

        foreach (var item in request.Goods)
        {
            var good = goods.FirstOrDefault(x => x.Id == item.GoodId);
            if (good == null)
            {
                continue;
            }
            var orderDetail = new OrderDetail();
            orderDetail.OrderId = order.Id;
            orderDetail.GoodId = item.GoodId;
            orderDetail.Quantity = item.Quantity;
            orderDetail.Price = item.Price;
            //orderDetail.UserCreated = userId;
            //orderDetail.UserUpdated = userId;

            var taxRate = taxRates.Find(x => x.Id == good.TaxRateId);
            orderDetail.TaxVAT = good.Price * (taxRate?.Percent ?? 0) / 100;
            orderDetail.CreatedAt = DateTime.Now;
            orderDetail.UpdatedAt = DateTime.Now;
            orderDetails.Add(orderDetail);
        }
        _context.GoodRoomTypes.UpdateRange(roomTypes);
        await _context.OrderDetail.AddRangeAsync(orderDetails);

        var carts = await _context.Cart.Where(x => !x.IsDelete && x.CustomerId == userId && goodIds.Contains(x.GoodId)).ToListAsync();
        carts = carts.ConvertAll(x =>
        {
            x.IsDelete = true;
            x.DeleteAt = DateTime.Now;
            return x;
        });
        _context.Cart.UpdateRange(carts);

        await _context.SaveChangesAsync();

        // send mail
        var company = await _companyService.GetCompany();
        var mailContent = new SendMail
        {
            CustomerId = userId,
            Title = "Đặt vé thành công",
            CreateAt = DateTime.Now,
            CreateSend = DateTime.Now,
            Content = @$"<p> Chúng tôi xin cảm ơn quý khách đã tin tưởng và sử dụng sản phẩm tại {company.Name}. 

                            Chúng tôi luôn nỗ lực không ngừng để cải thiện chất lượng sản phẩm và dịch vụ, nhằm đáp ứng và vượt qua sự mong đợi của Quý khách. Mỗi phản hồi và sự hài lòng của Quý khách là nguồn cảm hứng để chúng tôi hoàn thiện mình hơn mỗi ngày. </p>",
            Type = nameof(SendMailType.WebOrder),
        };
        _sendMailService.SendEmail(mailContent, request.Email);

    }

    public async Task<CommonWebResponse> GetByCustomer(WebOrderSearchModel searchModel)
    {
        try
        {
            var query = _context.Order.Where(x => !x.IsDelete && x.CustomerId == searchModel.CustomerId)
                                         .Where(x => string.IsNullOrEmpty(searchModel.SearchText) ||
                                                (!string.IsNullOrEmpty(x.FullName) && x.FullName.ToLower().Contains(searchModel.SearchText.ToLower())) ||
                                                (!string.IsNullOrEmpty(x.ShippingAddress) && x.ShippingAddress.ToLower().Contains(searchModel.SearchText.ToLower())))
                                         .Where(x => searchModel.Status == null || x.Status == searchModel.Status);

            var orders = await query.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize)
                                         .ToListAsync();

            var data = new List<OrderHistoryViewModel>();
            foreach (var order in orders)
            {
                var orderHistory = new OrderHistoryViewModel();
                orderHistory.Id = order.Id;
                orderHistory.Status = order.Status;
                orderHistory.FullName = order.FullName;
                orderHistory.ShippingAddress = order.ShippingAddress;
                orderHistory.Tell = order.Tell;
                orderHistory.TotalPrice = order.TotalPrice;
                orderHistory.TotalPriceDiscount = order.TotalPriceDiscount;
                orderHistory.TotalPricePaid = order.TotalPricePaid;
                orderHistory.UserCreated = order.UserCreated;
                orderHistory.CreatedAt = order.CreatedAt;
                switch (order.Status)
                {
                    case OrderStatus.CREATED:
                        orderHistory.StatusName = "Mới tạo";
                        break;

                    case OrderStatus.CONFIRMED:
                        orderHistory.StatusName = "Đã xác nhận";
                        break;

                    case OrderStatus.SHIPPING:
                        orderHistory.StatusName = "Đang giao";
                        break;

                    case OrderStatus.SHIPPED:
                        orderHistory.StatusName = "Đã giao";
                        break;

                    case OrderStatus.COMPLETED:
                        orderHistory.StatusName = "Hoàn thành";
                        break;

                    case OrderStatus.CANCELED:
                        orderHistory.StatusName = "Đã hủy";
                        break;
                }
                orderHistory.OrderDetails = new List<OrderHistoryDetailViewModel>();
                var orderDetails = await _context.OrderDetail.Where(detail => detail.OrderId == order.Id).ToListAsync();
                foreach (var detail in orderDetails)
                {
                    var orderDetail = new OrderHistoryDetailViewModel();
                    orderDetail.Id = detail.Id;
                    orderDetail.OrderId = detail.OrderId;
                    orderDetail.Price = detail.Price;
                    orderDetail.Quantity = detail.Quantity;
                    orderDetail.TaxVAT = detail.TaxVAT;
                    orderDetail.GoodId = detail.GoodId;
                    orderDetail.Good = await _context.Goods.Where(good => good.Id == detail.GoodId).FirstOrDefaultAsync();
                    orderHistory.OrderDetails.Add(orderDetail);
                }
                data.Add(orderHistory);
            }

            var result = new PagingResult<OrderHistoryViewModel>
            {
                CurrentPage = searchModel.Page,
                PageSize = searchModel.PageSize,
                TotalItems = await query.CountAsync(),
                Data = data
            };
            return new CommonWebResponse()
            {
                State = true,
                Code = 200,
                Message = "Lấy dữ liệu thành công",
                Data = result
            };
        }
        catch
        {
            return new CommonWebResponse()
            {
                State = false,
                Code = 400,
                Message = "Lỗi lấy dữ liệu",
                Data = null
            };
        }
    }
}