using ManageEmployee.Dal.DbContexts;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class RemoveDeleteWithAccGroup3Handler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    public RemoveDeleteWithAccGroup3Handler(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        if (account.Type == 5 && account.AccGroup == 3)
        {
            var warehouses = _context.Warehouses.Where(x => x.Code != account.WarehouseCode).ToList();
            foreach (var warehouse in warehouses)
            {
                var sameLevelDetail = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                    x.Code == account.Code && x.ParentRef == account.ParentRef &&
                    x.WarehouseCode == warehouse.Code);
                if (sameLevelDetail != null)
                {
                    var parent = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                        x.Code == sameLevelDetail.ParentRef &&
                        x.WarehouseCode == sameLevelDetail.WarehouseCode);
                    //Kiểm tra có còn chi tiết không
                    if (parent != null && _context.GetChartOfAccount(year).Any(x => x.Id != sameLevelDetail.Id &&
                        x.ParentRef == account.ParentRef &&
                        x.WarehouseCode == sameLevelDetail.WarehouseCode))
                    {
                        //Kiểm tra còn tài khoản cùng cấp không
                        if (_context.GetChartOfAccount(year).Any(x => x.ParentRef == parent.ParentRef))
                        {
                            parent.DisplayDelete = false;
                        }
                        else
                        {
                            parent.DisplayDelete = true;
                        }
                    }

                    _context.ChartOfAccounts.Remove(sameLevelDetail);
                }
            }
        }
        else if (account.Type == 6 && account.AccGroup == 3)
        {
            var warehouses = _context.Warehouses.Where(x => x.Code != account.WarehouseCode).ToList();
            foreach (var warehouse in warehouses)
            {
                var sameLevelDetail = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                    x.Code == account.Code && x.ParentRef == account.ParentRef &&
                    x.WarehouseCode == warehouse.Code);
                if (sameLevelDetail != null)
                {
                    var segments = sameLevelDetail.ParentRef.Split(':');
                    var parentRef = segments[1];
                    var grandParentRef = segments[0];
                    var parent = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                        x.Code == parentRef && x.ParentRef == grandParentRef &&
                        x.WarehouseCode == sameLevelDetail.WarehouseCode);
                    //Kiem tra co con chi tiet cung cap khong
                    if (parent != null && !_context.GetChartOfAccount(year).Any(x => x.Id != sameLevelDetail.Id &&
                        x.ParentRef == account.ParentRef &&
                        x.WarehouseCode == sameLevelDetail.WarehouseCode))
                    {
                        parent.DisplayDelete = true;
                    }

                    _context.ChartOfAccounts.Remove(sameLevelDetail);
                }
            }
        }

        _context.SaveChanges();
    }
}