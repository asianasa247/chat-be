using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Jobs;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.DataTransferObject.StatusModels;
using ManageEmployee.DataTransferObject.CompanyModels;

namespace ManageEmployee.Services;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;

    public JobService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Job> GetAll()
    {
        return _context.Jobs
            .Where(x => !x.IsDelete)
            .OrderBy(x => x.Name);
    }

    public async Task<object> GetJobsAndStatusesExistingCustomerHistoriesAsync(int customerId)
    {
        var query = await (
                    from ch in _context.CustomerContactHistories
                    join jb in _context.Jobs on ch.JobsId equals jb.Id
                    join st in _context.Status on ch.StatusId equals st.Id
                    where customerId == 0 || ch.CustomerId == customerId
                    select new
                    {
                        ch.JobsId,
                        ch.StatusId,
                        JobName = jb.Name,
                        JobColor = jb.Color,
                        StatusName = st.Name,
                        StatusColor = st.Color,
                    }).ToListAsync();

        var jobs = query
                    .GroupBy(g => new { g.JobsId, g.JobName, g.JobColor })
                    .Select(s => new JobDto()
                    {
                        Id = s.Key.JobsId.Value,
                        Name = s.Key.JobName,
                        Color = s.Key.JobColor,
                        StatusIds = s.Select(i => i.StatusId.Value).ToList(),
                        Count = s.Select(i => i.StatusId.Value).Count(),
                    }).ToList();

        var statuses = query
                    .GroupBy(g => new { g.StatusId, g.StatusName, g.StatusColor })
                    .Select(s => new StatusDto()
                    {
                        Id = s.Key.StatusId.Value,
                        Name = s.Key.StatusName,
                        Color = s.Key.StatusColor,
                        JobIds = s.Select(i => i.JobsId.Value).ToList(),
                        Count = s.Select(i => i.JobsId.Value).Count(),
                    }).ToList();

        return new
        {
            Jobs = jobs,
            Statuses = statuses
        };
    }

    public IEnumerable<Job> GetPaging(int currentPage, int pageSize, string keyword)
    {
        var query = _context.Jobs
            .Where(x => !x.IsDelete);
        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
            );
        }

        return query
            .Skip(pageSize * currentPage)
            .Take(pageSize);
    }

    public Job GetById(int id)
    {
        return _context.Jobs.Find(id);
    }

    public async Task<string> Create(Job param)
    {
        if (_context.Jobs.Where(u => u.Name == param.Name).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }

        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Jobs.Add(param);
        await _context.SaveChangesAsync();

        return string.Empty;
    }

    public async Task<string> Update(Job param)
    {
        var job = await _context.Jobs.FindAsync(param.Id);

        if (job == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        job.Name = param.Name;
        job.Description = param.Description;
        job.Status = param.Status;
        job.CompanyId = param.CompanyId;
        job.Color = param.Color;

        job.UpdatedAt = DateTime.Now;
        job.UserUpdated = param.UserUpdated;

        _context.Jobs.Update(job);
        await _context.SaveChangesAsync();

        return string.Empty;
    }

    public int Count(string keyword)
    {
        var query = _context.Jobs.Where(x => !x.IsDelete);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.Trim().ToLower().Equals(keyword.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                     x.Name.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().StartsWith(keyword.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().EndsWith(keyword.Trim().ToLower()) ||
                                     x.Description.Trim().ToLower().Equals(keyword.Trim().ToLower())
            );
        }
        return query.Count();
    }

    public string Delete(int id)
    {
        try
        {
            _context.Database.BeginTransactionAsync();

            int result = _context.CustomerContactHistories.Where(o => o.JobsId == id).Count();

            if (result > 0)
            {
                throw new ErrorException(ResultErrorConstants.Is_Used);
            }
            else
            {
                var job = _context.Jobs.Find(id);
                if (job != null)
                {
                    _context.Jobs.Remove(job);
                }
            }
            _context.SaveChanges();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }
}