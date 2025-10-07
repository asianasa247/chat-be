using AutoMapper;
using Common.Constants;
using ManageEmployee.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Entities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.BillEntities;

namespace ManageEmployee.Services;
public class GoodWarehousesService : IGoodWarehousesService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
    private readonly IChartOfAccountService _chartOfAccountService;
    public GoodWarehousesService(ApplicationDbContext context, IMapper mapper, IHubContext<BroadcastHub, IHubClient> hubContext, IChartOfAccountService chartOfAccountService)
    {
        _context = context;
        _mapper = mapper;
        _hubContext = hubContext;
        _chartOfAccountService = chartOfAccountService;
    }

    public async Task<PagingResult<GoodWarehousesViewModel>> GetAll(SearchViewModel param)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;
            var results = (from p in _context.GoodWarehouses
                           where p.Quantity > 0
                           && (string.IsNullOrEmpty(param.GoodType) || p.GoodsType == param.GoodType)
                           && (string.IsNullOrEmpty(param.Account) || p.Account == param.Account)
                           && (string.IsNullOrEmpty(param.Detail1) || p.Detail1 == param.Detail1)
                           && (string.IsNullOrEmpty(param.PriceCode) || p.PriceList == param.PriceCode)
                           && (string.IsNullOrEmpty(param.MenuType) || p.MenuType == param.MenuType)
                           && ((string.IsNullOrEmpty(param.SearchText) || (!string.IsNullOrEmpty(p.Detail2) ? p.DetailName2 : (p.DetailName1 ?? p.AccountName)).Contains(param.SearchText))
                           || p.Detail2.Contains(param.SearchText))
                           && p.Status == param.Status
                           orderby p.IsPrinted
                           select new GoodWarehousesViewModel()
                           {
                               Id = p.Id,
                               MenuType = p.MenuType,
                               Account = p.Account,
                               AccountName = p.AccountName,
                               Warehouse = p.Warehouse,
                               WarehouseName = p.WarehouseName,
                               Detail1 = p.Detail1,
                               Detail2 = p.Detail2,
                               DetailName1 = p.DetailName1,
                               DetailName2 = p.DetailName2,
                               GoodsType = p.GoodsType,
                               Image1 = p.Image1,
                               Quantity = p.Quantity,
                               Status = p.Status,
                               PriceList = p.PriceList,
                               Order = p.Order,
                               OrginalVoucherNumber = p.OrginalVoucherNumber,
                               LedgerId=p.LedgerId,
                               QrCode = (!String.IsNullOrEmpty(p.Detail2) ? p.Detail2 : (p.Detail1 ?? p.Account)) + " " + p.Order + "-" + p.Id,
                               GoodCode = !String.IsNullOrEmpty(p.Detail2) ? p.Detail2 : (p.Detail1 ?? p.Account),
                               GoodName = !String.IsNullOrEmpty(p.DetailName2) ? p.DetailName2 : (p.DetailName1 ?? p.AccountName),
                               Note = p.Note,
                               DateExpiration = p.DateExpiration,
                               DateManufacture = p.DateManufacture,
                               IsPrinted = p.IsPrinted,
                               QuantityInput = p.QuantityInput
                           });
            if (!string.IsNullOrEmpty(param.Warehouse))
            {
                results = results.Where(x => x.Warehouse == param.Warehouse);
            }

            if (!string.IsNullOrEmpty(param.GoodCode))
            {
                results = results.Where(x => x.Detail1 == param.GoodCode || x.Detail2 == param.GoodCode);
            }
            List<GoodWarehousesViewModel> datas;
            if (param.Page == 0)
                datas = await results.ToListAsync();
            else
            {
                datas = await results.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
                var goodWarehouseIds = datas.Select(x => x.Id).ToList();
                var goodWarehousePositions = await _context.GoodWarehousesPositions.Where(x => goodWarehouseIds.Contains(x.GoodWarehousesId)).ToListAsync();
                var warehouses = await _context.Warehouses.Where(x => !x.IsDelete).ToListAsync();
                var shevels = await _context.WareHouseShelves.ToListAsync();
                var floors = await _context.WareHouseFloors.ToListAsync();
                var positions = await _context.WareHousePositions.ToListAsync();

                foreach (var data in datas)
                {
                    var goods = await _context.Goods.FirstOrDefaultAsync(x => (x.Detail1 == data.Detail1 || string.IsNullOrEmpty(data.Detail1))
                                                && (x.Detail2 == data.Detail2 || string.IsNullOrEmpty(data.Detail1))
                                                && x.Account == data.Account
                                                && x.PriceList == "BGC"
                                                && (x.Warehouse == data.Warehouse || string.IsNullOrEmpty(data.Warehouse)));
                    data.SalePrice = goods?.SalePrice ?? 0;
                    var goodWarehouseDetails = goodWarehousePositions.Where(x => x.GoodWarehousesId == data.Id);
                    data.Positions = new List<string>();
                    foreach (var goodWarehouseDetail in goodWarehouseDetails)
                    {
                        var warehouse = warehouses.Find(X => X.Code == goodWarehouseDetail.Warehouse);
                        var shevel = shevels.Find(X => X.Id == goodWarehouseDetail.WareHouseShelvesId);
                        var floor = floors.Find(X => X.Id == goodWarehouseDetail.WareHouseFloorId);
                        var position = positions.Find(X => X.Id == goodWarehouseDetail.WareHousePositionId);
                        data.Positions.Add("Số lượng " + goodWarehouseDetail.Quantity.ToString() + " " + warehouse?.Name + ", " + shevel?.Name + ", " + floor?.Name + ", " + position?.Name);
                    }

                }
            }


            return new PagingResult<GoodWarehousesViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = results.Count(),
                Data = datas
            };

        }
        catch
        {
            return new PagingResult<GoodWarehousesViewModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<GoodWarehousesViewModel>()
            };
        }
    }

    public async Task<string> SyncChartOfAccount(int year)
    {
        var listAccount = _context.GetChartOfAccount(year)
               .Where(x => (x.Classification == 2 || x.Classification == 3)).ToList();
        var goodWarehouses = new List<GoodWarehouses>();
        var goodWarehouseUpdates = new List<GoodWarehouses>();
        var goodWarehouseChecks = await _context.GoodWarehouses.Where(x => x.Order == 0).ToListAsync();
        if (listAccount.Count > 0)
        {
            foreach (var account in listAccount)
            {
                if (string.IsNullOrEmpty(account.ParentRef) || account.HasDetails
                        || account.OpeningStockQuantityNB == null || account.OpeningStockQuantityNB == 0)
                    continue;
                GoodWarehouses goodWarehouse;

                if (account.Type == 6)
                {
                    var parentCodes = account.ParentRef.Split(":");

                    goodWarehouse = goodWarehouseChecks.Find(x => x.Detail2 == account.Code && x.Detail1 == parentCodes[1] && x.Account == parentCodes[0]);
                    if (goodWarehouse is null)
                        goodWarehouse = new();

                    goodWarehouse.Account = parentCodes[0];
                    goodWarehouse.Detail1 = parentCodes[1];
                    goodWarehouse.Detail2 = account.Code;
                    goodWarehouse.DetailName2 = account.Name;

                }
                else
                {
                    goodWarehouse = goodWarehouseChecks.Find(x => x.Detail1 == account.Code && x.Account == account.ParentRef);
                    if (goodWarehouse is null)
                        goodWarehouse = new();

                    goodWarehouse.Account = account.ParentRef;
                    goodWarehouse.Detail1 = account.Code;
                    goodWarehouse.DetailName1 = account.Name;
                }
                goodWarehouse.Warehouse = account.WarehouseCode;
                goodWarehouse.WarehouseName = account.WarehouseName;
                goodWarehouse.Order = 0;
                goodWarehouse.Status = 1;

                goodWarehouse.Quantity = account.OpeningStockQuantityNB ?? 0;
                goodWarehouse.QuantityInput = account.OpeningStockQuantityNB ?? 0;
                if (goodWarehouse.Id == 0)
                    goodWarehouses.Add(goodWarehouse);
                else
                    goodWarehouseUpdates.Add(goodWarehouse);
            }
            _context.GoodWarehouses.AddRange(goodWarehouses);
            _context.GoodWarehouses.UpdateRange(goodWarehouseUpdates);
            await _context.SaveChangesAsync();
            var shelve = await _context.WareHouseShelves.FirstOrDefaultAsync();
            var position = await _context.GoodWarehousesPositions.FirstOrDefaultAsync();
            var floor = await _context.WareHouseFloors.FirstOrDefaultAsync();
            var goodWarehousesPositions = goodWarehouses.Select(x => new GoodWarehousesPosition
            {
                GoodWarehousesId = x.Id,
                Quantity = x.Quantity,
                Warehouse = x.Warehouse,
                WareHouseShelvesId = shelve?.Id ?? 0,
                WareHouseFloorId = floor?.Id ?? 0,
                WareHousePositionId = position?.Id ?? 0

            });
            _context.GoodWarehousesPositions.AddRange(goodWarehousesPositions);
            await _context.SaveChangesAsync();
        }
        return "";
    }
    public async Task<string> SyncTonKho(int year)
    {
        var listAccount = _context.GetChartOfAccount(year)
              .Where(x => (x.Classification == 2 || x.Classification == 3) && x.DisplayInsert).ToList();
        if (listAccount.Count > 0)
        {
            var goodWareHouses = _context.GoodWarehouses.ToList();
            
            var accountCode = "";
            var detail1 = "";
            var detail2 = "";
            foreach (var account in listAccount)
            {
                double nhapKhos = 0;
                double xuatKhos = 0;
                if (account.Type == 6)
                {
                    var parentCodes = account.ParentRef.Split(":");
                    accountCode = parentCodes[0];
                    detail1 = parentCodes[1];
                    detail2 = account.Code;
                }
                else
                {
                    accountCode = account.ParentRef;
                    detail1 = account.Code;
                    detail2 = "";
                }
                var listNhapKho = _context.GetLedger(year).Where(x => x.DebitCode == accountCode
                                && (string.IsNullOrEmpty(detail1) || x.DebitDetailCodeFirst == detail1)
                                && (string.IsNullOrEmpty(detail2) || x.DebitDetailCodeSecond == detail2)
                                && (string.IsNullOrEmpty(account.WarehouseCode) || x.DebitWarehouse == account.WarehouseCode)
                                ).ToList();
                var orginalVoucherNumbers = listNhapKho.Select(x => x.OrginalVoucherNumber).Distinct().ToList();
                foreach (var orginalVoucherNumber in orginalVoucherNumbers)
                {
                    nhapKhos = listNhapKho.Where(x => x.OrginalVoucherNumber == orginalVoucherNumber).Sum(x => x.Quantity);
                    xuatKhos = _context.GetLedger(year).Where(x => x.CreditCode == accountCode
                            && (string.IsNullOrEmpty(detail1) || x.CreditDetailCodeFirst == detail1)
                            && (string.IsNullOrEmpty(detail2) || x.CreditDetailCodeSecond == detail2)
                            && (string.IsNullOrEmpty(account.WarehouseCode) || x.CreditWarehouse == account.WarehouseCode)).Sum(x => x.Quantity);


                    double quantity = (account.OpeningStockQuantityNB ?? 0) + nhapKhos - xuatKhos;
                    var goodWareHouse = goodWareHouses.Find(x => x.Account == accountCode
                                    && (string.IsNullOrEmpty(detail1) || x.Detail1 == detail1)
                                    && (string.IsNullOrEmpty(detail2) || x.Detail2 == detail2)
                                    && (string.IsNullOrEmpty(account.WarehouseCode) || x.Warehouse == account.WarehouseCode)
                                    && x.OrginalVoucherNumber == orginalVoucherNumber);
                    if (quantity == 0 && goodWareHouse != null)
                    {
                        _context.GoodWarehouses.Remove(goodWareHouse);
                        continue;
                    }
                    if (quantity > 0 && goodWareHouse != null)
                    {
                        goodWareHouse.Quantity = quantity;
                        _context.GoodWarehouses.Update(goodWareHouse);
                        continue;
                    }
                    if (quantity > 0 && goodWareHouse == null)
                    {
                        goodWareHouse = new GoodWarehouses();
                        goodWareHouse.Account = accountCode;
                        goodWareHouse.Detail1 = detail1;
                        goodWareHouse.Detail2 = detail2;
                        goodWareHouse.Warehouse = account.WarehouseCode;
                        goodWareHouse.WarehouseName = account.WarehouseName;
                        goodWareHouse.Quantity = quantity;
                        goodWareHouse.Order = 0;
                        goodWareHouse.Status = 1;
                        goodWareHouse.OrginalVoucherNumber = orginalVoucherNumber;
                        _context.GoodWarehouses.Add(goodWareHouse);
                    }
                }

            }
            await _context.SaveChangesAsync();
        }
        return "";
    }

    public async Task<object> CompleteBill(List<BillDetailViewPaging> billDetails, bool isForce, int userId)
    {
        StringBuilder messResponse = new StringBuilder();
        var listExport = new List<GoodWarehouseExport>();
        var billId = billDetails[0].BillId;
        var bill = await _context.Bills.FindAsync(billId);
        if (bill == null)
        {
            return new
            {
                message = $"Không tìm thấy bill với id {billId}",
                retry = 0,
                status = 400
            };
        }
        foreach (var billDetail in billDetails)
        {
            var quantityTotal = billDetail.Suggestions.Sum(b => b.Positions.Sum(x => x.QuantityReal));
            if (billDetail.Quantity != quantityTotal)
            {
                return new
                {
                    message = $"Số lượng quét sản phẩm {billDetail.GoodsCode} khác với đơn hàng; ",
                    retry = 0,
                    status = 400
                };
            }

            foreach (var goodQRCheck in billDetail.Suggestions)
            {
                if (billDetail.Quantity < 1)
                    continue;
                goodQRCheck.RealQuantity = goodQRCheck.Positions.Sum(x => x.QuantityReal);
                if (goodQRCheck.Quantity > goodQRCheck.RealQuantity && (billDetail.Quantity - goodQRCheck.Quantity) > 0 && !isForce)
                {
                    messResponse.Append($"Sản phẩm {goodQRCheck.QrCode} sắp hết hạn; ");
                    continue;
                }
                int goodWareHouseId = int.Parse(goodQRCheck.QrCode.Split("-")[1]);
                billDetail.Quantity -= goodQRCheck.RealQuantity;
                var goodExport = new GoodWarehouseExport();
                goodExport.CreatedAt = DateTime.Now;
                goodExport.Quantity = goodQRCheck.RealQuantity;
                goodExport.BillId = billDetail.BillId;
                goodExport.GoodWarehouseId = goodWareHouseId;
                goodExport.DateExpiration = goodQRCheck.DateExpiration;
                goodExport.GoodsCode = billDetail.GoodsCode;
                goodExport.GoodsName = billDetail.GoodsName;
                goodExport.QrCode = goodQRCheck.QrCode;
                listExport.Add(goodExport);

                var goodWarehouse = _context.GoodWarehouses.Find(goodWareHouseId);

                if (goodWarehouse != null)
                {
                    goodWarehouse.Quantity -= goodQRCheck.RealQuantity;
                    // TODO: delete item phieu nhap
                    if (goodWarehouse.Quantity < 1)
                    {
                        _context.GoodWarehouses.Remove(goodWarehouse);
                        var goodWarehousesPositions = await _context.GoodWarehousesPositions.Where(x => x.GoodWarehousesId == goodWareHouseId).ToListAsync();
                        _context.GoodWarehousesPositions.RemoveRange(goodWarehousesPositions);
                    }
                    else
                    {
                        _context.Update(goodWarehouse);
                        if (goodQRCheck.Positions != null)
                        {
                            foreach (var positionDetail in goodQRCheck.Positions)
                            {
                                var position = await _context.GoodWarehousesPositions.FindAsync(positionDetail.Id);
                                if (position != null && positionDetail.QuantityReal > 0)
                                {
                                    position.Quantity = position.Quantity - positionDetail.QuantityReal;
                                    _context.Update(position);
                                }
                            }
                        }
                    }
                }
            }
        }
        await _context.SaveChangesAsync();

        string messResponseOut = messResponse.ToString();
        if (string.IsNullOrEmpty(messResponseOut))
        {
            var billTracking = _context.BillTrackings.FirstOrDefault(x => x.BillId == billId && x.Status != "Cancel" && x.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION);
            if (billTracking is null)
                return new { message = messResponseOut, retry = 1, status = 400 };

            if (billTracking.TranType.Contains(TranTypeConst.Paid))
                billTracking.TranType = TranTypeConst.Paid + "-" + TranTypeConst.Cooked;
            else
                billTracking.TranType = TranTypeConst.Cooked;

            billTracking.Note = string.Join("; ", billDetails.Where(x => !string.IsNullOrEmpty(x.Note)).Select(x => x.GoodsName + " - " + x.Note).ToList());
            billTracking.UpdateAt = DateTime.Now;

            _context.BillTrackings.Update(billTracking);
            // ghi xuat kho
            await _context.GoodWarehouseExport.AddRangeAsync(listExport);

            var reasonForManagers = billDetails.Where(x => !string.IsNullOrEmpty(x.ReasonForManager)).Select(x => x.GoodsCode + ", " + x.GoodsName + ", số bill" + bill.DisplayOrder + ", " + x.ReasonForManager);
            if (reasonForManagers.Any())
            {
                // select manager
                var roleAdminBranch = await _context.UserRoles.Where(x => x.Code == UserRoleConst.AdminBranch).Select(x => x.Id.ToString()).FirstOrDefaultAsync();
                if (roleAdminBranch != null)
                {
                    var users = await _context.Users.Where(X => X.UserRoleIds.Contains(roleAdminBranch) && !X.IsDelete).ToListAsync();
                    foreach (var user in users)
                    {
                        var billTrackingAdd = new BillTracking()
                        {
                            BillId = billDetails[0].BillId,
                            Status = "Success",
                            Note = string.Join("; ", reasonForManagers),
                            UserIdReceived = user.Id,
                            Type = BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION,
                            UserCode = userId.ToString(),
                        };
                        await _context.BillTrackings.AddAsync(billTrackingAdd);
                    }
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.BroadcastMessage();
                }
            }
            await _context.SaveChangesAsync();

            return new { message = "Thành công", retry = 0, status = 200 };
        }

        return new { message = messResponse, retry = 1, status = 400 };
    }
    public async Task Update(GoodWarehousesUpdateModel item, double quantityChange = 0)
    {
        var goodWarehouses = _context.GoodWarehouses.AsNoTracking().FirstOrDefault(x => x.Id == item.Id);
        if (goodWarehouses is null)
            return;
        goodWarehouses.Status = item.Status;
        goodWarehouses.MenuType = item.MenuType;
        goodWarehouses.Account = item.Account;
        goodWarehouses.AccountName = item.AccountName;
        goodWarehouses.Detail1 = item.Detail1;
        goodWarehouses.DetailName1 = item.DetailName1;
        goodWarehouses.Detail2 = item.Detail2;
        goodWarehouses.DetailName2 = item.DetailName2;
        goodWarehouses.Warehouse = item.Warehouse;
        goodWarehouses.WarehouseName = item.WarehouseName;
        goodWarehouses.OrginalVoucherNumber = item.OrginalVoucherNumber;
        goodWarehouses.LedgerId = item.LedgerId;
        goodWarehouses.Order = item.Order;
        goodWarehouses.DateManufacture = item.DateManufacture;
        goodWarehouses.DateExpiration = item.DateExpiration;
        goodWarehouses.Note = item.Note;
        goodWarehouses.Image1 = item.Image1;
        if(quantityChange > 0)
        {
            goodWarehouses.Quantity = quantityChange;
            goodWarehouses.QuantityInput = quantityChange;

        }
        _context.GoodWarehouses.Update(goodWarehouses);

        var positions = await _context.GoodWarehousesPositions.Where(x => x.GoodWarehousesId == item.Id).ToListAsync();
        _context.GoodWarehousesPositions.RemoveRange(positions);

        var goodWarehousesPositions = item.Positions
                            .ConvertAll(x => new GoodWarehousesPosition
                            {
                                GoodWarehousesId = item.Id,
                                WareHouseFloorId = x.WareHouseFloorId,
                                WareHouseShelvesId = x.WareHouseShelvesId,
                                WareHousePositionId = x.WareHousePositionId,
                                Warehouse = x.Warehouse,
                                Quantity = x.Quantity
                            });
        await _context.GoodWarehousesPositions.AddRangeAsync(goodWarehousesPositions);

        await _context.SaveChangesAsync();
    }

    public async Task<GoodWarehousesUpdateModel> GetById(int id)
    {
        var goodWareHouse = await _context.GoodWarehouses.FindAsync(id);
        var data = _mapper.Map<GoodWarehousesUpdateModel>(goodWareHouse);
        data.Positions = await _context.GoodWarehousesPositions
            .Where(X => X.GoodWarehousesId == id)
            .Select(x => _mapper.Map<GoodWarehousesPositionUpdateModel>(x))
            .ToListAsync();
        data.Positions = data.Positions.ConvertAll(x => { x.Warehouse = string.IsNullOrEmpty(x.Warehouse) ? goodWareHouse.Warehouse : x.Warehouse; return x; });
        return data;
    }

    public async Task<string> UpdatePrintedStatus(int[] ids)
    {
        var goodWarehouses = _context.GoodWarehouses.Where(x => ids.Contains(x.Id));
        foreach (var item in goodWarehouses)
        {
            item.IsPrinted = true;
            _context.GoodWarehouses.Update(item);
        }
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<ReportForBranchModel> ReportWareHouse(int warehouseId, int shelveId, int floorId, string type)
    {
        return type switch
        {
            "warehouse" => await ReportForWareHouse(warehouseId),
            "shelve" => await ReportForShelve(warehouseId, shelveId),
            _ => await ReportForFloor(warehouseId, shelveId, floorId),
        };
    }

    private async Task<ReportForBranchModel> ReportForWareHouse(int wareHouseId)
    {
        var warehouseCheck = await _context.Warehouses.FindAsync(wareHouseId);
        if (warehouseCheck == null)
            warehouseCheck = new Warehouse();

        var goodWareHouseChecks = await _context.GoodWarehousesPositions.Where(x => x.Warehouse == warehouseCheck.Code).ToListAsync();


        ReportForBranchModel itemWH = new()
        {
            Name = warehouseCheck?.Name,
            Quantity = goodWareHouseChecks.Sum(x => x.Quantity),
            Type = "warehouse",
            Id = wareHouseId,
            Items = new List<List<ReportForBranchModel>>()
        };
        var shelveIds = await _context.WareHouseWithShelves.Where(x => x.WareHouseId == wareHouseId).Select(x => x.WareHouseShelveId).Distinct().ToListAsync();
        var shelves = await _context.WareHouseShelves.Where(x => shelveIds.Contains(x.Id)).ToListAsync();

        var orderHorizontals = shelves.Select(x => x.OrderHorizontal).Distinct().OrderBy(x => x);
        var orderVerticals = shelves.Select(x => x.OrderVertical).Distinct().OrderBy(x => x);
        foreach (var orderVertical in orderVerticals)
        {
            List<ReportForBranchModel> verticals = new List<ReportForBranchModel>();
            foreach (var orderHorizontal in orderHorizontals)
            {
                var shelve = shelves.Find(x => x.OrderVertical == orderVertical && x.OrderHorizontal == orderHorizontal);
                if (shelve is null)
                {
                    verticals.Add(null);
                    continue;
                }
                var goodWareHousePositions = goodWareHouseChecks.Where(x => x.WareHouseShelvesId == shelve.Id);
                var goodWareHouseIds = goodWareHousePositions.Select(X => X.GoodWarehousesId).Distinct();
                var goodWareHouseNames = await _context.GoodWarehouses.Where(x => goodWareHouseIds.Contains(x.Id)).ToListAsync();

                var goodDetailNames = "";
                foreach (var goodDetail in goodWareHouseNames)
                {
                    goodDetailNames += (!string.IsNullOrEmpty(goodDetail.DetailName2) ? goodDetail.DetailName2 :
                                               (!string.IsNullOrEmpty(goodDetail.DetailName1) ? goodDetail.DetailName1 : goodDetail.AccountName)) + ": ";
                    goodDetailNames += goodWareHousePositions.Where(x => x.GoodWarehousesId == goodDetail.Id).Sum(x => x.Quantity) + "; ";
                }

                ReportForBranchModel itemSh = new()
                {
                    Name = shelve.Name,
                    Quantity = goodWareHousePositions.Sum(x => x.Quantity),
                    Type = "shelve",
                    GoodsDetails = goodDetailNames,
                    Id = shelve.Id,
                    Items = new List<List<ReportForBranchModel>>()
                };
                verticals.Add(itemSh);
            }
            itemWH.Items.Add(verticals);
        }
        return itemWH;
    }

    private async Task<ReportForBranchModel> ReportForShelve(int wareHouseId, int shelveId)
    {
        var wareHouseShelve = await _context.WareHouseShelves.FindAsync(shelveId);
        var warehouseCheck = await _context.Warehouses.FindAsync(wareHouseId);

        var goodWareHouseChecks = await _context.GoodWarehousesPositions.Where(x => x.WareHouseShelvesId == shelveId && x.Warehouse == warehouseCheck.Code).ToListAsync();

        ReportForBranchModel itemWH = new()
        {
            Name = wareHouseShelve.Name,
            Quantity = goodWareHouseChecks.Sum(x => x.Quantity),
            Type = "shelve",
            Id = shelveId,
            Items = new List<List<ReportForBranchModel>>()
        };
        var floorIds = await _context.WareHouseShelvesWithFloors.Where(x => x.WareHouseShelvesId == shelveId).Select(X => X.WareHouseFloorId).Distinct().ToListAsync();
        var floors = await _context.WareHouseFloors.Where(x => floorIds.Contains(x.Id)).OrderBy(x => x.Name).ToListAsync();
        List<ReportForBranchModel> verticals = new List<ReportForBranchModel>();

        foreach (var floor in floors)
        {

            var goodWareHousePositions = goodWareHouseChecks.Where(x => x.WareHouseFloorId == floor.Id);
            var goodWareHouseIds = goodWareHousePositions.Select(X => X.GoodWarehousesId).Distinct();
            var goodWareHouseNames = await _context.GoodWarehouses.Where(x => goodWareHouseIds.Contains(x.Id)).ToListAsync();
            var goodDetailNames = "";
            foreach (var goodDetail in goodWareHouseNames)
            {
                goodDetailNames += (!string.IsNullOrEmpty(goodDetail.DetailName2) ? goodDetail.DetailName2 :
                                           (!string.IsNullOrEmpty(goodDetail.DetailName1) ? goodDetail.DetailName1 : goodDetail.AccountName)) + ": ";
                goodDetailNames += goodWareHousePositions.Where(x => x.GoodWarehousesId == goodDetail.Id).Sum(x => x.Quantity) + "; ";
            }

            ReportForBranchModel itemSh = new()
            {
                Name = floor.Name,
                Quantity = goodWareHousePositions.Sum(x => x.Quantity),
                GoodsDetails = goodDetailNames,
                Type = "floor",
                Id = floor.Id,
                Items = new List<List<ReportForBranchModel>>()
            };
            verticals.Add(itemSh);
        }
        itemWH.Items.Add(verticals);

        return itemWH;
    }

    private async Task<ReportForBranchModel> ReportForFloor(int wareHouseId, int shelveId, int floorId)
    {
        var wareHouseFloor = await _context.WareHouseFloors.FindAsync(floorId);
        var warehouseCheck = await _context.Warehouses.FindAsync(wareHouseId);

        var goodWareHouseChecks = await _context.GoodWarehousesPositions.Where(x => x.WareHouseFloorId == floorId && x.Warehouse == warehouseCheck.Code
            && x.WareHouseShelvesId == shelveId).ToListAsync();


        ReportForBranchModel itemWH = new()
        {
            Name = wareHouseFloor.Name,
            Quantity = goodWareHouseChecks.Sum(x => x.Quantity),
            Type = "floor",
            Id = floorId,
            Items = new List<List<ReportForBranchModel>>()
        };

        var positionIds = await _context.WareHouseFloorWithPositions.Where(x => x.WareHouseFloorId == floorId).Select(X => X.WareHousePositionId).Distinct().ToListAsync();

        var positions = await _context.WareHousePositions.Where(x => positionIds.Contains(x.Id)).ToListAsync();
        List<ReportForBranchModel> verticals = new List<ReportForBranchModel>();

        foreach (var position in positions)
        {
            var goodWareHousePositions = goodWareHouseChecks.Where(x => x.WareHousePositionId == position.Id);
            var goodWareHouseIds = goodWareHousePositions.Select(X => X.GoodWarehousesId).Distinct();
            var goodWareHouseNames = await _context.GoodWarehouses.Where(x => goodWareHouseIds.Contains(x.Id)).ToListAsync();


            var goodDetailNames = "";
            foreach (var goodDetail in goodWareHouseNames)
            {
                goodDetailNames += (!string.IsNullOrEmpty(goodDetail.DetailName2) ? goodDetail.DetailName2 :
                                           (!string.IsNullOrEmpty(goodDetail.DetailName1) ? goodDetail.DetailName1 : goodDetail.AccountName)) + ": ";
                goodDetailNames += goodWareHousePositions.Where(x => x.GoodWarehousesId == goodDetail.Id).Sum(x => x.Quantity) + "; ";
            }
            ReportForBranchModel itemSh = new()
            {
                Name = position.Name,
                Quantity = goodWareHousePositions.Sum(x => x.Quantity),
                GoodsDetails = goodDetailNames,
                Type = "position",
                Id = position.Id,
                Items = new List<List<ReportForBranchModel>>()
            };
            verticals.Add(itemSh);
        }
        itemWH.Items.Add(verticals);

        return itemWH;
    }

    public async Task Create(Ledger entity, int year)
    {
        ChartOfAccount chartOfAccount = await _chartOfAccountService.GetAccountByCode(entity.DebitCode, year);
        if ((chartOfAccount?.Classification == 2 || chartOfAccount?.Classification == 3))
        {
            var order = await _context.GoodWarehouses.OrderByDescending(x => x.Order).Where(x => x.Account == entity.DebitCode
                                        && x.Detail1 == entity.DebitDetailCodeFirst
                                        && x.Detail2 == entity.DebitDetailCodeSecond
                                        && (string.IsNullOrEmpty(entity.DebitWarehouse) || x.Warehouse == entity.DebitWarehouse)).Select(x => x.Order).FirstOrDefaultAsync();
            var goodWareHouse = new GoodWarehouses
            {
                Account = entity.DebitCode,
                AccountName = entity.DebitCodeName,
                Detail1 = entity.DebitDetailCodeFirst,
                DetailName1 = entity.DebitDetailCodeFirstName,
                Detail2 = entity.DebitDetailCodeSecond,
                DetailName2 = entity.DebitDetailCodeSecondName,
                Warehouse = entity.DebitWarehouse,
                WarehouseName = entity.DebitWarehouseName,
                Quantity = entity.Quantity,
                Order = (order ?? 0) + 1,
                OrginalVoucherNumber = entity.OrginalVoucherNumber,
                LedgerId = (int?)entity.Id,
                Status = 1,
                QuantityInput = entity.Quantity
            };
            await _context.GoodWarehouses.AddAsync(goodWareHouse);
            await _context.SaveChangesAsync();

            var shelve = await _context.WareHouseShelves.FirstOrDefaultAsync();
            var position = await _context.GoodWarehousesPositions.FirstOrDefaultAsync();
            var floor = await _context.WareHouseFloors.FirstOrDefaultAsync();
            var goodWarehousesPositions = new GoodWarehousesPosition
            {
                GoodWarehousesId = goodWareHouse.Id,
                Quantity = goodWareHouse.Quantity,
                Warehouse = goodWareHouse.Warehouse,
                WareHouseShelvesId = shelve?.Id ?? 0,
                WareHouseFloorId = floor?.Id ?? 0,
                WareHousePositionId = position?.Id ?? 0

            };
            _context.GoodWarehousesPositions.AddRange(goodWarehousesPositions);
            await _context.SaveChangesAsync();
        }
    } 
}