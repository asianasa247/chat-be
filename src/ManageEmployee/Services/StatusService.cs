using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Statuses;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class StatusService : IStatusService
{
    private readonly ApplicationDbContext _context;

    public StatusService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Status>> GetAll(StatusTypeEnum type)
    {
        return await _context.Status
            .Where(x => !x.IsDelete && x.Type == type)
            .OrderBy(x => x.Order).ToListAsync();
    }

    public async Task<PagingResult<Status>> GetAll(StatusPagingRequest param)
    {
        var query = _context.Status
            .Where(x => !x.IsDelete)
            .Where(x => param.Type == null || x.Type == param.Type)
            ;
        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(param.SearchText.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().StartsWith(param.SearchText.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().EndsWith(param.SearchText.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().StartsWith(param.SearchText.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().EndsWith(param.SearchText.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().Equals(param.SearchText.Trim().ToLower())
            );
        }

        return new PagingResult<Status>
        {
            Data = await query.OrderBy(x => x.Order).Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync(),
            PageSize = param.PageSize,
            CurrentPage = param.Page,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<Status> GetById(int id)
    {
        return await _context.Status.FindAsync(id);
    }

    public async Task<string> Create(Status param)
    {
        var isExistStatus = await _context.Status.AnyAsync(u => u.Name == param.Name);
        if (isExistStatus)
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Status.Add(param);
       await  _context.SaveChangesAsync();

        return string.Empty;
    }

    public async Task<string> Update(Status param)
    {
        var status = await _context.Status.FindAsync(param.Id);

        if (status == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        status.Name = param.Name;
        status.Description = param.Description;
        status.StatusDetect = param.StatusDetect;
        status.CompanyId = param.CompanyId;
        status.Color = param.Color;
        status.Order = param.Order;

        status.UpdatedAt = DateTime.Now;
        status.UserUpdated = param.UserUpdated;

        _context.Status.Update(status);
        await _context.SaveChangesAsync();

        return string.Empty;
    }

    public async Task<string> Delete(int id)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            int result =await  _context.CustomerContactHistories.Where(o => o.JobsId == id).CountAsync();

            if (result > 0)
            {
                throw new ErrorException(ResultErrorConstants.Is_Used);
            }
            else
            {
                var status = await _context.Status.FindAsync(id);
                if (status != null)
                {
                    _context.Status.Remove(status);
                }
            }
           await  _context.SaveChangesAsync();
            await _context.Database.CommitTransactionAsync();
            return string.Empty;
        }
        catch
        {
            await _context.Database.RollbackTransactionAsync();
            throw;
        }
    }
}