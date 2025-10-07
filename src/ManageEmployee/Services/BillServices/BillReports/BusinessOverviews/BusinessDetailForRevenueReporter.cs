using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews;
using ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.BusinessOverviews
{
    public class BusinessDetailForRevenueReporter: IBusinessDetailForRevenueReporter
    {
        private readonly ApplicationDbContext _context;
        public BusinessDetailForRevenueReporter(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<BusinessDetailForGroupGoodReporterModel> ReportAsync(BillReportBranchRequestModel param)
        {
            
            var itemOut = new BusinessDetailForGroupGoodReporterModel();

            itemOut.Goods = await GoodAsync(param.FromAt, param.ToAt, param.BranchId);
            itemOut.GroupGoods = await GroupGoodAsync(param.FromAt, param.ToAt, param.BranchId);
            itemOut.Users = await UserAsync(param.FromAt, param.ToAt, param.BranchId);
            itemOut.Customers = await CustomerAsync(param.FromAt, param.ToAt, param.BranchId);

            return itemOut;
        }

        private async Task<IEnumerable<ReportForGroupGoodModel>> GroupGoodAsync(DateTime fromAt, DateTime toAt, int? branchId)
        {
            var groupGoods = await _context.BillDetails
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt
                            && (branchId == null || x.BranchId == branchId))
                .Join(_context.Goods,
                b => b.GoodsId,
                g => g.Id,
                (b, g) => new
                {
                    bill = b,
                    good = g,
                })
                .GroupBy(x => x.good.Detail1)
                .Select(x => new ReportForGroupGoodModel
                {
                    Code = x.Key,
                    Name = x.FirstOrDefault().good.DetailName1,
                    Amount = x.Sum(x => x.bill.Quantity * x.bill.UnitPrice),
                    AmountAverage = 0,
                    AmountPreviousPeriod = 0,
                    TotalBill = x.Select(x => x.bill.BillId).Distinct().Count(),
                })
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var groupCodes = groupGoods.Select(x => x.Code);
            var dayAmount = (toAt - fromAt).Days + 1;
            toAt = fromAt.AddDays(-1);
            fromAt = fromAt.AddDays(-dayAmount);
            var itemOutPreviousPeriods = await _context.BillDetails
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt
                            && (branchId == null || x.BranchId == branchId))
                .Join(_context.Goods,
                b => b.GoodsId,
                g => g.Id,
                (b, g) => new
                {
                    bill = b,
                    good = g,
                })
                .GroupBy(x => x.good.Detail1)
                .Select(x => new ReportForGroupGoodModel
                {
                    Code = x.Key,
                    Amount = x.Sum(x => x.bill.Quantity * x.bill.UnitPrice),
                })
                .Where(x => groupCodes.Contains(x.Code))
                .OrderByDescending(x => x.Amount)
                .ToListAsync();

            foreach (var item in groupGoods)
            {
                item.AmountAverage = item.Amount / item.TotalBill;
                var RevenueAmountPreviousPeriod = itemOutPreviousPeriods.Find(x => x.Code == item.Code)?.Amount ?? 0;
                if (RevenueAmountPreviousPeriod > 0)
                {
                    item.AmountPreviousPeriod = (item.Amount - RevenueAmountPreviousPeriod) / RevenueAmountPreviousPeriod;
                }
            }
            return groupGoods;
        }

        private async Task<IEnumerable<ReportForGroupGoodModel>> GoodAsync(DateTime fromAt, DateTime toAt, int? branchId)
        {
            var groupGoods = await _context.BillDetails
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt
                                           && (branchId == null || x.BranchId == branchId))
                .Join(_context.Goods,
                b => b.GoodsId,
                g => g.Id,
                (b, g) => new
                {
                    bill = b,
                    good = g,
                })
                .GroupBy(x => x.good.Detail2)
                .Select(x => new ReportForGroupGoodModel
                {
                    Code = x.Key,
                    Name = x.FirstOrDefault().good.DetailName2,
                    Amount = x.Sum(x => x.bill.Quantity * x.bill.UnitPrice),
                    AmountAverage = 0,
                    AmountPreviousPeriod = 0,
                    TotalBill = x.Select(x => x.bill.BillId).Distinct().Count(),
                })
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var groupCodes = groupGoods.Select(x => x.Code);
            var dayAmount = (toAt - fromAt).Days + 1;
            toAt = fromAt.AddDays(-1);
            fromAt = fromAt.AddDays(-dayAmount);
            var itemOutPreviousPeriods = await _context.BillDetails
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt
                                && (branchId == null || x.BranchId == branchId))
                .Join(_context.Goods,
                b => b.GoodsId,
                g => g.Id,
                (b, g) => new
                {
                    bill = b,
                    good = g,
                })
                .GroupBy(x => x.good.Detail1)
                .Select(x => new ReportForGroupGoodModel
                {
                    Code = x.Key,
                    Amount = x.Sum(x => x.bill.Quantity * x.bill.UnitPrice),
                })
                .Where(x => groupCodes.Contains(x.Code))
                .OrderByDescending(x => x.Amount)
                .ToListAsync();

            foreach (var item in groupGoods)
            {
                item.AmountAverage = item.Amount / item.TotalBill;
                var RevenueAmountPreviousPeriod = itemOutPreviousPeriods.Find(x => x.Code == item.Code)?.Amount ?? 0;
                if (RevenueAmountPreviousPeriod > 0)
                {
                    item.AmountPreviousPeriod = (item.Amount - RevenueAmountPreviousPeriod) / RevenueAmountPreviousPeriod;
                }
            }
            return groupGoods;
        }

        private async Task<IEnumerable<ReportForCustomerModel>> CustomerAsync(DateTime fromAt, DateTime toAt, int? branchId)
        {
            var groupGoods = await _context.Bills
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt
                             && (branchId == null || x.BranchId == branchId))
                .GroupBy(x => x.CustomerId)
                .Select(x => new ReportForCustomerModel
                {
                    Amount = x.Sum(x => x.TotalAmount + x.DiscountPrice),
                    AmountAverage = 0,
                    AmountPreviousPeriod = 0,
                    TotalBill = x.Count(),
                    CustomerId = x.Key
                })
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var customerIds = groupGoods.Select(x => x.CustomerId);
            var dayAmount = (toAt - fromAt).Days + 1;
            toAt = fromAt.AddDays(-1);
            fromAt = fromAt.AddDays(-dayAmount);

            var itemOutPreviousPeriods = await _context.Bills
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt && customerIds.Contains(x.CustomerId)
                            && (branchId == null || x.BranchId == branchId))
                .GroupBy(x => x.CustomerId)
                .Select(x => new ReportForCustomerModel
                {
                    Amount = x.Sum(x => x.TotalAmount + x.DiscountPrice),
                    CustomerId = x.Key,
                    TotalBill = x.Count(),
                })
                .OrderByDescending(x => x.Amount)
                .ToListAsync();

            foreach (var item in groupGoods)
            {
                item.AmountAverage = item.Amount / item.TotalBill;
                var RevenueAmountPreviousPeriod = itemOutPreviousPeriods.Find(x => x.CustomerId == item.CustomerId)?.Amount ?? 0;
                if (RevenueAmountPreviousPeriod > 0)
                {
                    item.AmountPreviousPeriod = (item.Amount - RevenueAmountPreviousPeriod) / RevenueAmountPreviousPeriod;
                }
                var customer = await _context.Customers.FindAsync(item.CustomerId);
                if (customer != null)
                {
                    item.Code = customer.Code;
                    item.Name = customer.Name;
                }
            }
            return groupGoods;
        }

        private async Task<IEnumerable<ReportForUserModel>> UserAsync(DateTime fromAt, DateTime toAt, int? branchId)
        {
            var groupGoods = await _context.Bills
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt
                            && (branchId == null || x.BranchId == branchId))
                .GroupBy(x => x.UserCode)
                .Select(x => new ReportForUserModel
                {
                    Amount = x.Sum(x => x.TotalAmount + x.DiscountPrice),
                    AmountAverage = 0,
                    AmountPreviousPeriod = 0,
                    TotalBill = x.Count(),
                    UserCode = x.Key
                })
                .OrderByDescending(x => x.Amount)
                .Take(10)
                .ToListAsync();

            var userCodes = groupGoods.Select(x => x.UserCode);
            var dayAmount = (toAt - fromAt).Days + 1;
            toAt = fromAt.AddDays(-1);
            fromAt = fromAt.AddDays(-dayAmount);
            var itemOutPreviousPeriods = await _context.Bills
                .Where(x => x.CreatedDate >= fromAt && x.CreatedDate <= toAt && userCodes.Contains(x.UserCode)
                            && (branchId == null || x.BranchId == branchId))
                .GroupBy(x => x.UserCode)
                .Select(x => new ReportForUserModel
                {
                    Amount = x.Sum(x => x.TotalAmount + x.DiscountPrice),
                    UserCode = x.Key,
                    TotalBill = x.Count(),
                })
                .OrderByDescending(x => x.Amount)
                .ToListAsync();

            foreach (var item in groupGoods)
            {
                item.AmountAverage = item.Amount / item.TotalBill;
                var RevenueAmountPreviousPeriod = itemOutPreviousPeriods.Find(x => x.UserCode == item.UserCode)?.Amount ?? 0;
                if (RevenueAmountPreviousPeriod > 0)
                {
                    item.AmountPreviousPeriod = (item.Amount - RevenueAmountPreviousPeriod) / RevenueAmountPreviousPeriod;
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == item.UserCode);
                if (user != null)
                {
                    item.Code = user.Username;
                    item.Name = user.FullName;
                }
            }
            return groupGoods;
        }


    }
}
