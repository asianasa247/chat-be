using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Customers;

namespace ManageEmployee.Services.CustomerServices;

public class CustomerClassificationService : ICustomerClassificationService
{
    private ApplicationDbContext _context;

    public CustomerClassificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<CustomerClassification> GetAll()
    {
        return _context.CustomerClassifications
            .Where(x => !x.IsDelete)
            .OrderBy(x => x.Name);
    }

    public List<CustomerClassification> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.CustomerClassifications
            .Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()));
        }

        return query
            .Skip(pageSize * currentPage)
            .Take(pageSize).ToList();
    }

    public CustomerClassification GetById(int id)
    {
        try
        {
            return _context.CustomerClassifications.Where(x => x.Id == id).FirstOrDefault();
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public CustomerClassification Create(CustomerClassification param)
    {
        {
            if (_context.CustomerClassifications.Where(u => u.Name == param.Name).Any())
            {
                throw new ErrorException(ResultErrorConstants.NAME_EXIST);
            }

            if (string.IsNullOrWhiteSpace(param.Name))
                throw new ErrorException(ResultErrorConstants.MODEL_MISS);

            _context.CustomerClassifications.Add(param);
            _context.SaveChanges();

            return param;
        }
    }

    public void Update(CustomerClassification param)
    {
        var item = _context.CustomerClassifications.Find(param.Id);

        if (item == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        item.Name = param.Name;
        item.Status = param.Status;
        item.Color = param.Color;
        item.Purchase = param.Purchase;

        item.UpdatedAt = DateTime.Now;
        item.UserUpdated = param.UserUpdated;

        _context.CustomerClassifications.Update(item);
        _context.SaveChanges();
    }

    public string Delete(int id)
    {
        try
        {
            _context.Database.BeginTransactionAsync();

            var result = _context.CustomerClassifications.Find(id);

            if (result != null)
            {
                _context.CustomerClassifications.Remove(result);
            }
            _context.SaveChanges();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }
}