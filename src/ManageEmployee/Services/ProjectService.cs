using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProjectModels;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Projects;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class ProjectService: IProjectService
{
    private readonly ApplicationDbContext _context;

    public ProjectService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<ProjectGetListModel>> GetAll()
    {
        var datas = await _context.Projects.Where(x => x.IsDeleted != true).Select(x => new ProjectGetListModel
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Status = x.Status,
        }).ToListAsync();
        foreach (var item in datas)
        {
            var isExistLedger = await _context.Ledgers.AnyAsync(x => x.ProjectCode == item.Code);
            item.AllowDelete = !isExistLedger;
            item.AllowUpdateCode = !isExistLedger ;
        }
        return datas;
    }

    public async Task<PagingResult<ProjectPagingModel>> GetAll(int pageIndex, int pageSize, string keyword)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var result = new PagingResult<ProjectPagingModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
            };


            var branchs = _context.Projects.Where(x => !x.IsDeleted && x.Id != 0)
                            .Select(x => new ProjectPagingModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Code = x.Code,
                                Status = x.Status,
                            }).AsNoTracking();
            result.TotalItems = await branchs.CountAsync();
            result.Data = await branchs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            foreach(var item in result.Data) 
            {
                var isExistLedger = await _context.Ledgers.AnyAsync(x => x.ProjectCode == item.Code);
                item.AllowDelete = isExistLedger;
            }
            return result;
        }
        catch
        {
            return new PagingResult<ProjectPagingModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<ProjectPagingModel>()
            };
        }
    }

    public async Task<string> Create(ProjectModel request)
    {
        try
        {
            Project item = new()
            {
                Code = request.Code,
                Name = request.Name,
                Status = request.Status,
            };
            _context.Projects.Add(item);
            await _context.SaveChangesAsync();

            return string.Empty;
        }
        catch
        {
            throw;
        }
    }

    public async Task<ProjectDetaillModel> GetById(int id)
    {
        try
        {
            var item = await _context.Projects.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();
            if (item != null)
            {
                var isExistLedger =  await _context.Ledgers.AnyAsync(x=> x.ProjectCode == item.Code);
                return new ProjectDetaillModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Code = item.Code,
                    Status = item.Status,
                    AllowUpdateCode = isExistLedger ,
                };
            }
            else
            {
                return new ProjectDetaillModel();
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<string> Update(ProjectModel request)
    {
        var item = await _context.Projects.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);
        if (item == null)
        {
            return ErrorMessages.DataNotFound;
        }
        item.Name = request.Name;
        item.Code = request.Code;
        item.Status = request.Status;
        _context.Projects.Update(item);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<string> Delete(int id)
    {
        var item = await _context.Projects.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();

        if (item != null)
        {
            var ledger = await _context.Ledgers.FirstOrDefaultAsync(x => x.ProjectCode == item.Code);
            if (ledger != null)
                return "Tồn tại phát sinh";
           
            _context.Projects.Remove(item);
            await _context.SaveChangesAsync();
        }
        return string.Empty;
    }
}
