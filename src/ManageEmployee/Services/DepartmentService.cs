using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Departments;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class DepartmentService : IDepartmentService
{
    private readonly ApplicationDbContext _context;

    public DepartmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<Department>> GetAll(DepartmentRequest request)
    {
        if (request.PageSize <= 0)
            request.PageSize = 20;

        if (request.Page < 0)
            request.Page = 1;

        var result = new PagingResult<Department>()
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
        };
        var query = _context.Departments.Where(x => x.isDelete == false)
            .Where(x => string.IsNullOrEmpty(request.SearchText) || x.Code.Contains(request.SearchText)
            || x.Name.Contains(request.SearchText))
            .Where(x => x.BranchId > 0 ? x.BranchId == request.BranchId : true);
        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Id).Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return result;
    }

    public IEnumerable<Department> GetAll()
    {
        return _context.Departments.Where(x => x.isDelete == false)
                .OrderBy(x => x.Name);
    }

    public Department GetById(int id)
    {
        return _context.Departments.Where(x => x.isDelete == false && x.Id == id).FirstOrDefault();
    }

    public bool checkMemberHaveWarehouseCode(int? id, string code)
    {
        var exist = _context.Departments.Where(x => x.isDelete == false && x.Code == code).ToList();
        if (id != null)
        {
            exist = exist.Where(x => x.Id != id).ToList();
        }
        return exist.Count > 0;
    }

    public bool checkMemberHaveWarehouse(int id)
    {
        var exist = _context.Users.Where(x => !x.IsDelete && x.DepartmentId == id).FirstOrDefault();
        return exist != null;
    }

    public Department Create(Department param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Departments.Add(param);
        _context.SaveChanges();

        return param;
    }

    public Department Update(int id, Department param)
    {
        var department = _context.Departments.Where(x => x.isDelete == false && x.Id == id).FirstOrDefault();
        if (department == null)
        {
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);
        }
        department.Code = param.Code;
        department.Name = param.Name;
        department.BranchId = param.BranchId;
        _context.Departments.Update(department);
        _context.SaveChanges();

        return department;
    }

    public void Delete(int id)
    {
        var department = _context.Departments.Where(x => !x.isDelete && x.Id == id).FirstOrDefault();
        if (department != null)
        {
            department.isDelete = true;
            _context.Departments.Update(department);
            _context.SaveChanges();
        }
    }

    public async Task<IEnumerable<Department>> GetListDepartmentForTask(int userId)
    {
        var taskRoleUserQueryable = _context.UserTaskRoleDetails
               .Where(x => x.UserId == userId)
               .Select(x => x.UserTaskId);
        var tasks = await _context.UserTasks.Where(x => x.IsDeleted == false && (x.UserCreated == userId || x.ViewAll == true || taskRoleUserQueryable.Contains(x.Id))).ToListAsync();
        var departmentIds = tasks.Select(x => x.DepartmentId).Distinct();
        return _context.Departments.Where(x => !x.isDelete && departmentIds.Contains(x.Id))
                .OrderBy(x => x.Name);
    }
}