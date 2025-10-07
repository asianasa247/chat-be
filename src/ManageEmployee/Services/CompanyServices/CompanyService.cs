using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.CompanyModels;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.CompanyServices;

public class CompanyService : ICompanyService
{
    private readonly ApplicationDbContext _context;
    private readonly IAesEncryptionHelper _aes;

    public CompanyService(ApplicationDbContext context, IAesEncryptionHelper aes)
    {
        _context = context;
        _aes = aes;
    }

    public async Task<Company> GetCompany()
    {
        var company = await _context.Companies.OrderByDescending(x => x.SignDate).FirstOrDefaultAsync();
        if (company == null)
            company = new Company();
        return company;
    }

    public async Task<Company> GetById(int id)
    {   var result = await _context.Companies.FindAsync(id);
        return result;
    }

    public async Task<Company> Update(CompanyViewModel param)
    {
        var company = await _context.Companies.FirstOrDefaultAsync();

        if (company == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        company.Name = param.Name;
        company.Address = param.Address;
        company.Phone = param.Phone;
        company.MST = param.MST;
        company.Email = param.Email;
        company.Fax = param.Fax;
        company.WebsiteName = param.WebsiteName;

        company.NameOfCEO = param.NameOfCEO;
        company.NoteOfCEO = param.NoteOfCEO;
        company.NameOfChiefAccountant = param.NameOfChiefAccountant;
        company.NoteOfChiefAccountant = param.NoteOfChiefAccountant;
        company.NameOfTreasurer = param.NameOfTreasurer;
        company.NameOfStorekeeper = param.NameOfStorekeeper;
        company.NameOfChiefSupplier = param.NameOfChiefSupplier;
        company.NoteOfChiefSupplier = param.NoteOfChiefSupplier;
        if (!string.IsNullOrEmpty(param.FileOfBusinessRegistrationCertificate))
        {
            company.FileOfBusinessRegistrationCertificate = param.FileOfBusinessRegistrationCertificate;
        }
        company.AssignPerson = param.AssignPerson;
        company.SignDate = param.SignDate;
        company.CharterCapital = (double)param.CharterCapital;
        company.BusinessType = (int)param.BusinessType;
        company.AccordingAccountingRegime = (int)param.AccordingAccountingRegime;
        company.MethodCalcExportPrice = (int)param.MethodCalcExportPrice;
        company.Note = param.Note;
        if (!string.IsNullOrEmpty(param.FileLogo))
        {
            company.FileLogo = param.FileLogo;
        }

        company.UpdateAt = param.UpdateAt;
        company.UserUpdated = param.UserUpdated;
        company.IsShowBarCode = param.IsShowBarCode;
        company.Defautl_themeweb = param.Defautl_themeweb;
        company.Signature = param.Signature;
        company.IsImage = param.IsImage;
        company.FacebookAppId = param.FacebookAppId;
        company.GoogleAppId = param.GoogleAppId;
        if (!string.IsNullOrWhiteSpace(param.PasswordAccountTax) && company.PasswordAccountTax != param.PasswordAccountTax)
        {
            company.PasswordAccountTax = _aes.Encrypt(param.PasswordAccountTax);
        }
        else
        {
            company.PasswordAccountTax = param.PasswordAccountTax;
        }
        company.InvName = param.InvName;
        company.InvSerial = param.InvSerial;
        company.ClientID = param.ClientID;
        company.ClientSecret = param.ClientSecret;  
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();

        return company;
    }

    public async Task<List<Company>> GetAll(int currentPage, int pageSize)
    {
        List<Company> listCompany = await _context.Companies
            .AsNoTracking()
            .OrderByDescending(x => x.SignDate)
            .Skip(pageSize * (currentPage - 1))
            .Take(pageSize).ToListAsync();
        return listCompany;
    }

    public async Task Create(Company param)
    {
        var itemExist = await _context
            .Companies
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == param.Id);
        if (!string.IsNullOrWhiteSpace(param.PasswordAccountTax))
        {
            param.PasswordAccountTax = _aes.Encrypt(param.PasswordAccountTax);
        }
        if (itemExist != null)
        {
            if (string.IsNullOrEmpty(param.FileOfBusinessRegistrationCertificate))
            {
                param.FileOfBusinessRegistrationCertificate = itemExist.FileOfBusinessRegistrationCertificate;
            }

            if (string.IsNullOrEmpty(param.FileLogo))
            {
                param.FileLogo = itemExist.FileLogo;
            }

            _context.Entry(param).State = EntityState.Detached;
            await _context.Companies.AddAsync(param);
        }
        else
        {   
            param.Id = 0;
            await _context.Companies.AddAsync(param);
        }

        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var companyLog = await _context.Companies.FindAsync(id);
        if (companyLog != null)
        {
            _context.Companies.Remove(companyLog);
            await _context.SaveChangesAsync();
        }
    }
}