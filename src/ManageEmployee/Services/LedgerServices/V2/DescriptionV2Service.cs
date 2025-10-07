using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.LedgerServices.V2;
public class DescriptionV2Service : IDescriptionV2Service
{
    private readonly ApplicationDbContext _context;
    public DescriptionV2Service(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DescriptionV2Model>> GetPage(int year, string documentCode)
    {
        var descriptions = await _context.Descriptions.Where(x => string.IsNullOrEmpty(documentCode) || x.DocumentCode == documentCode).ToListAsync();
        var debitCodes = descriptions.Select(x => x.DebitCode);
        var debitCodeFirsts = descriptions.Select(x => x.DebitDetailCodeFirst);
        var debitCodeSeconds = descriptions.Select(x => x.DebitDetailCodeSecond);
        var creditCodes = descriptions.Select(x => x.CreditCode);
        var creditCodeFirsts = descriptions.Select(x => x.CreditDetailCodeFirst);
        var creditCodeSeconds = descriptions.Select(x => x.CreditDetailCodeSecond);

        var accountCodes = debitCodes.Concat(debitCodeFirsts).Concat(debitCodeSeconds).Concat(creditCodes).Concat(creditCodeFirsts).Concat(creditCodeSeconds).Where(x => !string.IsNullOrEmpty(x))
                        .Distinct().ToList();

        var accounts = await _context.GetChartOfAccount(year).Where(x => accountCodes.Contains(x.Code)).ToListAsync();

        var response = descriptions
            .ConvertAll(x => new DescriptionV2Model
            {
                Id = x.Id,
                Name = x.Name,
                DocumentCode = x.DocumentCode,
                Debit = FindAccount(accounts, x.DebitCode),
                DebitDetailFirst = FindAccount(accounts, x.DebitDetailCodeFirst),
                DebitDetailSecond = FindAccount(accounts, x.DebitDetailCodeSecond),
                Credit = FindAccount(accounts, x.CreditCode),
                CreditDetailFirst = FindAccount(accounts, x.CreditDetailCodeFirst),
                CreditDetailSecond = FindAccount(accounts, x.CreditDetailCodeSecond),
            });


        return response;
    }

    public CommonModel FindAccount(List<ChartOfAccount> accounts, string code)
    {
        return accounts.Select(account => new CommonModel
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            IsForeignCurrency = account.IsForeignCurrency,
            AccGroup = account.AccGroup,
            Classification = account.Classification,
            HasDetails = account.HasDetails,
            WarehouseCode = account.WarehouseCode,
            DisplayInsert = account.DisplayInsert,
            Duration = account.Duration,
            Protected = account.Protected,
            ParentRef = account.ParentRef,

        }).FirstOrDefault(f => f.Code == code);
    }
}
