using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Positions;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class PositionService : IPositionService
{
    private ApplicationDbContext _context;

    public PositionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<Position>> GetAll(int currentPage, int pageSize, string keyword)
    {
        if (pageSize <= 0)
            pageSize = 20;

        if (currentPage < 0)
            currentPage = 1;


        var result = new PagingResult<Position>()
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
        };

        var query = _context.Positions.Where(x => !x.isDelete)
            .Where(x => string.IsNullOrEmpty(keyword) || x.Code.Contains(keyword)
            || x.Name.Contains(keyword));

        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Order).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        return result;
    }

    public IEnumerable<Position> GetAll()
    {
        var query = _context.Positions.Where(x => !x.isDelete).OrderBy(x => x.Order);
        return query
                .ToList();
    }

    public Position GetById(int id)
    {
        return _context.Positions.Find(id);
    }

    public Position Create(Position param)
    {
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);
        if (_context.Positions.Where(p => p.Code == param.Code).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }
        _context.Positions.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(Position param)
    {
        var position = _context.Positions.Find(param.Id);

        if (position == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        if (_context.Positions.Where(p => p.Id != param.Id && p.Code == param.Code).Any())
        {
            throw new ErrorException(ResultErrorConstants.CODE_EXIST);
        }
        var checkMemberHaveWarehoue = _context.PositionDetails.Where(x => !x.isDelete && x.PositionId == param.Id).ToList();
        if (!checkMemberHaveWarehoue.Any())
            position.Code = param.Code;

        position.Name = param.Name;
        position.Code = param.Code;
        position.Order = param.Order;

        _context.Positions.Update(position);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var position = _context.Positions.Find(id);
        if (position != null)
        {
            var checkMemberHaveWarehoue = _context.PositionDetails.Where(x => !x.isDelete && x.PositionId == id).ToList();
            if (checkMemberHaveWarehoue.Any())
                throw new ErrorException(ResultErrorConstants.WAREHOUSE_USER_CONTAINT);

            position.isDelete = true;
            _context.Positions.Update(position);
            _context.SaveChanges();
        }
    }
}