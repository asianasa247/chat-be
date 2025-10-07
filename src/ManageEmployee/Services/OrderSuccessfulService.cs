using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Orders;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class OrderSuccessfulServicce : IOrderSuccessfulService
{
    private ApplicationDbContext _context;

    public OrderSuccessfulServicce(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagingResult<OrderSuccessful>> GetAll(int currentPage, int pageSize,string keyword)
    {
        if (pageSize <= 0)
            pageSize = 20;

        if (currentPage < 0)
            currentPage = 1;


        var result = new PagingResult<OrderSuccessful>()
        {
            CurrentPage = currentPage,
            PageSize = pageSize,
        };

        var query = _context.OrderSuccessfuls ;

        result.TotalItems = await query.CountAsync();
        result.Data = await query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
        return result;
    }

    public IEnumerable <OrderSuccessful> GetAll()
    {
        var query = _context.OrderSuccessfuls.OrderBy(x => x.Id);
        return query.ToList();
    }

    public OrderSuccessful GetByID(int id)
    {
        return _context.OrderSuccessfuls.Find(id);
    }

    public OrderSuccessful Create(OrderSuccessful param)
    {
        if (string.IsNullOrWhiteSpace(param.Content))
            throw new ErrorException(ResultErrorConstants.MODEL_MISS);
        _context.OrderSuccessfuls.Add(param);
        _context.SaveChanges();

        return param;
    }

    public void Update(OrderSuccessful param)
    {
        var ordersuccessful = _context.OrderSuccessfuls.Find(param.Id);

        if (ordersuccessful == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        
           
       ordersuccessful.Content =param.Content;
        

        _context.OrderSuccessfuls.Update(ordersuccessful);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var ordersuccessfull = _context.OrderSuccessfuls.Find(id);
        if (ordersuccessfull != null)
        {
            _context.OrderSuccessfuls.Remove(ordersuccessfull);
            _context.SaveChanges();
        }
    }
}