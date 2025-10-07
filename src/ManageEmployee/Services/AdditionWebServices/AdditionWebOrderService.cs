using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Services.Interfaces.AdditionWebServices;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.AdditionWebServices;

public class AdditionWebOrderService : AdditionWebServiceBase, IAdditionWebOrderService
{
    private readonly ApplicationDbContext _dbContext;

    public AdditionWebOrderService(IDbContextFactory dbContextFactory, ApplicationDbContext dbContext) 
        : base(dbContextFactory)
    {
        _dbContext = dbContext;
    }

    public async Task<PagingResult<OrderViewModelResponse>> SearchOrder(int webId, OrderSearchModel search)
    {
        var addtionWeb = await _dbContext.AdditionWebs.FirstOrDefaultAsync(x => x.Id == webId)
           ?? throw new Exception("Không tìm thấy website");

        if (string.IsNullOrEmpty(addtionWeb.DbName))
        {
            throw new Exception("Website chưa cấu hình database");
        }

        using var currentDbContext = GetApplicationDbContext(addtionWeb);

        var orderService = new OrderService(
            currentDbContext,
            null,
            null
        );

        return await orderService.SearchOrder(search);
    }
}
