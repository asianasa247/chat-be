using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Targets;

namespace ManageEmployee.Services;

public class TargetService : ITargetService
{
    private ApplicationDbContext _context;

    public TargetService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Target> GetAll(int currentPage, int pageSize, string keyword = "")
    {
        var query = _context.Targets.OrderBy(x => x.Order).ToList();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(x => x.Name.ToLower().Contains(keyword.ToLower())).ToList();
        }

        var results = query
            .Skip(pageSize * (currentPage - 1))
            .Take(pageSize).ToList();

        //var targetCheckInInfos = _timeKeepService
        //    .GetAllCheckInCount(DateTime.Now)
        //    .ToDictionary(x => x.TargetId);

        //foreach(var item in results)
        //{
        //    if (targetCheckInInfos.ContainsKey(item.Id))
        //    {
        //        item.CheckedInCount = targetCheckInInfos[item.Id].Count;
        //    }
        //}

        return results;
    }

    public IEnumerable<Target> GetAll()
    {
        var query = _context.Targets.OrderBy(x => x.Order);
        return query
                .ToList();
    }

    public Target GetById(int id)
    {
        return _context.Targets.Find(id);
    }

    public Target Create(Target param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        _context.Targets.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(Target request)
    {
        var target = _context.Targets.Find(request.Id);

        if (target == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        
        // Update all properties of the entity
        _context.Entry(target).CurrentValues.SetValues(request);
        
        var checkMemberHaveWarehoue = _context.Users.Where(x => !x.IsDelete && x.TargetId == request.Id).ToList();
        
        if (!checkMemberHaveWarehoue.Any())
            target.Code = request.Code;
        _context.Targets.Update(target);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var position = _context.Targets.Find(id);
        if (position != null)
        {
            var checkMemberHaveWarehoue = _context.Users.Where(x => !x.IsDelete && x.TargetId == id).ToList();
            if (checkMemberHaveWarehoue.Any())
                throw new ErrorException(ResultErrorConstants.WAREHOUSE_USER_CONTAINT);

            _context.Targets.Remove(position);
            _context.SaveChanges();
        }
    }
}