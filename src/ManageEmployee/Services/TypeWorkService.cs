using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class TypeWorkService : ITypeWorkService
{
    private readonly ApplicationDbContext _context;

    public TypeWorkService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<TypeWorkModel>> GetAll(int currentPage, int pageSize)
    {
        var query = from type in _context.TypeWorks
                    join _br in _context.Branchs on type.BranchId equals _br.Id into _br
                    from br in _br.DefaultIfEmpty()
                    join _de in _context.Departments on type.DepartmentId equals _de.Id into _de
                    from de in _de.DefaultIfEmpty()

                    select new TypeWorkModel()
                    {
                        Id = type.Id,
                        Name = type.Name,
                        DepartmentId = type.DepartmentId,
                        DepartmentName = de.Name,
                        BranchId = type.BranchId,
                        BranchName = br.Name,
                        Code = type.Code,
                        Color = type.Color,
                        Point = type.Point,
                    };
        var result = new PagingResult<TypeWorkModel>()
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
        };

        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Id).Skip((currentPage) * pageSize).Take(pageSize).ToListAsync();
        return result;
    }

    public IEnumerable<TypeWork> GetAll()
    {
        return _context.TypeWorks;
    }

    public TypeWork GetById(int id)
    {
        return _context.TypeWorks.Find(id);
    }

    public async Task Create(TypeWorkModel param)
    {
        var TypeWork = new TypeWork
        {
            Code = param.Code,
            Name = param.Name,
            Point = param.Point ?? 0,
            DepartmentId = param.DepartmentId ?? 0,
            BranchId = param.BranchId ?? 0,
            Color = param.Color
        };

        await _context.TypeWorks.AddAsync(TypeWork);
       await  _context.SaveChangesAsync();
    }

    public async Task Update(TypeWorkModel param)
    {
        var TypeWork = await _context.TypeWorks.FindAsync(param.Id);

        if (TypeWork == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        TypeWork.Code = param.Code;
        TypeWork.Name = param.Name;
        TypeWork.Point = param.Point ?? 0;
        TypeWork.DepartmentId = param.DepartmentId ?? 0;
        TypeWork.BranchId = param.BranchId ?? 0;
        TypeWork.Color = param.Color;

        _context.TypeWorks.Update(TypeWork);
        await _context.SaveChangesAsync();
    }

    public void Delete(int id)
    {
        var TypeWork = _context.TypeWorks.Find(id);
        if (TypeWork != null)
        {
            _context.TypeWorks.Remove(TypeWork);
            _context.SaveChanges();
        }
    }
}