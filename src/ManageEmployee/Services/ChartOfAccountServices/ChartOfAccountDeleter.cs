using Emgu.CV;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChartOfAccountServices;
public class ChartOfAccountDeleter: IChartOfAccountDeleter
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountCalculateBalancer _chartOfAccountCalculateBalancer;
    public ChartOfAccountDeleter(
        ApplicationDbContext context, 
        IChartOfAccountCalculateBalancer chartOfAccountCalculateBalancer
    )
    {
        _context = context;
        _chartOfAccountCalculateBalancer = chartOfAccountCalculateBalancer;
    }
    public async Task Delete(long id, int year)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        var account = await _context.ChartOfAccounts.FindAsync(id);
        if (account is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var dataDels  = await ValidateAsync(account, year);

        if (account.Code.Length == 3)
        {
            _context.ChartOfAccounts.Remove(account);
            await _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return;
        }
        var accountSame = await _context.GetChartOfAccount(year).Where(x => x.ParentRef == account.ParentRef).CountAsync();
        if (accountSame < 2)
        {
            var listAccountChild = await _context.GetChartOfAccount(year).Where(x => x.ParentRef.Substring(0, account.Code.Length) == account.Code).ToListAsync();
            if (listAccountChild.Count > 0)
            {
                listAccountChild.ForEach(x => x.ParentRef = x.ParentRef.Substring(0, account.Code.Length - 1));
            }
        }
        _context.ChartOfAccounts.Remove(account);
        if (dataDels.Any())
        {
            foreach(var data in dataDels)
            {
                if(data.tableName == nameof(Goods))
                {
                    var goodDels = await _context.Goods.Where(x => data.tableIds.Contains(x.Id)).ToListAsync();
                    _context.Goods.RemoveRange(goodDels);
                }
            }
        }

        await _context.SaveChangesAsync();
        await _chartOfAccountCalculateBalancer.CalculateBalance(account, year);
        await transaction.CommitAsync();
    }
    //(string tableName, List<int> tableIds)
    private async Task<List<(string tableName, List<int> tableIds)>> ValidateAsync(ChartOfAccount oldAccount, int year)
    {
        string parentCode = "";
        string grandCode = "";
        List<(string, List<int>)> dataDels = new();

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

        var goodIds = await _context.Goods.Where(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                   || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                   || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode))
            .Select(x => x.Id).ToListAsync();
        // check good was sale
        var isSaleGood = await _context.BillDetails.AnyAsync(x => x.CreatedDate.Year == year && goodIds.Contains(x.GoodsId));
        if (goodIds.Any() && isSaleGood)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "HÀNG HÓA"));
        dataDels.Add(new (nameof(Goods), goodIds));

        var isExistLedgerCredit = await _context.GetLedger(year).AnyAsync(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                            || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                            || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode)
                                                            );
        if (isExistLedgerCredit)
            throw new ErrorException(ChartOfAccountResultErrorConstants.ACCOUNT_USING);

        var isExistLedgerDebit = await _context.GetLedger(year).AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                            || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                            || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode));
        if (isExistLedgerDebit)
            throw new ErrorException(ChartOfAccountResultErrorConstants.ACCOUNT_USING);

        var isExistAccountPay = await _context.AccountPays.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistAccountPay)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "TÀI KHOẢN THANH TOÁN"));


        var isExistBillPromotion = await _context.BillPromotions.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistBillPromotion)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "KHUYẾN MÃI"));

        var isExistChartOfAccountGroupLink = await _context.ChartOfAccountGroupLinks.AnyAsync(x => x.CodeChartOfAccount == oldAccount.Code);
        if (isExistChartOfAccountGroupLink)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "TÀI KHOẢN ĐỒNG BỘ"));

        var isExistCustomer = await _context.Customers.AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode));
        if (isExistCustomer)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "KHÁCH HÀNG"));

        var isExistDocumentDebit = await _context.Documents.AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitCodeFirst == parentCode));
        if (isExistDocumentDebit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "TÀI LIỆU"));

        var isExistDocumentCredit = await _context.Documents.AnyAsync(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditCodeFirst == parentCode));
        if (isExistDocumentCredit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "TÀI LIỆU"));

        var isExistFixedAsset242Credit = await _context.FixedAsset242.AnyAsync(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode));
        if (isExistFixedAsset242Credit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "CÔNG CỤ DỤNG CỤ"));

        var isExistFixedAsset242Debit = await _context.FixedAsset242.AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode));
        if (isExistFixedAsset242Debit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "CÔNG CỤ DỤNG CỤ"));

        var isExistFixedAssetUserCredit = await _context.FixedAssetUser.AnyAsync(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode));
        if (isExistFixedAssetUserCredit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "CÔNG CỤ DỤNG CỤ"));

        var isExistFixedAssetUserDebit = await _context.FixedAssetUser.AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode));
        if (isExistFixedAssetUserDebit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "CÔNG CỤ DỤNG CỤ"));

        var isExistFixedAssetCredit = await _context.FixedAssets.AnyAsync(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode));
        if (isExistFixedAssetCredit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "CÔNG CỤ DỤNG CỤ"));

        var isExistFixedAssetDebit = await _context.FixedAssets.AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode));
        if (isExistFixedAssetDebit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "CÔNG CỤ DỤNG CỤ"));

        var isExistGoodDetail = await _context.GoodDetails.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistGoodDetail)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "ĐỊNH MỨC HÀNG HÓA"));

        var isExistGoodWarehouse = await _context.GoodWarehouses.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistGoodWarehouse)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "HÀNG HÓA TRONG KHO"));

        var isExistGoodsPromotionDetail = await _context.GoodsPromotionDetails.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistGoodsPromotionDetail)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "HÀNG HÓA KHUYẾN MÃI"));

        var isExistGoodsQuotaDetail = await _context.GoodsQuotaDetails.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistGoodsQuotaDetail)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "ĐỊNH MỨC HÀNG HÓA"));

        var isExistInventory = await _context.Inventory.AnyAsync(x => (oldAccount.Type < 5 && x.Account == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.Detail1 == oldAccount.Code && x.Account == parentCode)
                                                    || (oldAccount.Type == 6 && x.Detail2 == oldAccount.Code && x.Account == grandCode && x.Detail1 == parentCode));
        if (isExistInventory)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "HÀNG TỒN KHO"));

        var isExistLedgerProcedureProductDetailCredit = await _context.LedgerProcedureProductDetails.AnyAsync(x => (oldAccount.Type < 5 && x.CreditCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.CreditDetailCodeFirst == oldAccount.Code && x.CreditCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.CreditDetailCodeSecond == oldAccount.Code && x.CreditCode == grandCode && x.CreditDetailCodeFirst == parentCode));
        if (isExistLedgerProcedureProductDetailCredit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "QUY TRÌNH NHẬP XUẤT"));

        var isExistLedgerProcedureProductDetailDebit = await _context.LedgerProcedureProductDetails.AnyAsync(x => (oldAccount.Type < 5 && x.DebitCode == oldAccount.Code)
                                                    || (oldAccount.Type == 5 && x.DebitDetailCodeFirst == oldAccount.Code && x.DebitCode == parentCode)
                                                    || (oldAccount.Type == 6 && x.DebitDetailCodeSecond == oldAccount.Code && x.DebitCode == grandCode && x.DebitDetailCodeFirst == parentCode));
        if (isExistLedgerProcedureProductDetailDebit)
            throw new ErrorException(string.Format(ChartOfAccountResultErrorConstants.ACCOUNT_USING_OTHER_TABLE, "QUY TRÌNH NHẬP XUẤT"));

        return dataDels;
    }
}
