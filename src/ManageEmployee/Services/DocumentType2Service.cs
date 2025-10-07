using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Documents;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class DocumentType2Service : IDocumentType2Service
{
    private readonly ApplicationDbContext _context;

    public DocumentType2Service(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<DocumentType2>> GetAll(DocumentType1RequestModel param)
    {
        var query = _context.DocumentType2
            .Where(x => !x.IsDelete)
            .Where(x => param.FromAt == null || x.DateText >= param.FromAt)
            .Where(x => param.ToAt == null || x.DateText <= param.ToAt);

        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query.Where(x => x.DepartmentName.ToLower().Contains(param.SearchText.Trim())
                                     || x.Content.Trim().Contains(param.SearchText.Trim())
                                     || x.TextSymbol.Trim().Contains(param.SearchText.Trim())
                                     || x.SignerTextName.Trim().Contains(param.SearchText.Trim())
                                              );
        }

        return new PagingResult<DocumentType2>()
        {
            Data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync(),
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<DocumentType2> GetByIdAsyncd(int id)
    {
        return await _context.DocumentType2
            .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
    }

    public async Task CreateAsync(DocumentType2 param)
    {
        param.DocumentName = await _context.Documents.Where(x => x.Id == param.DocumentId).Select(x => x.Name).FirstOrDefaultAsync();
        _context.DocumentType2.Add(param);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DocumentType2 param)
    {
        var data = await _context.DocumentType2.FindAsync(param.Id);

        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        data.DocumentId = param.DocumentId;
        data.DocumentName = _context.Documents.FirstOrDefault(x => x.Id == param.DocumentId)?.Name;
        data.TextSymbol = param.TextSymbol;
        data.DateText = param.DateText;
        data.BranchId = param.BranchId;
        data.DepartmentId = param.DepartmentId;
        data.DepartmentName = _context.Departments.FirstOrDefault(x => x.Id == param.DepartmentId)?.Name;
        data.Content = param.Content;
        data.SignerTextId = param.SignerTextId;
        data.SignerTextName = _context.Users.FirstOrDefault(x => x.Id == param.SignerTextId)?.FullName;
        data.Recipient = param.Recipient;
        data.DraftarId = param.DraftarId;
        if (!string.IsNullOrEmpty(param.FileUrl))
        {
            data.FileUrl = param.FileUrl;
            data.FileName = param.FileName;
        }

        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = param.UserUpdated;

        _context.DocumentType2.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var data = await _context.DocumentType2.FindAsync(id);
        if (data != null)
        {
            data.IsDelete = true;
            data.DeleteAt = DateTime.Now;
            _context.DocumentType2.Update(data);
            await _context.SaveChangesAsync();
        }
    }

}