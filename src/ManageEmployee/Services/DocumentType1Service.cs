using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Documents;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class DocumentType1Service : IDocumentType1Service
{
    private readonly ApplicationDbContext _context;

    public DocumentType1Service(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentType1> GetByIdAsync(int id)
    {
        return await _context.DocumentType1
            .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);
    }

    public async Task<PagingResult<DocumentType1>> GetAll(DocumentType1RequestModel param)
    {
        var query = _context.DocumentType1
            .Where(x => !x.IsDelete)
            .Where(x => param.FromAt == null || x.ToDate >= param.FromAt)
            .Where(x => param.ToAt == null || x.ToDate <= param.ToAt)
            ;
        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query.Where(x => x.DocumentTypeName.ToLower().Contains(param.SearchText)
                            || x.Content.ToLower().Contains(param.SearchText)
                            || x.TextSymbol.ToLower().Contains(param.SearchText)
                            || x.Signer.ToLower().Contains(param.SearchText)
                            || x.UnitName.ToLower().Contains(param.SearchText)
                            );
        }

        return new PagingResult<DocumentType1>()
        {
            Data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync(),
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task CreateAsync(DocumentType1 param)
    {
        param.DocumentTypeName = await _context.DocumentTypes.Where(x => x.Id == param.DocumentTypeId).Select(x => x.Name).FirstOrDefaultAsync();
        await _context.DocumentType1.AddAsync(param);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DocumentType1 param)
    {
        var data = await _context.DocumentType1.FindAsync(param.Id);

        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        data.DocumentTypeId = param.DocumentTypeId;
        data.DocumentTypeName = _context.DocumentTypes.FirstOrDefault(x => x.Id == param.DocumentTypeId)?.Name;
        data.ToDate = param.ToDate;
        data.UnitName = param.UnitName;
        data.TextSymbol = param.TextSymbol;
        data.DateText = param.DateText;
        data.BranchId = param.BranchId;
        data.DepartmentId = param.DepartmentId;
        data.DepartmentName = param.DepartmentName;
        data.Content = param.Content;
        data.ReceiverId = param.ReceiverId;
        data.ReceiverName = param.ReceiverName;
        if (!string.IsNullOrEmpty(param.FileUrl))
        {
            data.FileUrl = param.FileUrl;
            data.FileName = param.FileName;
        }
        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = param.UserUpdated;

        _context.DocumentType1.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var data = await _context.DocumentType1.FindAsync(id);
        if (data != null)
        {
            data.IsDelete = true;
            data.DeleteAt = DateTime.Now;
            _context.DocumentType1.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}