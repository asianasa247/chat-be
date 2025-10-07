using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Certificates;

namespace ManageEmployee.Services;

public class CertificateService : ICertificateService
{
    private readonly ApplicationDbContext _context;

    public CertificateService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Certificate> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.Certificates
            .Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                                   x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
                                              );
        }

        return query
            .Skip(pageSize * currentPage)
            .Take(pageSize);
    }

    public IEnumerable<Certificate> GetAll()
    {
        return _context.Certificates
            .Where(x => !x.IsDelete)
                .OrderBy(x => x.Name);
    }

    public Certificate GetById(int id)
    {
        return _context.Certificates.Find(id);
    }

    public Certificate Create(Certificate param)
    {
        if (_context.Certificates.Where(u => u.Name == param.Name).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Certificates.Add(param);
        _context.SaveChanges();

        return param;
    }

    public async Task Update(Certificate param)
    {
        var certificate = await _context.Certificates.FindAsync(param.Id);

        if (certificate == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        certificate.Name = param.Name;
        certificate.Description = param.Description;
        certificate.Status = param.Status;
        certificate.CompanyId = param.CompanyId;

        certificate.UpdatedAt = DateTime.Now;
        certificate.UserUpdated = param.UserUpdated;

        _context.Certificates.Update(certificate);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var certificate = await _context.Certificates.FindAsync(id);
        if (certificate != null)
        {
            certificate.IsDelete = true;
            certificate.DeleteAt = DateTime.Now;
            _context.Certificates.Update(certificate);
            await _context.SaveChangesAsync();
        }
    }

    public int Count(string keyword)
    {
        var query = _context.Certificates.Where(x => !x.IsDelete);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                                   x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
                                              );
        }
        return query.Count();
    }
}