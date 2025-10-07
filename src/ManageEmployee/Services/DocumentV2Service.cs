using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.Documents;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class DocumentV2Service : IDocumentV2Service
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public DocumentV2Service(
        ApplicationDbContext context,
        IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<DocumentV2Model>> GetByUserAsync(string userId, int year)
    {
        var query = await _context.Documents.AsNoTracking()
            .Where(x => !x.IsDelete)
            .Where(x => x.UserId == userId || string.IsNullOrEmpty(x.UserId))
            .OrderBy(x => x.Name).ToListAsync();
        if (!query.Any())
        {
            return new List<DocumentV2Model>();
        }

        var result = _mapper.Map<List<DocumentV2Model>>(query);
        var accountCodes = new List<string>();
        accountCodes.AddRange(query.Select(s => s.DebitCode));
        accountCodes.AddRange(query.Select(s => s.DebitCodeFirst));
        accountCodes.AddRange(query.Select(s => s.DebitCodeSecond));
        accountCodes.AddRange(query.Select(s => s.CreditCode));
        accountCodes.AddRange(query.Select(s => s.CreditCodeFirst));
        accountCodes.AddRange(query.Select(s => s.CreditCodeSecond));
        accountCodes = accountCodes.Where(w => w is not null).ToList();

        var accounts = await _context.GetChartOfAccount(year).Where(x => accountCodes.Contains(x.Code)).ToListAsync();
        if (!accounts.Any())
        {
            return result;
        }
        for (int i = 0; i < query.Count; i++)
        {
            result[i].Debit = FindAccount(accounts, query[i].DebitCode);
            result[i].DebitDetailFirst = FindAccount(accounts, query[i].DebitCodeFirst);
            result[i].DebitDetailSecond = FindAccount(accounts, query[i].DebitCodeSecond);
            result[i].Credit = FindAccount(accounts, query[i].CreditCode);
            result[i].CreditDetailFirst = FindAccount(accounts, query[i].CreditCodeFirst);
            result[i].CreditDetailSecond = FindAccount(accounts, query[i].CreditCodeSecond);
        }
        return result;
    }

    public async Task<DocumentV2Model> GetByIdAsync(int id, int year)
    {
        var query = await  _context.Documents.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if(query is null)
        {
            return null;
        }
        var result = _mapper.Map<DocumentV2Model>(query);
        var accountCodes = new string[]
        {
            query.DebitCode, query.DebitCodeFirst, query.DebitCodeSecond, query.CreditCode, query.CreditCodeFirst, query.CreditCodeSecond
        };
        accountCodes = accountCodes.Where(w => w is not null).ToArray();
        if (!accountCodes.Any())
        {
            return result;
        }
        var accounts = await _context.GetChartOfAccount(year).Where(x => accountCodes.Contains(x.Code)).ToListAsync();
        result.Debit = FindAccount(accounts, query.DebitCode);
        result.DebitDetailFirst = FindAccount(accounts, query.DebitCodeFirst);
        result.DebitDetailSecond = FindAccount(accounts, query.DebitCodeSecond);
        result.Credit = FindAccount(accounts, query.CreditCode);
        result.CreditDetailFirst = FindAccount(accounts, query.CreditCodeFirst);
        result.CreditDetailSecond = FindAccount(accounts, query.CreditCodeSecond);
        if (!string.IsNullOrEmpty(result.UserId))
        {
            result.UserManager = await _context.Users.FirstOrDefaultAsync(f => f.Id == int.Parse(result.UserId));
        }
        return result;
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