using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ChartOfAccountServices;
public class ChartOfAccountUpdater: IChartOfAccountUpdater
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountCalculateBalancer _chartOfAccountCalculateBalancer;
    private readonly INameCodeAccountChanger _nameCodeAccountChanger;
    public ChartOfAccountUpdater(
        ApplicationDbContext context,
        IChartOfAccountCalculateBalancer chartOfAccountCalculateBalancer,
        INameCodeAccountChanger nameCodeAccountChanger
    )
    {
        _context = context;
        _chartOfAccountCalculateBalancer = chartOfAccountCalculateBalancer;
        _nameCodeAccountChanger = nameCodeAccountChanger;
    }
    public async Task<string> Update(ChartOfAccount entity, int year)
    {
        try
        {
            if (entity.Year == 0)
            {
                entity.Year = year;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            var isExistAccountGroup = await _context.GetChartOfAccountGroupLink(year).AnyAsync(x => x.CodeChartOfAccount == entity.ParentRef);
            if (isExistAccountGroup)
                return ChartOfAccountResultErrorConstants.ACCOUNT_GROUP_LINK;

            var isExistAccountCode = await _context.GetChartOfAccount(year).AnyAsync(x => x.Code == entity.Code && x.Id != entity.Id);
            if (isExistAccountCode)
                return ChartOfAccountResultErrorConstants.ACCOUNT_EXIST;

            int lengthCode = entity.Code.Length;
            var accountCheck = await _context.ChartOfAccounts.FirstOrDefaultAsync(x => x.Id == entity.Id);

            await _nameCodeAccountChanger.ChangeNameCodeAccount(entity, accountCheck, year);

            if (lengthCode > 3)
            {
                var accountParent = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.Code.Substring(0, lengthCode - 1));
                if (accountParent == null)
                    return ChartOfAccountResultErrorConstants.ACCOUNT_PARENT_NOT_EXIST;

                if (accountParent.HasDetails)
                {
                    accountParent.HasDetails = false;
                    string parentRef = accountParent.Code.Substring(0, lengthCode - 2);
                    var listAccount = _context.GetChartOfAccount(year).Where(x => x.ParentRef == parentRef).ToList();
                    listAccount.ForEach(x => x.ParentRef = parentRef);
                }
            }
            _context.ChartOfAccounts.Update(entity);
            await _context.SaveChangesAsync();
            await _chartOfAccountCalculateBalancer.CalculateBalance(entity, year);
            await transaction.CommitAsync();
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }
    }

}
