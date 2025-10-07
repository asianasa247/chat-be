using Common.Helpers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Orders;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IBillService _billService;
    private readonly IBillDetailService _billDetailService;

    public OrderService(ApplicationDbContext context, IBillService billService, IBillDetailService billDetailService)
    {
        _context = context;
        _billService = billService;
        _billDetailService = billDetailService;
    }

    public async Task<PagingResult<OrderViewModelResponse>> SearchOrder(OrderSearchModel search)
    {
        try
        {
            var datas = _context.Order.Where(x => !x.IsDelete)
                                      .Where(x => string.IsNullOrEmpty(search.SearchText) || x.FullName.Contains(search.SearchText))
                                      .Where(x => search.status == null || x.Status == search.status);
            if (search.fromDate > 0)
            {
                var fromDate = DateHelpers.UnixTimeStampToDateTime(search.fromDate ?? 0).Date;
                datas = datas.Where(x => x.CreatedAt >= fromDate);
            }
            if (search.toDate > 0)
            {
                var toDate = DateHelpers.UnixTimeStampToDateTime(search.toDate ?? 0).Date;
                datas = datas.Where(x => x.CreatedAt <= toDate);
            }
            var orders = await datas.Skip((search.Page) * search.PageSize).Take(search.PageSize).ToListAsync();
            var result = new List<OrderViewModelResponse>();
            var customerIds = orders.Select(x => x.CustomerId).ToList();
            var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();
            foreach (var order in orders)
            {
                var customer = customers.Find(x => x.Id == order.CustomerId);
                var orderViewModel = new OrderViewModelResponse();
                orderViewModel.Id = order.Id;
                orderViewModel.TotalPrice = order.TotalPrice;
                orderViewModel.TotalPriceDiscount = order.TotalPriceDiscount;
                orderViewModel.TotalPricePaid = order.TotalPricePaid;
                orderViewModel.Status = order.Status;
                orderViewModel.StatusName = EnumHelpers.GetDescription(order.Status);
                orderViewModel.CustomerId = order.CustomerId;
                orderViewModel.CustomerName = customer?.Name;
                orderViewModel.Tell = customer?.Phone;
                orderViewModel.FullName = order.FullName;
                orderViewModel.ShippingAddress = order.ShippingAddress;
                orderViewModel.CreateAt = order.CreatedAt;
                orderViewModel.OrderDetails = new List<OrderDetailViewModel>();
                var orderDetails = await _context.OrderDetail.Where(x => x.OrderId == order.Id).ToListAsync();
                foreach (var item in orderDetails)
                {
                    var orderDetail = new OrderDetailViewModel();
                    orderDetail.GoodId = item.GoodId;
                    orderDetail.GoodDetailId = item.GoodDetailId;
                    var good = await _context.Goods.FirstOrDefaultAsync(x => x.Id == item.GoodId);
                    orderDetail.GoodCode = !string.IsNullOrEmpty(good?.Detail2) ? good?.Detail2 : good?.Detail1;
                    orderDetail.GoodName = !string.IsNullOrEmpty(good?.DetailName2) ? good?.DetailName2 : good?.DetailName1;
                    orderDetail.Price = item.Price;
                    orderDetail.TaxVAT = item.TaxVAT ?? 0;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.DiscountPrice = 0;
                    orderDetail.TotalAmount = orderDetail.Price * orderDetail.Quantity + orderDetail.TaxVAT - orderDetail.DiscountPrice;
                    orderViewModel.OrderDetails.Add(orderDetail);
                }
                result.Add(orderViewModel);
            }

            return new PagingResult<OrderViewModelResponse>()
            {
                CurrentPage = search.Page,
                PageSize = search.PageSize,
                TotalItems = await datas.CountAsync(),
                Data = result
            };
        }
        catch
        {
            return new PagingResult<OrderViewModelResponse>()
            {
                CurrentPage = search.Page,
                PageSize = search.PageSize,
                TotalItems = 0,
                Data = new List<OrderViewModelResponse>()
            };
        }
    }

    public async Task<List<OrderDetailViewModel>> GetDetail(int id)
    {
        var itemOuts = new List<OrderDetailViewModel>();
        var orderDetails = await _context.OrderDetail.Where(x => x.OrderId == id).ToListAsync();
        foreach (var item in orderDetails)
        {
            var orderDetail = new OrderDetailViewModel
            {
                Id = item.Id,
                GoodId = item.GoodId,
                GoodDetailId = item.GoodDetailId
            };
            var good = await _context.Goods.Where(x => x.Id == item.GoodId).FirstOrDefaultAsync();
            orderDetail.GoodName = good != null ? (good.DetailName2 ?? good.DetailName1) : "";
            orderDetail.GoodCode = good != null ? (good.Detail2 ?? good.Detail1) : "";
            orderDetail.Price = item.Price;
            orderDetail.TaxVAT = item.TaxVAT ?? 0;
            orderDetail.Quantity = item.Quantity;
            orderDetail.DiscountPrice = 0;
            orderDetail.TotalAmount = orderDetail.Quantity * (orderDetail.Price + orderDetail.TaxVAT);
            itemOuts.Add(orderDetail);
        }
        return itemOuts;
    }

    public async Task<string> Update(OrderViewModelResponse requests, int userId, int year)
    {
        try
        {
            var order = await _context.Order.Where(x => x.Id == requests.Id).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }
            if (order.Status == OrderStatus.CREATED && requests.Status != OrderStatus.CONFIRMED)
            {
                throw new ErrorException(ErrorMessages.CannotChangeStatus);
            }
            if (order.Status == OrderStatus.CONFIRMED && requests.Status != OrderStatus.SHIPPING)
            {
                throw new ErrorException(ErrorMessages.CannotChangeStatus);
            }
            
            order.Status = requests.Status;
            order.UpdatedAt = DateTime.Now;
            var orderDetails = await _context.OrderDetail.Where(x => x.OrderId == requests.Id).ToListAsync();

            // create a new bill
            if (requests.Status == OrderStatus.CONFIRMED)
            {
                var billModel = new BillModel
                {
                    UserCode = "",
                    TotalAmount = order.TotalPrice,
                    QuantityCustomer = 1,
                    BillNumber = BillNumberGenerator.GenerateBillNumber(),
                    CustomerId = order.CustomerId,
                    UserCreated = userId,
                };
                var bill = await _billService.Create(billModel);
                order.BillId = bill.Id;

                var billDetails = orderDetails.Select(x => new BillDetailModel
                {
                    BillId = bill.Id,
                    GoodsId = x.GoodId,
                    Quantity = x.Quantity,
                    UnitPrice = x.Price,
                    TaxVAT = x.TaxVAT,
                }).ToList();
                await _billDetailService.Create(billDetails, year);
            }

            _context.Order.Update(order);

            await _context.SaveChangesAsync();
        }
        catch(Exception ex)
        {
            throw new ErrorException(ex.Message);
        }

        return string.Empty;
    }

    public async Task<IEnumerable<OrderViewModelResponse>> GetListOrderNew()
    {
        try
        {
            var orders = await _context.Order.Where(x => x.IsDelete != true)
                                     .Where(x => x.Status == OrderStatus.CREATED).ToListAsync();
            var result = new List<OrderViewModelResponse>();

            foreach (var order in orders)
            {
                var orderViewModel = new OrderViewModelResponse();
                orderViewModel.Id = order.Id;
                orderViewModel.TotalPrice = order.TotalPrice;
                orderViewModel.TotalPriceDiscount = order.TotalPriceDiscount;
                orderViewModel.TotalPricePaid = order.TotalPricePaid;
                orderViewModel.Status = order.Status;
                orderViewModel.StatusName = EnumHelpers.GetDescription(order.Status);
                orderViewModel.CustomerId = order.CustomerId;
                orderViewModel.Tell = order.Tell;
                orderViewModel.FullName = order.FullName;
                orderViewModel.ShippingAddress = order.ShippingAddress;
                orderViewModel.CreateAt = order.CreatedAt;
                orderViewModel.Notify = " <strong> " + order.FullName + " </strong ><br/> " + order.ShippingAddress + " <br/> " + order.CreatedAt.ToString("dd/MM/yyyy:HH:mm");

                orderViewModel.OrderDetails = new List<OrderDetailViewModel>();
                var orderDetails = await _context.OrderDetail.Where(x => x.OrderId == order.Id).ToListAsync();
                foreach (var item in orderDetails)
                {
                    var orderDetail = new OrderDetailViewModel();
                    orderDetail.GoodId = item.GoodId;
                    orderDetail.GoodDetailId = item.GoodDetailId;
                    var good = _context.Goods.Where(x => x.Id == item.GoodId).FirstOrDefault();
                    orderDetail.GoodCode = good != null ? (good.Detail2 ?? good.Detail1) : "";
                    orderDetail.GoodName = good != null ? (good.DetailName2 ?? good.DetailName1) : "";
                    orderDetail.Price = item.Price;
                    orderDetail.TaxVAT = item.TaxVAT ?? 0;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.DiscountPrice = 0;
                    orderViewModel.OrderDetails.Add(orderDetail);
                }
                result.Add(orderViewModel);
            }

            return result;
        }
        catch
        {
            return null;
        }
    }
    public static class BillNumberGenerator
    {
        private static int currentBillNumber = 0; // Biến để theo dõi số thứ tự hiện tại.

        public static string GenerateBillNumber()
        {
            currentBillNumber++; // Tăng số thứ tự lên 1.
            return "DONHANGWEB-" + currentBillNumber.ToString("D3"); // Tạo giá trị BillNumber theo định dạng "DONHANGWEB-001", "DONHANGWEB-002", ...
        }
    }
}