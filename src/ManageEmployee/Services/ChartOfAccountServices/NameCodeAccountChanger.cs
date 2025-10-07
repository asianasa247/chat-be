using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChartOfAccountServices;
public class NameCodeAccountChanger: INameCodeAccountChanger
{
    private readonly ApplicationDbContext _context;
    public NameCodeAccountChanger(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task ChangeNameCodeAccount(ChartOfAccount form, ChartOfAccount oldAccount, int year)
    {
        if (oldAccount.Code == form.Code && oldAccount.Name == form.Name)
            return;

        string parentCode = "";
        string grandCode = "";

        if (oldAccount.Type == 5)
        {
            parentCode = oldAccount.ParentRef;
        }
        else if (oldAccount.Type == 6)
        {
            var parents = oldAccount.ParentRef.Split(":");
            parentCode = parents[1];
            grandCode = parents[0];
        }

        var listGood = await _context.Goods.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var listLedgerCredit = await _context.GetLedger(year).Where(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                            || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                            || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode)
                                                            ).ToListAsync();
        var listLedgerDebit = await _context.GetLedger(year).Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                            || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                            || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode)).ToListAsync();
        var accountPays = await _context.AccountPays.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var billPromotions = await _context.BillPromotions.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var chartOfAccountGroupLinks = await _context.ChartOfAccountGroupLinks.Where(x => x.CodeChartOfAccount == oldAccount.Code).ToListAsync();
        var customers = await _context.Customers.Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode)).ToListAsync();
        var documentDebits = await _context.Documents.Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitCodeFirst == parentCode)).ToListAsync();
        var documentCredits = await _context.Documents.Where(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditCodeFirst == parentCode)).ToListAsync();
        var fixedAsset242Credits = await _context.FixedAsset242.Where(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode)).ToListAsync();
        var fixedAsset242Debits = await _context.FixedAsset242.Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode)).ToListAsync();
        var fixedAssetUserCredits = await _context.FixedAssetUser.Where(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode)).ToListAsync();
        var fixedAssetUserDebits = await _context.FixedAssetUser.Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode)).ToListAsync();
        var fixedAssetCredits = await _context.FixedAssets.Where(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode)).ToListAsync();
        var fixedAssetDebits = await _context.FixedAssets.Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode)).ToListAsync();
        var goodDetails = await _context.GoodDetails.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var goodWarehouses = await _context.GoodWarehouses.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var goodsPromotionDetails = await _context.GoodsPromotionDetails.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var goodsQuotaDetails = await _context.GoodsQuotaDetails.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var inventory = await _context.Inventory.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode)).ToListAsync();
        var ledgerProcedureProductDetailCredits = await _context.LedgerProcedureProductDetails.Where(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode)).ToListAsync();
        var ledgerProcedureProductDetailDebits = await _context.LedgerProcedureProductDetails.Where(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode)).ToListAsync();



        listGood = listGood.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.DetailName1 = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.DetailName2 = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.Goods.UpdateRange(listGood);

        listLedgerCredit = listLedgerCredit.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.CreditDetailCodeFirst = form.Code;
                    x.CreditDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.CreditDetailCodeSecond = form.Code;
                    x.CreditDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.CreditCode = form.Code;
                    x.CreditCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.Ledgers.UpdateRange(listLedgerCredit);

        listLedgerDebit = listLedgerDebit.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitDetailCodeFirst = form.Code;
                    x.DebitDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.DebitDetailCodeSecond = form.Code;
                    x.DebitDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.DebitCode = form.Code;
                    x.DebitCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.Ledgers.UpdateRange(listLedgerDebit);

        accountPays = accountPays.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.DetailName1 = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.DetailName2 = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.AccountPays.UpdateRange(accountPays);

        billPromotions = billPromotions.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.Detail1Name = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.Detail2Name = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.BillPromotions.UpdateRange(billPromotions);

        chartOfAccountGroupLinks = chartOfAccountGroupLinks.ConvertAll(x =>
        {
            x.CodeChartOfAccount = form.Code;
            return x;
        });
        _context.ChartOfAccountGroupLinks.UpdateRange(chartOfAccountGroupLinks);

        customers = customers.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitDetailCodeFirst = form.Code;
                    break;
                case 6:
                    x.DebitDetailCodeSecond = form.Code;
                    break;
                default:
                    x.DebitCode = form.Code;
                    break;
            }
            return x;
        });
        _context.Customers.UpdateRange(customers);

        documentDebits = documentDebits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitCodeFirst = form.Code;
                    x.DebitCodeFirstName = form.Name;
                    break;
                case 6:
                    x.DebitCodeSecond = form.Code;
                    x.DebitCodeSecondName = form.Name;
                    break;
                default:
                    x.DebitCode = form.Code;
                    x.NameDebitCode = form.Name;
                    break;
            }
            return x;
        });
        _context.Documents.UpdateRange(documentDebits);

        documentCredits = documentCredits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.CreditCodeFirst = form.Code;
                    x.CreditCodeFirstName = form.Name;
                    break;
                case 6:
                    x.CreditCodeSecond = form.Code;
                    x.CreditCodeSecondName = form.Name;
                    break;
                default:
                    x.CreditCode = form.Code;
                    x.NameCreditCode = form.Name;
                    break;
            }
            return x;
        });
        _context.Documents.UpdateRange(documentCredits);

        fixedAsset242Credits = fixedAsset242Credits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.CreditDetailCodeFirst = form.Code;
                    x.CreditDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.CreditDetailCodeSecond = form.Code;
                    x.CreditDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.CreditCode = form.Code;
                    x.CreditCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.FixedAsset242.UpdateRange(fixedAsset242Credits);

        fixedAsset242Debits = fixedAsset242Debits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitDetailCodeFirst = form.Code;
                    x.DebitDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.DebitDetailCodeSecond = form.Code;
                    x.DebitDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.DebitCode = form.Code;
                    x.DebitCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.FixedAsset242.UpdateRange(fixedAsset242Debits);

        fixedAssetUserCredits = fixedAssetUserCredits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.CreditDetailCodeFirst = form.Code;
                    x.CreditDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.CreditDetailCodeSecond = form.Code;
                    x.CreditDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.CreditCode = form.Code;
                    x.CreditCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.FixedAssetUser.UpdateRange(fixedAssetUserCredits);

        fixedAssetUserDebits = fixedAssetUserDebits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitDetailCodeFirst = form.Code;
                    x.DebitDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.DebitDetailCodeSecond = form.Code;
                    x.DebitDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.DebitCode = form.Code;
                    x.DebitCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.FixedAssetUser.UpdateRange(fixedAssetUserDebits);

        fixedAssetCredits = fixedAssetCredits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.CreditDetailCodeFirst = form.Code;
                    x.CreditDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.CreditDetailCodeSecond = form.Code;
                    x.CreditDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.CreditCode = form.Code;
                    x.CreditCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.FixedAssets.UpdateRange(fixedAssetCredits);

        fixedAssetDebits = fixedAssetDebits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitDetailCodeFirst = form.Code;
                    x.DebitDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.DebitDetailCodeSecond = form.Code;
                    x.DebitDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.DebitCode = form.Code;
                    x.DebitCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.FixedAssets.UpdateRange(fixedAssetDebits);

        goodDetails = goodDetails.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.DetailName1 = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.DetailName2 = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.GoodDetails.UpdateRange(goodDetails);

        goodWarehouses = goodWarehouses.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.DetailName1 = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.DetailName2 = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.GoodWarehouses.UpdateRange(goodWarehouses);

        goodsPromotionDetails = goodsPromotionDetails.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.Detail1Name = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.Detail2Name = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.GoodsPromotionDetails.UpdateRange(goodsPromotionDetails);

        goodsQuotaDetails = goodsQuotaDetails.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.DetailName1 = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.DetailName2 = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.GoodsQuotaDetails.UpdateRange(goodsQuotaDetails);

        inventory = inventory.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.Detail1 = form.Code;
                    x.DetailName1 = form.Name;
                    break;
                case 6:
                    x.Detail2 = form.Code;
                    x.DetailName2 = form.Name;
                    break;
                default:
                    x.Account = form.Code;
                    x.AccountName = form.Name;
                    break;
            }
            return x;
        });
        _context.Inventory.UpdateRange(inventory);

        ledgerProcedureProductDetailCredits = ledgerProcedureProductDetailCredits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.CreditDetailCodeFirst = form.Code;
                    x.CreditDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.CreditDetailCodeSecond = form.Code;
                    x.CreditDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.CreditCode = form.Code;
                    x.CreditCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.LedgerProcedureProductDetails.UpdateRange(ledgerProcedureProductDetailCredits);

        ledgerProcedureProductDetailDebits = ledgerProcedureProductDetailDebits.ConvertAll(x =>
        {
            switch (oldAccount.Type)
            {
                case 5:
                    x.DebitDetailCodeFirst = form.Code;
                    x.DebitDetailCodeFirstName = form.Name;
                    break;
                case 6:
                    x.DebitDetailCodeSecond = form.Code;
                    x.DebitDetailCodeSecondName = form.Name;
                    break;
                default:
                    x.DebitCode = form.Code;
                    x.DebitCodeName = form.Name;
                    break;
            }
            return x;
        });
        _context.LedgerProcedureProductDetails.UpdateRange(ledgerProcedureProductDetailDebits);

        await _context.SaveChangesAsync();
    }

}
