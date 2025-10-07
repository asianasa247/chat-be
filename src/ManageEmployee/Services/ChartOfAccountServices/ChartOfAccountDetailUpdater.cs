using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Validators;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChartOfAccountServices;
public class ChartOfAccountDetailUpdater: IChartOfAccountDetailUpdater
{
    private readonly IChartOfAccountValidator _chartOfAccountValidator;
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountCalculateBalancer _chartOfAccountCalculateBalancer;
    private readonly INameCodeAccountChanger _nameCodeAccountChanger;

    public ChartOfAccountDetailUpdater(
        ApplicationDbContext context,
        IChartOfAccountValidator chartOfAccountValidator,
        IChartOfAccountCalculateBalancer chartOfAccountCalculateBalancer,
        INameCodeAccountChanger nameCodeAccountChanger
    )
    {
        _chartOfAccountValidator = chartOfAccountValidator;
        _context = context;
        _chartOfAccountCalculateBalancer = chartOfAccountCalculateBalancer;
        _nameCodeAccountChanger = nameCodeAccountChanger;
    }
    public async Task<string> UpdateAccountDetail(ChartOfAccountModel model, int year)
    {
        try
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            var oldAccount = _context.GetChartOfAccount(year).FirstOrDefault(x => x.Id == model.Id);
            if (oldAccount is null)
                return string.Empty;
            var form = new ChartOfAccount
            {
                Code = model.Code,
                Name = model.Name,
            };
            //update goods
            if (oldAccount.StockUnit != model.StockUnit)
            {
                string acountCode = model.ParentRef;
                string detail1 = model.Code;
                string detail2 = "";
                if(oldAccount.Type == 6)
                {
                    string[] segments = oldAccount.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    detail2 = model.Code;
                    acountCode = segments[0];
                    detail1 = segments[1];
                }
                var goods = await _context.Goods.Where(x => x.Account == acountCode 
                            && (string.IsNullOrEmpty(x.Detail1) || x.Detail1 == detail1)
                            && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == detail2)).ToListAsync();
                goods = goods.ConvertAll(x =>
                {
                    x.StockUnit = model.StockUnit;
                    return x;
                });
                _context.Goods.UpdateRange(goods);
            }
            await _nameCodeAccountChanger.ChangeNameCodeAccount(form, oldAccount, year);


            oldAccount.StockUnitPrice = model.StockUnitPrice;
            oldAccount.StockUnitPriceNB = model.StockUnitPriceNB;
            oldAccount.OpeningDebit = model.OpeningDebit;
            oldAccount.OpeningDebitNB = model.OpeningDebitNB;
            oldAccount.OpeningStockQuantity = model.OpeningStockQuantity;
            oldAccount.OpeningStockQuantityNB = model.OpeningStockQuantityNB;
            oldAccount.OpeningForeignDebit = model.OpeningForeignDebit;
            oldAccount.OpeningForeignDebitNB = model.OpeningForeignDebitNB;
            oldAccount.OpeningCredit = model.OpeningCredit;
            oldAccount.OpeningCreditNB = model.OpeningCreditNB;
            oldAccount.OpeningForeignCreditNB = model.OpeningForeignCreditNB;
            oldAccount.OpeningForeignCredit = model.OpeningForeignCredit;
            oldAccount.StockUnit = model.StockUnit;
            oldAccount.Duration = model.Duration;
            oldAccount.AccGroup = model.AccGroup;
            oldAccount.Classification = model.Classification;
            oldAccount.Protected = model.Protected;
            oldAccount.Name = model.Name;
            oldAccount.Code = model.Code;
            oldAccount.IsForeignCurrency = model.IsForeignCurrency;

            var existingAccount = model.Code != oldAccount.Code
                ? await _context.GetChartOfAccount(year).SingleOrDefaultAsync(x => x.Code == model.Code && x.Id != model.Id)
                : null;
            var result =
                await _chartOfAccountValidator.ValidateUpdateAccountDetail(model, oldAccount, existingAccount, year);
            if (!string.IsNullOrEmpty(result.ErrorMessage))
                return result.ErrorMessage;

            result.Handlers.ForEach(item => item.Handle(oldAccount, year));
            if (existingAccount != null)
            {
                existingAccount.OpeningDebit += oldAccount.OpeningDebit;
                existingAccount.OpeningCredit += oldAccount.OpeningCredit;
                existingAccount.OpeningForeignCredit += oldAccount.OpeningForeignCredit;
                existingAccount.OpeningForeignDebit += oldAccount.OpeningForeignDebit;
                existingAccount.IsForeignCurrency = model.IsForeignCurrency;
                _context.ChartOfAccounts.Remove(oldAccount);
                _context.ChartOfAccounts.Update(existingAccount);
                await _context.SaveChangesAsync();
                await _chartOfAccountCalculateBalancer.CalculateBalance(existingAccount, year);
            }
            else
            {
                _context.ChartOfAccounts.Update(oldAccount);
                await _context.SaveChangesAsync();
                await _chartOfAccountCalculateBalancer.CalculateBalance(oldAccount, year);
            }

            await transaction.CommitAsync();
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }
    }

}
