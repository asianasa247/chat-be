using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Relatives;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class RelationShipService : IRelationShipService
{
    private readonly ApplicationDbContext _context;

    public RelationShipService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool Create(RelationShip relative)
    {
        relative.CreatedAt = DateTime.Now;
        relative.IsDelete = false;
        _context.RelationShips.Add(relative);
        _context.SaveChanges();

        return true;
    }

    public bool Update(RelationShip relative)
    {
        var result = _context.RelationShips.AsNoTracking().FirstOrDefault(x => x.Id == relative.Id);
        if (result == null)
            throw new ErrorException(ResultErrorConstants.USER_EMPTY_OR_DELETE);
        var submitRelative = new RelationShip()
        {
            Id = relative.Id,
            EmployeeId = relative.EmployeeId,
            EmployeeName = relative.EmployeeName,
            PersonOppositeId = relative.PersonOppositeId,
            PersonOppositeName = relative.PersonOppositeName,
            UpdatedAt = DateTime.Now,
            UserUpdated = relative.UserUpdated,
            ClaimingYourself = relative.ClaimingYourself,
            ProclaimedOpposite = relative.ProclaimedOpposite,
            Type = relative.Type,
        };
        _context.RelationShips.Update(submitRelative);
        _context.SaveChanges();
        return true;
    }

    public void Delete(int id)
    {
        var result = _context.RelationShips.AsNoTracking().FirstOrDefault(x => !x.IsDelete && x.Id == id);
        if (result != null)
        {
            result.IsDelete = true;
            _context.RelationShips.Update(result);
            _context.SaveChanges();
        }
    }

    public IEnumerable<RelationShip> GetListPaging(int _pageSize, int page, int employeeId)
    {
        var query = _context.RelationShips
            .Where(x => !x.IsDelete);
        if (employeeId > 0)
            query = query.Where(x => x.EmployeeId == employeeId);
        return query
            .OrderByDescending(x => x.CreatedAt)
            .ThenBy(x => x.EmployeeName)
            .Skip(_pageSize * (page - 1))
            .Take(_pageSize);
    }

    public RelationShip GetById(int id)
    {
        return _context.RelationShips.Find(id);
    }

    public int Count(int employeeId)
    {
        if (employeeId > 0)
            return _context.RelationShips
            .Where(x => !x.IsDelete).Count(x => x.EmployeeId == employeeId);
        return _context.RelationShips
            .Where(x => !x.IsDelete).Count(x => !x.IsDelete);
    }
}