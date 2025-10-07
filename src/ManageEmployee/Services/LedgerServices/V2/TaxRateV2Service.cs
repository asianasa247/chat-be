using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.LedgerServices.V2;
public class TaxRateV2Service : ITaxRateV2Service
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    public TaxRateV2Service(ApplicationDbContext context, IChartOfAccountV2Service chartOfAccountV2Service)
    {
        _context = context;
        _chartOfAccountV2Service = chartOfAccountV2Service;
    }
    public async Task<IEnumerable<TaxRateV2Model>> GetAll(int year)
    {
        var taxRates = await _context.TaxRates.OrderBy(x => x.Order).ToListAsync();
        var listOut = new List<TaxRateV2Model>();
        foreach (var taxRate in taxRates)
        {
            var itemOut = new TaxRateV2Model()
            {
                Debit = await _chartOfAccountV2Service.FindAccount(taxRate.DebitCode, string.Empty, year),
                Credit = await _chartOfAccountV2Service.FindAccount(taxRate.CreditCode, string.Empty, year),
                DebitFirst = await _chartOfAccountV2Service.FindAccount(taxRate.DebitFirstCode, taxRate.DebitCode, year),
                CreditFirst = await _chartOfAccountV2Service.FindAccount(taxRate.CreditFirstCode, taxRate.CreditCode, year),
                DebitSecond = await _chartOfAccountV2Service.FindAccount(taxRate.DebitSecondCode, taxRate.DebitCode + ":" + taxRate.DebitFirstCode, year),
                CreditSecond = await _chartOfAccountV2Service.FindAccount(taxRate.CreditSecondCode, taxRate.CreditCode + ":" + taxRate.CreditFirstCode, year),
                Code = taxRate.Code,
                Name = taxRate.Name,
                Description = taxRate.Description,
                Percent = taxRate.Percent
            };
            listOut.Add(itemOut);
        }

        return listOut;
    }
}