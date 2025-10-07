using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices;


public class BillReporter : IBillReporter
{
    private readonly ApplicationDbContext _context;
    public BillReporter(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BillReporterModel> ReportAsync(BillPagingRequestModel param)
    {
        var fromAt = (param.FromAt ?? DateTime.Today).Date;
        var toAt = (param.ToAt ?? DateTime.Today).Date.AddDays(1);

        var user = await _context.Users.FindAsync(param.UserId);
        if (user is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        var bills = await _context.Bills.Where(x => x.CreatedDate >= fromAt && x.CreatedDate < toAt && x.UserCode == user.Username).ToListAsync();
        var customerIds = bills.Select(x => x.CustomerId);
        var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();
        var configDiscountCodes = typeof(ConfigDiscountConst).GetFields().Select(x => x.ToString());
        var configDiscounts = await _context.ConfigDiscounts.Where(x => configDiscountCodes.Contains(x.Code)).ToListAsync();
        var customerClassficationOld = await _context.CustomerClassifications.FirstOrDefaultAsync(x => x.Code == ConfigDiscountConst.CustomerOld);
        var customerClassficationNew = await _context.CustomerClassifications.FirstOrDefaultAsync(x => x.Code == ConfigDiscountConst.CustomerNew);

        var itemOut = new BillReporterModel
        {
            Items = new List<BillReporterDetailModel>()
        };
        double amountBonus = 0;
        foreach (var customer in customers)
        {
            var billCustomers = bills.Where(x => x.CustomerId == customer.Id).ToList();
            var billCustomerIds = billCustomers.Select(x => x.Id);
            var detailOut = new BillReporterDetailModel
            {
                CustomerName = customer.Name,
            };
            detailOut.QuantitySold = await _context.BillDetails.Where(x => billCustomerIds.Contains(x.BillId)).SumAsync(x => x.Quantity);
            detailOut.AmountSold = await _context.BillDetails.Where(x => billCustomerIds.Contains(x.BillId)).SumAsync(x => x.Quantity * x.UnitPrice);
            detailOut.QuantityRefund = await _context.BillDetailRefunds.Where(x => billCustomerIds.Contains(x.BillDetailId ?? 0)).SumAsync(x => x.Quantity);
            detailOut.AmountRefund = await _context.BillDetailRefunds.Where(x => billCustomerIds.Contains(x.BillDetailId ?? 0)).SumAsync(x => x.Quantity * x.UnitPrice);

            detailOut.QuantityRemaining = detailOut.QuantitySold - detailOut.QuantityRefund;
            detailOut.AmountRemaining = detailOut.AmountSold - detailOut.AmountRefund;
            var debitcode1 = "111";
            var debitcode2 = "112";
            var creditcode = "131";
            detailOut.TotalAmount = await _context.Ledgers.Where(x =>
                x.OrginalBookDate >= fromAt &&
                x.OrginalBookDate < toAt
                && (
                    x.DebitCode.StartsWith(debitcode1) ||
                    x.DebitCode.StartsWith(debitcode2)
                ) &&
                x.CreditCode.StartsWith(creditcode)
            ).SumAsync(x => x.Amount);

            itemOut.Items.Add(detailOut);
            if (customer.CustomerClassficationId == customerClassficationOld?.Id)
            {
                amountBonus += configDiscounts.FirstOrDefault(x => x.Code == customerClassficationOld.Code)?.DiscountReceivedYear ?? 0;
            }
            else if (customer.CustomerClassficationId == customerClassficationNew?.Id)
            {
                amountBonus += configDiscounts.FirstOrDefault(x => x.Code == customerClassficationNew.Code)?.DiscountReceivedYear ?? 0;
            }
        }
        itemOut.TotalItem = new BillReporterDetailModel
        {
            QuantityRefund = itemOut.Items.Sum(x => x.QuantityRefund),
            QuantityRemaining = itemOut.Items.Sum(x => x.QuantityRemaining),
            QuantitySold = itemOut.Items.Sum(x => x.QuantitySold),
            AmountRefund = itemOut.Items.Sum(x => x.AmountRefund),
            AmountRemaining = itemOut.Items.Sum(x => x.AmountRemaining),
            AmountSold = itemOut.Items.Sum(x => x.AmountSold),
            TotalAmount = itemOut.Items.Sum(x => x.AmountSold),
        };

        itemOut.TotalQuantity = itemOut.Items.Sum(x => x.QuantityRemaining);
        itemOut.AverageUnitPrice = itemOut.Items.Sum(x => x.AmountRemaining) / itemOut.TotalQuantity;
        itemOut.TonsCollected = itemOut.Items.Sum(x => x.TotalAmount) / itemOut.AverageUnitPrice;

        itemOut.AmountBonus = itemOut.TonsCollected * amountBonus;
        itemOut.ReceiveBonus = itemOut.AmountBonus / 2;
        itemOut.ToTalAmount = itemOut.AmountBonus - itemOut.ReceiveBonus;

        return itemOut;
    }

    public async Task<object> ReportHomeAsync(RequestFilterDateModel param, int year)
    {
        try
        {
            if (param.FromAt is null)
            {
                param.FromAt = new DateTime(year, 1, 1);
            }

            if (param.ToAt is null)
            {
                param.ToAt = DateTime.Today.AddDays(1);
            }
            else
            {
                param.ToAt = param.ToAt.Value.AddDays(1);
            }

            var ledgerQuyery = _context.GetLedger(year, 3).Where(x => x.OrginalBookDate >= param.FromAt && x.OrginalBookDate < param.ToAt);

            var itemOut = new
            {
                TotalAmount = Math.Abs((await ledgerQuyery.Where(x => x.DebitCode == "5111").SumAsync(x => x.Amount)) - (await ledgerQuyery.Where(x => x.CreditCode == "5111" ).SumAsync(x => x.Amount))),
                TotalQuantity = Math.Abs((await ledgerQuyery.Where(x => x.DebitCode == "6422").SumAsync(x => x.Amount)) - (await ledgerQuyery.Where(x => x.CreditCode == "6422").SumAsync(x => x.Amount))),
                TotalCustomer = await _context.Bills.Where(x => x.Status.Contains(TranTypeConst.Paid)
                               && x.CreatedDate >= param.FromAt && x.CreatedDate < param.ToAt).SumAsync(x => x.QuantityCustomer),
                TotalAmountCN = Math.Abs((await ledgerQuyery.Where(x => x.DebitCode == "131").SumAsync(x => x.Amount)) - (await ledgerQuyery.Where(x => x.CreditCode == "131").SumAsync(x => x.Amount))),
                TotalAmount331 = Math.Abs((await ledgerQuyery.Where(x => x.DebitCode == "331").SumAsync(x => x.Amount)) - (await ledgerQuyery.Where(x => x.CreditCode == "331").SumAsync(x => x.Amount))),
                TotalAmountWebsite = 0
            };

            return itemOut;
        }
        catch(Exception ex) 
        {
            throw new ErrorException(ex.Message, ex);
        }
    }

}