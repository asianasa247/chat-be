using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Documents;

namespace ManageEmployee.Services;

public class DocumentService : IDocumentService
{
    private readonly ApplicationDbContext _context;

    public DocumentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<DocumentModel.MapDocument> GetAll(string keyword = "")
    {
        var query = from p in _context.Documents
                    where p.IsDelete != true
                        && (!String.IsNullOrEmpty(keyword) ? (
                        p.Code.Trim().ToLower().Contains(keyword) || p.Name.Trim().ToLower().Contains(keyword) ||
                        p.NameDebitCode.Trim().ToLower().Contains(keyword) || p.NameCreditCode.Trim().ToLower().Contains(keyword) ||
                        p.UserCode.Trim().ToLower().Contains(keyword) || p.UserFullName.Trim().ToLower().Contains(keyword)
                        ) : p.Id != 0)
                    select new DocumentModel.MapDocument()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code,
                        Stt = p.Stt,
                        DebitCode = p.DebitCode,
                        NameDebitCode = p.NameDebitCode,
                        DebitCodeFirst = p.DebitCodeFirst,
                        DebitCodeFirstName = p.DebitCodeFirstName,
                        DebitCodeSecond = p.DebitCodeSecond,
                        DebitCodeSecondName = p.DebitCodeSecondName,

                        CreditCode = p.CreditCode,
                        NameCreditCode = p.NameCreditCode,
                        CreditCodeFirst = p.CreditCodeFirst,
                        CreditCodeFirstName = p.CreditCodeFirstName,
                        CreditCodeSecond = p.CreditCodeSecond,
                        CreditCodeSecondName = p.CreditCodeSecondName,

                        AllowDelete = p.AllowDelete,
                        Check = p.Check,
                        UserId = p.UserId,
                        UserCode = p.UserCode,
                        UserFullName = p.UserFullName,
                        Title = p.Title
                    };
        return query.OrderBy(x => x.Stt).ToList();
    }

    public IEnumerable<Document> GetAll()
    {
        return _context.Documents
            .Where(x => !x.IsDelete)
            .OrderBy(x => x.Name);
    }
    public Document GetById(int id)
    {
        return _context.Documents.Find(id);
    }

    public int GetLastIdentity()
    {
        var maxDocument = _context.Documents.Where(x => x.IsDelete == false).OrderByDescending(x => x.Stt).FirstOrDefault();
        return maxDocument != null ? maxDocument.Stt + 1 : 1;
    }

    public Document Create(Document param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        //param.NameCreditCode = _context.get?.FirstOrDefault(x => x.Code?.ToLower() == param.CreditCode?.ToLower())?.Name;
        //param.NameDebitCode = chartOfAccounts?.FirstOrDefault(x => x.Code?.ToLower() == param.DebitCode?.ToLower())?.Name;


        _context.Documents.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(Document param)
    {
        var documentCurrent = _context.Documents.Find(param.Id);

        if (documentCurrent == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        documentCurrent.UserId = param.UserId;
        documentCurrent.UserCode = param.UserCode;
        documentCurrent.UserFullName = param.UserFullName;
        documentCurrent.Stt = param.Stt;
        documentCurrent.Code = param.Code;
        documentCurrent.Name = param.Name;
        documentCurrent.CreditCode = param.CreditCode;
        documentCurrent.NameCreditCode = param.NameCreditCode;

        documentCurrent.CreditCodeFirst = param.CreditCodeFirst;
        documentCurrent.CreditCodeFirstName = param.CreditCodeFirstName;
        documentCurrent.CreditCodeSecond = param.CreditCodeSecond;
        documentCurrent.CreditCodeSecondName = param.CreditCodeSecondName;

        documentCurrent.DebitCode = param.DebitCode;
        documentCurrent.NameDebitCode = param.NameDebitCode;

        documentCurrent.DebitCodeFirst = param.DebitCodeFirst;
        documentCurrent.DebitCodeFirstName = param.DebitCodeFirstName;

        documentCurrent.DebitCodeSecond = param.DebitCodeSecond;
        documentCurrent.DebitCodeSecondName = param.DebitCodeSecondName;

        documentCurrent.Check = param.Check;
        documentCurrent.AllowDelete = param.AllowDelete;
        documentCurrent.UpdatedAt = DateTime.Now;
        documentCurrent.UserUpdated = param.UserUpdated;
        documentCurrent.Title = param.Title;

        _context.Documents.Update(documentCurrent);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var documentCurrent = _context.Documents.Find(id);
        if (documentCurrent != null)
        {
            documentCurrent.IsDelete = true;
            documentCurrent.DeleteAt = DateTime.Now;
            _context.Documents.Update(documentCurrent);
            _context.SaveChanges();
        }
    }

    public string GetDocumentTypeName(string voucherType)
    {
        if (string.IsNullOrWhiteSpace(voucherType))
            return string.Empty;

        var type = voucherType.ToLower();

        var result = _context.Documents.Where(x => x.Code.ToLower() == type)
            .Select(x => x.Name)
            .FirstOrDefault();

        return result ?? string.Empty;
    }

    public IEnumerable<Document> GetAllByUser(string userId)
    {
        return _context.Documents
            .Where(x => !x.IsDelete)
            .Where(x => x.UserId == userId || string.IsNullOrEmpty(x.UserId))
            .OrderBy(x => x.Name);
    }
}