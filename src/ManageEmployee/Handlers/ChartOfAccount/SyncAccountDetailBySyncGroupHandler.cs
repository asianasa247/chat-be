using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;

namespace ManageEmployee.Handlers.ChartOfAccount;

public class SyncAccountDetailBySyncGroupHandler : IChartOfAccountHandler
{
    private readonly ApplicationDbContext _context;
    private readonly ChartOfAccountModel _model;
    private readonly IMapper _mapper;
    private readonly AccountSyncAction _action;

    public SyncAccountDetailBySyncGroupHandler(ApplicationDbContext context, ChartOfAccountModel model, IMapper mapper,
        AccountSyncAction action)
    {
        _context = context;
        _model = model;
        _mapper = mapper;
        _action = action;
    }

    public void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        Entities.ChartOfAccountEntities.ChartOfAccount parentAccount;
        var accountCode = account.Code;
        var accountParentRef = account.ParentRef;
        var accountWarehouseCode = account.WarehouseCode;

        account.Year = year;

        if (account.Type == 5)
        {
            parentAccount = _context.GetChartOfAccount(year).SingleOrDefault(x => x.Code == accountParentRef);
            if (parentAccount != null)
            {
                if (_action == AccountSyncAction.Create)
                    parentAccount.HasDetails = true;
                else if (_action == AccountSyncAction.Delete)
                {
                    parentAccount.HasDetails =
                        _context.GetChartOfAccount(year).Any(x =>
                            x.ParentRef == accountParentRef && x.Id != account.Id && x.Type == 5);
                }
            }
        }
        else
        {
            var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var grandParentRef = string.Empty;
            if (segments != null && segments.Length > 0)
            {
                grandParentRef = segments[0];
            }
            parentAccount = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                x.Code == grandParentRef);
        }
   
        var syncGroupIds = _context.GetChartOfAccountGroupLink(year).Where(x => parentAccount != null && x.CodeChartOfAccount == parentAccount.Code)
            .Select(x => x.CodeChartOfAccountGroup).ToList();
        var syncAccountCodes = _context.GetChartOfAccountGroupLink(year)
            .Where(x => syncGroupIds.Contains(x.CodeChartOfAccountGroup) && ((parentAccount != null && x.CodeChartOfAccount != parentAccount.Code) || (parentAccount == null && x.CodeChartOfAccount != null)))
            .Select(x => x.CodeChartOfAccount).ToList();
        var syncAccounts = _context.GetChartOfAccount(year).Where(x => syncAccountCodes.Contains(x.Code)).ToList();

        foreach (var syncAccount in syncAccounts)
        {
            syncAccount.DisplayDelete = false;
            syncAccount.DisplayInsert = true;

            if (_action == AccountSyncAction.Create)
            {
                var duplicatedDetail = _mapper.Map<Entities.ChartOfAccountEntities.ChartOfAccount>(account);
                Map(account, duplicatedDetail, true);
                if (account.Type == 5)
                {
                    duplicatedDetail.ParentRef = syncAccount.Code;
                    if (account.AccGroup == 3)
                    {
                        var warehouses = _context.Warehouses.Where(x => x.Code != duplicatedDetail.WarehouseCode)
                            .ToList();
                        foreach (var warehouse in warehouses)
                        {
                            if (!_context.GetChartOfAccount(year).Any(x =>
                                x.Code == duplicatedDetail.Code && x.Type == duplicatedDetail.Type &&
                                x.WarehouseCode == warehouse.Code && x.ParentRef == duplicatedDetail.ParentRef))
                            {
                                //var cloneAccount = _mapper.Map<Entities.ChartOfAccount>(duplicatedDetail);
                                var cloneAccount = new Entities.ChartOfAccountEntities.ChartOfAccount();
                                cloneAccount.Id = 0;
                                cloneAccount.ParentRef = duplicatedDetail.ParentRef;
                                cloneAccount.WarehouseCode = warehouse.Code;

                                Map(duplicatedDetail, cloneAccount, true);

                                _context.ChartOfAccounts.Add(cloneAccount);
                                _context.SaveChanges();
                            }
                        }
                    }


                }
                else
                {
                    var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    var parentRef = segments[1];
                    duplicatedDetail.ParentRef = $"{syncAccount.Code}:{parentRef}";

                    var acckhos = _context.GetChartOfAccount(year).Where(x => x.Code == parentRef && x.ParentRef == syncAccount.Code).ToList();
                    foreach(var acckho in acckhos)
                    {
                        acckho.HasChild = true;
                        if (acckho.WarehouseCode == duplicatedDetail.WarehouseCode)
                            continue;
                        //var duplicatedDetail_new = _mapper.Map<Entities.ChartOfAccount>(duplicatedDetail);
                        var duplicatedDetail_new = new Entities.ChartOfAccountEntities.ChartOfAccount();
                        duplicatedDetail_new.Id = 0;
                        duplicatedDetail_new.ParentRef = duplicatedDetail.ParentRef;
                        duplicatedDetail_new.WarehouseCode = acckho.WarehouseCode;
                        Map(duplicatedDetail_new, duplicatedDetail, true);

                        _context.ChartOfAccounts.Add(duplicatedDetail_new);
                    }
                }
                syncAccount.HasDetails = true;
                _context.ChartOfAccounts.Add(duplicatedDetail);
                _context.SaveChanges();
                //CalculateBalance(duplicatedDetail);
            }
            else if (_action == AccountSyncAction.Edit)
            {
                //_mapper.Map(_model, account);

                if (account.Type == 5)
                {
                    var detail =
                        _context.GetChartOfAccount(year).SingleOrDefault(
                            x => x.ParentRef == syncAccount.Code && x.Code == accountCode && x.Type == 5 &&
                                 x.WarehouseCode == accountWarehouseCode);
                    if (detail != null)
                    {
                        Map(account, detail);
                    }

                    _context.SaveChanges();
                    //CalculateBalance(detail);
                }
                else if (account.Type == 6)
                {
                    var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    var parentRef = segments[1];
                    var detail = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                        x.ParentRef == $"{syncAccount.Code}:{parentRef}" && x.Type == 6 && x.Code == accountCode &&
                        x.WarehouseCode == accountWarehouseCode);
                    if (detail != null)
                    {
                        Map(account, detail);
                    }

                    _context.SaveChanges();
                    //CalculateBalance(detail);
                }
            }
            else
            {
                if (account.Type == 5)
                {
                    var detail = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                        x.Code == accountCode && x.ParentRef == syncAccount.Code &&
                        x.WarehouseCode == accountWarehouseCode);
                    if (detail != null)
                    {
                        _context.ChartOfAccounts.Remove(detail);
                        _context.SaveChanges();
                        if (!_context.GetChartOfAccount(year).Any(x => x.ParentRef == syncAccount.Code))
                        {
                            syncAccount.DisplayDelete = true;
                            syncAccount.HasDetails = false;
                        }

                        CalculateBalance(detail, year);
                    }
                }
                else if (account.Type == 6)
                {
                    var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    var parentRef = segments[1];
                    var detail = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                        x.Code == accountCode && x.ParentRef == $"{syncAccount.Code}:{parentRef}" &&
                        x.WarehouseCode == accountWarehouseCode);
                    if (detail != null)
                    {
                        _context.ChartOfAccounts.Remove(detail);
                        _context.SaveChanges();
                        if (!_context.GetChartOfAccount(year).Any(x => x.ParentRef == $"{syncAccount.Code}:{parentRef}"))
                        {
                            syncAccount.DisplayDelete = true;
                        }

                        CalculateBalance(detail, year);
                    }
                }
            }
        }

        _context.SaveChanges();
    }

    private void CalculateBalance(Entities.ChartOfAccountEntities.ChartOfAccount account, int year)
    {
        Entities.ChartOfAccountEntities.ChartOfAccount parent;

        if (account.Type == 6)
        {
            var segments = account.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
            var parentRef = segments[1];
            var grandParentRef = segments[0];
            parent = _context.GetChartOfAccount(year).SingleOrDefault(x =>
                x.Code == parentRef && x.ParentRef == grandParentRef && x.WarehouseCode == account.WarehouseCode);
        }
        else
        {
            parent = _context.GetChartOfAccount(year).SingleOrDefault(x => x.Code == account.ParentRef);
        }

        if (parent != null)
        {
            var sumModel = GetTotal(account, year);
            parent.OpeningDebit = sumModel.OpeningDebit;
            parent.OpeningCredit = sumModel.OpeningCredit;
            parent.OpeningForeignDebit = sumModel.OpeningForeignDebit;
            parent.OpeningForeignCredit = sumModel.OpeningForeignCredit;
            _context.SaveChanges();
            if (!string.IsNullOrEmpty(parent.ParentRef))
                CalculateBalance(parent, year);
        }
    }

    private SumModel GetTotal(Entities.ChartOfAccountEntities.ChartOfAccount model, int year)
    {
        var result = new SumModel()
        {
            OpeningCredit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningCredit),
            OpeningDebit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningDebit),
            OpeningForeignCredit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningForeignCredit),
            OpeningForeignDebit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningForeignDebit),
           ClosingCredit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningCredit + x.ArisingCredit),
            ClosingDebit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningDebit + x.ArisingDebit),
            ClosingForeignCredit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningForeignCredit + x.ArisingForeignCredit),
            ClosingForeignDebit = _context.GetChartOfAccount(year)
                .Where(coa =>
                    coa.ParentRef == model.ParentRef && coa.Type == model.Type)
                .Sum(x => x.OpeningForeignDebit + x.ArisingForeignDebit)
        };

        return result;
    }

    private void Map(Entities.ChartOfAccountEntities.ChartOfAccount from, Entities.ChartOfAccountEntities.ChartOfAccount to, bool isCreate = false)
    {
        if (isCreate)
        {
            to.OpeningDebit = 0;
            to.OpeningCredit = 0;
            to.OpeningForeignCredit = 0;
            to.OpeningForeignDebit = 0;
            to.StockUnit = from.StockUnit;
            to.StockUnitPrice = 0;
            to.OpeningStockQuantity = 0;
        }
        //to.OpeningDebit = from.OpeningDebit;
        //to.OpeningCredit = from.OpeningCredit;
        //to.OpeningForeignCredit = from.OpeningForeignCredit;
        //to.OpeningForeignDebit = from.OpeningForeignDebit;
        //to.ClosingDebit = from.ClosingDebit;
        //to.ClosingCredit = from.ClosingCredit;
        //to.ClosingForeignCredit = from.ClosingForeignCredit;
        //to.ClosingForeignDebit = from.ClosingForeignDebit;
        //to.MinimumStockQuantity = from.MinimumStockQuantity;
        //to.MaximumStockQuantity = from.MaximumStockQuantity;
        //to.StockCostPrice = from.StockCostPrice;
        //to.StockSellingPrice = from.StockSellingPrice;
        //to.StockUnitPrice = from.StockUnitPrice;
        //to.OpeningStockQuantity = from.OpeningStockQuantity;

        to.StockUnit = from.StockUnit;
        to.Name = from.Name;
        to.Code = from.Code;
    }
    class SumModel
    {
        public double? OpeningDebit { get; set; }

        public double? OpeningCredit { get; set; }

        public double? OpeningForeignDebit { get; set; }

        public double? OpeningForeignCredit { get; set; }

        public double? ClosingDebit { get; set; }

        public double? ClosingCredit { get; set; }

        public double? ClosingForeignDebit { get; set; }

        public double? ClosingForeignCredit { get; set; }
    }
}