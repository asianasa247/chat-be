using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.UserServices;
public class UserContractHistoryService : IUserContractHistoryService
{
    private readonly ApplicationDbContext _context;

    public UserContractHistoryService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PagingResult<UserContractHistoryModel>> GetPagingAsync(PagingRequestModel param)
    {
        if (param.PageSize <= 0)
            param.PageSize = 20;

        if (param.Page < 0)
            param.Page = 1;

        var listData = from c in _context.UserContractHistories
                       join u in _context.Users on c.UserId equals u.Id
                       join t in _context.ContractTypes on c.ContractTypeId equals t.Id
                       orderby c.CreatedBy descending
                       select new UserContractHistoryModel
                       {
                           Id = c.Id,
                           ContractTypeName = t.Name,
                           UserName = u.FullName,
                           CreatedAt = c.CreatedAt

                       };


        return new PagingResult<UserContractHistoryModel>()
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await listData.CountAsync(),
            Data = await listData.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync()
        };
    }
    public async Task<UserContractHistory> GetDetail(int contractHistoryId)
    {
        return await _context.UserContractHistories.FindAsync(contractHistoryId);
    }
}
