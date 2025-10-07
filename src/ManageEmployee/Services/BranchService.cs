using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class BranchService : IBranchService
{
    private readonly ApplicationDbContext _context;

    public BranchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BranchModel>> GetAll()
    {
        return await _context.Branchs.Where(x => !x.IsDelete).Select(x => new BranchModel
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            ManagerName = x.ManagerName,
            TelephoneNumber = x.TelephoneNumber,
            Address = x.Address,
            Image = x.Image
        }).ToListAsync();
    }

    public async Task<PagingResult<BranchModel>> GetAll(int pageIndex, int pageSize, string keyword, int? type = null)
    {
        try
        {
            if (pageSize <= 0)
                pageSize = 20;

            if (pageIndex < 0)
                pageIndex = 1;

            var result = new PagingResult<BranchModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
            };

            var branchs = _context.Branchs.Where(x => !x.IsDelete && x.Id != 0)
                            .Where( x => string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword) || x.Code.Contains(keyword))
                            .Select(x => new BranchModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Code = x.Code,
                                ManagerName = x.ManagerName,
                            }).AsNoTracking();
            result.TotalItems = await branchs.CountAsync();
            result.Data = await branchs.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return result;
        }
        catch
        {
            return new PagingResult<BranchModel>()
            {
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = 0,
                Data = new List<BranchModel>()
            };
        }
    }

    public async Task<string> Create(Branch request)
    {
        try
        {
            var exist = await _context.Branchs.AnyAsync(
                x => x.Name.ToLower() == request.Name.ToLower() && !x.IsDelete);
            if (exist)
            {
                return ErrorMessages.BranchNameAlreadyExist;
            }
            var existCode = await _context.Branchs.AnyAsync(
                x => x.Code.ToLower() == request.Code.ToLower() && !x.IsDelete);
            if (existCode)
            {
                return ErrorMessages.BranchCodeAlreadyExist;
            }

            await _context.Branchs.AddAsync(request);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        catch
        {
            throw;
        }
    }

    public async Task<BranchModel> GetById(int id)
    {
        try
        {
            var branch = await _context.Branchs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
            if (branch != null)
            {
                return new BranchModel
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    Code = branch.Code,
                    ManagerName = branch.ManagerName,
                };
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<string> Update(Branch request)
    {
        try
        {
            var branch = await _context.Branchs.Where(x => x.Id == request.Id && !x.IsDelete).FirstOrDefaultAsync();
            if (branch == null)
            {
                return ErrorMessages.DataNotFound;
            }

            var checkBranchName = await _context.Branchs.Where(x => x.Name.ToLower() == request.Name.ToLower() && !x.IsDelete && x.Id != request.Id).FirstOrDefaultAsync();
            if (checkBranchName != null && checkBranchName.Id != branch.Id)
            {
                return ErrorMessages.NameAlreadyExist;
            }
            var checkBranchCode = await _context.Branchs.Where(x => x.Code.ToLower() == request.Code.ToLower() && !x.IsDelete && x.Id != request.Id).FirstOrDefaultAsync();
            if (checkBranchCode != null && checkBranchCode.Id != branch.Id)
            {
                return ErrorMessages.BranchCodeAlreadyExist;
            }
            branch.Name = request.Name;
            branch.Code = request.Code;
            branch.ManagerName = request.ManagerName;
            _context.Branchs.Update(branch);
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        catch
        {
            throw;
        }
    }

    public async Task<string> Delete(int id)
    {
        var branch = await _context.Branchs.FirstOrDefaultAsync(x => x.Id == id && !x.IsDelete);
        var isExistUser = await _context.Users.AnyAsync(x => x.BranchId == branch.Id && !x.Status);
        if (isExistUser)
            return ErrorMessages.DataExist;

        if (branch != null)
        {
            branch.IsDelete = true;
            _context.Branchs.Update(branch);
           await  _context.SaveChangesAsync();
        }
        return string.Empty;
    }
}