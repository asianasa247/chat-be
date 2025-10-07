using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Documents;

namespace ManageEmployee.Services;

public class DocumentTypeService : IDocumentTypeService
{
    private readonly ApplicationDbContext _context;

    public DocumentTypeService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<DocumentType> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.DocumentTypes
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

    public DocumentType Create(DocumentType param)
    {
        if (_context.DocumentTypes.Any(u => u.Name == param.Name && !u.IsDelete))
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.DocumentTypes.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(DocumentType param)
    {
        var data = _context.DocumentTypes.Find(param.Id);

        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        data.Name = param.Name;
        data.Description = param.Description;
        data.Status = param.Status;

        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = param.UserUpdated;

        _context.DocumentTypes.Update(data);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var data = _context.DocumentTypes.Find(id);
        if (data != null)
        {
            data.IsDelete = true;
            data.DeleteAt = DateTime.Now;
            _context.DocumentTypes.Update(data);
            _context.SaveChanges();
        }
    }

    public int Count(string keyword)
    {
        var query = _context.DocumentTypes.Where(x => !x.IsDelete);

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

    public IEnumerable<DocumentType> GetAllByActive()
    {
        return _context.DocumentTypes.Where(x => !x.IsDelete);
    }
}