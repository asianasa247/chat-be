using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.Entities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Services.CompanyServices;
using ManageEmployee.Services.Interfaces.AdditionWebServices;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.AdditionWebServices;

public class AdditionWebService : AdditionWebServiceBase, IAdditionWebService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<AdditionWebService> _logger;
    public AdditionWebService(ApplicationDbContext context, IDbContextFactory dbContextFactory, IMapper mapper, ILogger<AdditionWebService> logger) : base(dbContextFactory)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<AdditionWeb>> GetAllAsync()
    {
        return await _context.AdditionWebs.ToListAsync();
    }

    public async Task<AdditionWeb> GetByIdAsync(int id)
    {
        return await _context.AdditionWebs.FindAsync(id);
    }

    public async Task<AdditionWeb> AddOrUpdateAsync(AdditionWebModel model)
    {
        var entity = await _context.AdditionWebs
            .FirstOrDefaultAsync(x => x.DbName == model.DbName);

        if (entity != null)
        {
            entity.UrlWeb = model.UrlWeb;
            entity.ConnectionString = model.ConnectionString;
            entity.ImageHost = model.ImageHost;
            entity.IsActive = model.IsActive;
            _context.AdditionWebs.Update(entity);
        }
        else
        {
            entity = _mapper.Map<AdditionWeb>(model);
            await _context.AdditionWebs.AddAsync(entity);
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<AdditionWeb> UpdateAsync(int id, AdditionWebModel model)
    {
        var entity = await _context.AdditionWebs.FindAsync(id);
        if (entity == null) return null;
        _mapper.Map(model, entity);
        entity.UrlWeb = model.UrlWeb;
        entity.DbName = model.DbName;
        entity.ConnectionString = model.ConnectionString;
        entity.ImageHost = model.ImageHost;
        entity.IsActive = model.IsActive;
        _context.AdditionWebs.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _context.AdditionWebs.FindAsync(id);
        if (entity != null)
        {
            _context.AdditionWebs.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<AdditionWebCompanyResult> GetCompanyInfo(int id)
    {
        var additionWeb = await _context.AdditionWebs.FirstOrDefaultAsync(x => x.Id == id)
          ?? throw new Exception("Không tìm thấy website");

        return await GetCompanyInfo(additionWeb);
    }

    public async Task<List<AdditionWebCompanyResult>> GetCompaniesInfo()
    {
        var additionWebs = await _context.AdditionWebs
            .AsNoTracking()
            .ToListAsync();

        var companyTasks = new List<Task<AdditionWebCompanyResult>>();

        foreach (var additionWeb in additionWebs)
        {
            companyTasks.Add(GetCompanyInfo(additionWeb));
        }

        var companies = await Task.WhenAll(companyTasks);
        return companies.Where(x => x != null && x.Id > 0).ToList();
    }

    private async Task<AdditionWebCompanyResult> GetCompanyInfo(AdditionWeb additionWeb)
    {
        try
        {
            using var currentDbContext = GetApplicationDbContext(additionWeb);

            var companyService = new CompanyService(currentDbContext, null);

            var company = await companyService.GetCompany();

            return ToCompanyResult(company, additionWeb);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching company info for AdditionWeb ID {AdditionWebId}", additionWeb.Id);
            return null;
        }
    }

    private AdditionWebCompanyResult ToCompanyResult(Company company, AdditionWeb additionWeb)
    {
        if (company == null)
        {
            return null;
        }

        var result = _mapper.Map<AdditionWebCompanyResult>(company);
        if (string.IsNullOrWhiteSpace(additionWeb.ImageHost))
        {
            return result;
        }

        result.FullLogo = !string.IsNullOrWhiteSpace(result.FileLogo) ? $"{additionWeb.ImageHost}/{result.FileLogo}" : string.Empty;

        return result;
    }

    public async Task<List<AdditionWebCompanyShortResult>> GetCompaniesShortInfo()
    {
        var companies = await GetCompaniesInfo();
        return _mapper.Map<List<AdditionWebCompanyShortResult>>(companies);
    }
}
