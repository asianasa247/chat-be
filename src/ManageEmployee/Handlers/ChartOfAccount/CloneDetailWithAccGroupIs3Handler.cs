using AutoMapper;
using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class CloneDetailWithAccGroupIs3Handler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CloneDetailWithAccGroupIs3Handler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        if (account.Type == 5 && account.AccGroup == 3)
        {
            var warehouses = _context.Warehouses.Where(x => x.Code != account.WarehouseCode).ToList();
            foreach (var warehouse in warehouses)
            {
                if (!_context.GetChartOfAccount(year).Any(x =>
                    x.Code == account.Code && x.ParentRef == account.ParentRef && x.Type == account.Type &&
                    x.WarehouseCode == warehouse.Code))
                {
                    var cloneAccount = _mapper.Map<Entities.ChartOfAccountEntities.ChartOfAccount>(account);
                    cloneAccount.Id = 0;
                    cloneAccount.OpeningStockQuantity = 0;
                    cloneAccount.WarehouseCode = warehouse.Code;
                    cloneAccount.Year = year;

                    _context.ChartOfAccounts.Add(cloneAccount);
                    _context.SaveChanges();
                }
            }
        }
        else if (account.Type == 6 && account.AccGroup == 3)
        {
            var warehouses = _context.Warehouses.Where(x => x.Code != account.WarehouseCode).ToList();
            foreach (var warehouse in warehouses)
            {
                if (!_context.GetChartOfAccount(year).Any(x =>
                    x.Code == account.Code && x.ParentRef == account.ParentRef && x.Type == account.Type &&
                    x.WarehouseCode == warehouse.Code))
                {
                    var cloneAccount = _mapper.Map<Entities.ChartOfAccountEntities.ChartOfAccount>(account);
                    cloneAccount.Id = 0;
                    cloneAccount.OpeningStockQuantity = 0;
                    cloneAccount.WarehouseCode = warehouse.Code;
                    var segments = account.ParentRef.Split(':');
                    var parentRef = segments[1];
                    var grandParentRef = segments[0];
                    var parent = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                        x.Code == parentRef && x.ParentRef == grandParentRef &&
                        x.WarehouseCode == cloneAccount.WarehouseCode);
                    if (parent != null)
                    {
                        parent.DisplayInsert = true;
                        parent.DisplayDelete = false;
                    }
                    cloneAccount.Year = year;

                    _context.ChartOfAccounts.Add(cloneAccount);
                    _context.SaveChanges();
                }
            }
        }
    }
}