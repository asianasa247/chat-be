using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.Services.Interfaces.TaxRates;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class TaxRateService : ITaxRateService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    private readonly IMapper _mapper;

    public TaxRateService(ApplicationDbContext context, IChartOfAccountV2Service chartOfAccountV2Service, IMapper mapper)
    {
        _context = context;
        _chartOfAccountV2Service = chartOfAccountV2Service;
        _mapper = mapper;
    }

    public async Task<List<TaxRate>> GetPage(int currentPage, int pageSize)
    {
        if (currentPage == 0)
            currentPage = 1;
        return await _context.TaxRates.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
    }

    public async Task<IEnumerable<TaxRate>> GetAll()
    {
        return await _context.TaxRates.ToListAsync();
    }

    public async Task<TaxRateModel> GetById(long id, int year)
    {
        var taxRate = await _context.TaxRates.FindAsync(id);
        var model = _mapper.Map<TaxRateModel>(taxRate);
        model.Debit = await _chartOfAccountV2Service.FindAccount(taxRate.DebitCode, string.Empty, year);
        model.Credit = await _chartOfAccountV2Service.FindAccount(taxRate.CreditCode, string.Empty, year);
        model.DebitFirst = await _chartOfAccountV2Service.FindAccount(taxRate.DebitFirstCode, taxRate.DebitCode, year);
        model.CreditFirst = await _chartOfAccountV2Service.FindAccount(taxRate.CreditFirstCode, taxRate.CreditCode, year);
        model.DebitSecond = await _chartOfAccountV2Service.FindAccount(taxRate.DebitSecondCode, taxRate.DebitCode + ":" + taxRate.DebitFirstCode, year);
        model.CreditSecond = await _chartOfAccountV2Service.FindAccount(taxRate.CreditSecondCode, taxRate.CreditCode + ":" + taxRate.CreditFirstCode, year);
          
        return model;

    }

    public async Task<TaxRate> GetTaxTypeByCode(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return new TaxRate();
        }
        return await _context.TaxRates.FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<int> CountAll()
    {
        return await _context.TaxRates.CountAsync();
    }

    public async Task<string> Create(TaxRate entity)
    {
        if (await _context.TaxRates.AnyAsync(x => x.Code == entity.Code))
            return ErrorMessages.TaxCodeAlreadyExist;
        _context.TaxRates.AddRange(entity);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Update(TaxRate entity)
    {
        if (await _context.TaxRates.AnyAsync(x => x.Code == entity.Code && x.Id != entity.Id))
            return ErrorMessages.TaxCodeAlreadyExist;
        var existingEntity = await _context.TaxRates.FindAsync(entity.Id);
        if (existingEntity == null)
            return ErrorMessages.DataNotFound;
        _context.TaxRates.Update(entity);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Delete(long id)
    {
        var entity = await _context.TaxRates.FindAsync(id);
        if (entity == null)
            return ErrorMessages.DataNotFound;
        _context.TaxRates.Remove(entity);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<List<TaxRate>> GetListTaxRateStartWithR()
    {
        var taxRates = await _context.TaxRates.Where(x => x.Type == 1).OrderBy(x => x.Order).ToListAsync();
           return taxRates.ConvertAll(x => {x.Name = x.Code + "-" + x.Name + "-" + x.Percent.ToString() + "%"; return x; });
    }
}