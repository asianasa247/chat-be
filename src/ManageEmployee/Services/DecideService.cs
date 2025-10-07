using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DecideEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Decides;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class DecideService : IDecideService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public DecideService(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task<PagingResult<Decide>> GetAll(int currentPage, int pageSize, string keyword)
    {
        var query = _context.Decide
            .Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.EmployeesName.ToLower().Contains(keyword.Trim().ToLower()) ||
                                                   x.DecideTypeName.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                              x.Code.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                              x.Note.Trim().ToLower().Contains(keyword.Trim().ToLower()) ||
                                              x.Description.Trim().ToLower().Contains(keyword.Trim().ToLower())
                                              );
        }

        return new PagingResult<Decide>
        {
            Data = await query.Skip(pageSize * currentPage).Take(pageSize).ToListAsync(),
            CurrentPage = currentPage,
            PageSize = pageSize,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<Decide> GetById(int id)
    {
        return await _context.Decide.FindAsync(id);
    }
    public async Task Create(Decide param)
    {
        param.EmployeesName = await _context.Users.Where(x => x.Id == param.EmployeesId).Select(X => X.FullName).FirstOrDefaultAsync();
        param.DecideTypeName = await _context.DecisionType.Where(x => x.Id == param.DecideTypeId).Select(X => X.Name).FirstOrDefaultAsync();
        _context.Decide.Add(param);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Decide param, IFormFile file)
    {
        var data = await _context.Decide.FindAsync(param.Id);

        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        if (file != null && data.FileUrl != file.FileName)
            param.FileUrl = _fileService.Upload(file, "Decide");
        data.Type = param.Type;
        data.EmployeesId = param.EmployeesId;
        data.EmployeesName = await _context.Users.Where(x => x.Id == param.EmployeesId).Select(X => X.FullName).FirstOrDefaultAsync();
        data.DecideTypeId = param.DecideTypeId;
        data.DecideTypeName = await _context.DecisionType.Where(x => x.Id == param.DecideTypeId).Select(X => X.Name).FirstOrDefaultAsync();
        data.Code = param.Code;
        data.Date = param.Date;
        data.Description = param.Description;
        data.Note = param.Note;
        data.FileUrl = param.FileUrl;

        data.UpdatedAt = DateTime.Now;
        data.UserUpdated = param.UserUpdated;

        _context.Decide.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.Decide.FindAsync(id);
        if (data != null)
        {
            data.IsDelete = true;
            data.DeleteAt = DateTime.Now;
            _context.Decide.Update(data);
            await _context.SaveChangesAsync();
        }
    }
}