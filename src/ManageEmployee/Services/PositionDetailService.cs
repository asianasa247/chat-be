using Common.Extensions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Positions;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class PositionDetailService : IPositionDetailService
{
    private readonly ApplicationDbContext _context;

    public PositionDetailService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PositionDetail>> GetAll()
    {
        return await _context.PositionDetails.Where(x => !x.isDelete)
                .OrderBy(x => x.Order).ToListAsync();
    }

    public async Task<PagingResult<PositionDetailModel>> GetAll(int currentPage, int pageSize, string keyword)
    {
        if (pageSize <= 0)
            pageSize = 20;

        if (currentPage < 0)
            currentPage = 1;

        var result = new PagingResult<PositionDetailModel>()
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
        };

        var query = from pd in _context.PositionDetails
                    join p in _context.Positions on pd.PositionId equals p.Id into pos
                    from p in pos.DefaultIfEmpty()
                    where !pd.isDelete && !p.isDelete
                    select new PositionDetailModel()
                    {
                        Id = pd.Id,
                        Name = pd.Name,
                        PositionId = pd.PositionId,
                        PositionName = p.Name,
                        Order = pd.Order
                    };
        if (!keyword.IsNullOrEmpty())
        {
            query = query.Where(x => string.IsNullOrEmpty(keyword) || x.Name.Contains(keyword));
        }
        result.TotalItems = await query.CountAsync();
        result.Data = await query.OrderBy(x => x.Order).Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        return result;
    }

    public async Task Create(PositionDetail param)
    {
        if (await _context.PositionDetails.Where(p => p.Name == param.Name).AnyAsync())
        {
            throw new ErrorException(ResultErrorConstants.NAME_EXIST);
        }
        if (string.IsNullOrWhiteSpace(param.Name))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);

        await _context.PositionDetails.AddAsync(param);
        await _context.SaveChangesAsync();
    }

    public async Task Update(PositionDetail param)
    {
        var position = await _context.PositionDetails.FindAsync(param.Id);

        if (position == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        if ( await _context.PositionDetails.AnyAsync(p => p.Id != param.Id && p.Name == param.Name))
        {
            throw new ErrorException(ResultErrorConstants.NAME_EXIST);
        }
        if (param.PositionId != position.PositionId)
        {
            var checkMemberHaveWarehoue = await _context.Users.AnyAsync(x => !x.IsDelete && x.PositionDetailId == param.Id);
            if (checkMemberHaveWarehoue)
                throw new ErrorException(ResultErrorConstants.WAREHOUSE_USER_CONTAINT);
        }
        position.PositionId = param.PositionId;
        position.Name = param.Name;
        position.Order = param.Order;
        position.IsManager = param.IsManager;

        _context.PositionDetails.Update(position);
         await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var position = await _context.PositionDetails.FindAsync(id);
        if (position != null)
        {
            var checkMemberHaveWarehoue = await _context.Users.AnyAsync(x => !x.IsDelete && x.PositionDetailId == id);
            if (checkMemberHaveWarehoue)
                throw new ErrorException(ResultErrorConstants.WAREHOUSE_USER_CONTAINT);

            position.isDelete = true;
            _context.PositionDetails.Update(position);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PositionDetail> GetById(int id)
    {
        return await _context.PositionDetails.FirstOrDefaultAsync(x => x.Id == id);
    }
}