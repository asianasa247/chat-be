using AutoMapper;
using Common.Constants;
using DinkToPdf.Contracts;
using ManageEmployee.Handlers;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;

namespace ManageEmployee.Services.LedgerServices;

public class LedgerService : ILedgerService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILedgerHelperService _ledgerHelperService;
    private readonly IMapper _mapper;
    private readonly ICompanyService _companyService;
    private readonly IAccountBalanceSheetService _accountBalanceSheet;
    private readonly IConverter _converterPDF;
    private readonly ILedgerFixedAssetService _ledgerFixedAssetService;
    private readonly ILedgerUpdateChartOfAccountNameService _ledgerUpdateChartOfAccountNameService;
    private readonly IGoodWarehousesService _goodWarehousesService;

    public LedgerService(
        ApplicationDbContext context, IChartOfAccountService chartOfAccountService,
        ILedgerHelperService ledgerHelperService, IMapper mapper, ICompanyService companyService
        , IConverter converterPDF,
        IAccountBalanceSheetService accountBalanceSheet
, ILedgerFixedAssetService ledgerFixedAssetService, ILedgerUpdateChartOfAccountNameService ledgerUpdateChartOfAccountNameService, IGoodWarehousesService goodWarehousesService)
    {
        _context = context;
        _chartOfAccountService = chartOfAccountService;
        _ledgerHelperService = ledgerHelperService;
        _mapper = mapper;
        _companyService = companyService;
        _accountBalanceSheet = accountBalanceSheet;
        _converterPDF = converterPDF;
        _ledgerFixedAssetService = ledgerFixedAssetService;
        _ledgerUpdateChartOfAccountNameService = ledgerUpdateChartOfAccountNameService;
        _goodWarehousesService = goodWarehousesService;
    }

    public async Task<PagingResultLedger<LedgerModel>> GetPage(LedgerRequestModel request, int year)
    {
        var searchQuery = string.IsNullOrWhiteSpace(request.SearchText) ? "" : request.SearchText.Trim().ToLower();
        if (request.Page == 0)
        {
            request.Page = 1;
        }
        var response = new PagingResultLedger<LedgerModel>()
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
        };

        var listType = new List<string>();
        if (request.DocumentType == nameof(AssetsType.PB))
        {
            listType.Add(AssetsType.PB.ToString());
            listType.Add(AssetsType.CCDCSD.ToString());
        }
        else if (!string.IsNullOrEmpty(request.DocumentType))
        {
            listType.Add(request.DocumentType);
        }

        var query = _context.GetLedger(year, request.IsInternal)
                          .Where(l => (string.IsNullOrEmpty(request.DocumentType) || listType.Contains(l.Type)) &&
                                      (request.FilterMonth == 0 || request.FilterMonth > 0 && l.Month == request.FilterMonth))
                          .Where(x => x.OrginalVoucherNumber.Contains(searchQuery)
                                   || x.DebitCode.ToLower().Contains(searchQuery) || x.DebitCodeName.ToLower().Contains(searchQuery)
                                   || x.DebitDetailCodeFirst.ToLower().Contains(searchQuery) || x.DebitDetailCodeFirstName.ToLower().Contains(searchQuery)
                                   || x.DebitDetailCodeSecond.ToLower().Contains(searchQuery) || x.DebitDetailCodeSecondName.ToLower().Contains(searchQuery)
                                   || x.CreditCode.ToLower().Contains(searchQuery) || x.CreditCode.ToLower().Contains(searchQuery)
                                   || x.CreditDetailCodeFirst.ToLower().Contains(searchQuery) || x.CreditDetailCodeFirstName.ToLower().Contains(searchQuery)
                                   || x.CreditDetailCodeSecond.ToLower().Contains(searchQuery) || x.CreditDetailCodeSecondName.ToLower().Contains(searchQuery)
                                   || x.OrginalDescription.Contains(searchQuery)
                                   || x.InvoiceNumber.Contains(searchQuery)
                                   || x.InvoiceTaxCode.Contains(searchQuery)
                                   || x.VoucherNumber.Contains(searchQuery)
                                   )
                          .OrderByDescending(s => s.Order).ThenByDescending(s => s.Id);

        response.TotalItems = await query.CountAsync();
        response.NextStt = await _context.GetLedger(year, request.IsInternal).OrderByDescending(x => x.Order).Where(x => x.Month == request.FilterMonth
        && (listType.Count == 0 ? x.Type == "PT" : listType.Contains(x.Type))).Select(x => x.Order).FirstOrDefaultAsync() + 1;
        var ledgers = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        response.Data = await TotalAmount(ledgers, request.IsInternal, year);

        return response;
    }

    private async Task<List<LedgerModel>> TotalAmount(List<Ledger> ledgers, int isInternal, int year)
    {
        var orginalVoucherNumbers = ledgers.Select(x => x.OrginalVoucherNumber).Distinct();
        var ledgerFinds = await _context.GetLedger(year, isInternal).Where(x => orginalVoucherNumbers.Contains(x.OrginalVoucherNumber)).ToListAsync();

        var itemOuts = new List<LedgerModel>();
        foreach (var ledger in ledgers)
        {
            var ledgerModel = _mapper.Map<LedgerModel>(ledger);
            ledgerModel.TotalAmount = ledgerFinds.Where(x => x.OrginalVoucherNumber == ledger.OrginalVoucherNumber).Sum(x => x.Amount);
            itemOuts.Add(ledgerModel);
        }
        return itemOuts;
    }

    private async Task<ChartOfAccount> UpdateChartOfAccount(string code, double amount, double? stockQuantity, int year, bool isCredit = false,
                string parentRef = "", string wareHouseCode = "", string _operator = "+", int isInternal = 1)
    {
        if (string.IsNullOrEmpty(code))// || amount == 0
        {
            return new ChartOfAccount();
        }
        ChartOfAccount currentAccount = await _chartOfAccountService.GetAccountByCode(code, year, parentRef, wareHouseCode);

        if (currentAccount == null)
        {
            // Nếu không tìm thấy cũng bỏ qua
            return new ChartOfAccount();
        }

        // Nếu là ngoại tệ
        if (currentAccount.IsForeignCurrency)
        {
            if (!isCredit)
            {
                if (_operator is "+")
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingForeignDebit = (currentAccount.ArisingForeignDebit ?? 0) + amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingForeignDebitNB = (currentAccount.ArisingForeignDebitNB ?? 0) + amount;
                    }
                }
                else
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingForeignDebit = (currentAccount.ArisingForeignDebit ?? 0) - amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingForeignDebitNB = (currentAccount.ArisingForeignDebitNB ?? 0) - amount;
                    }
                }
            }
            else
            {
                if (_operator is "+")
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingForeignCredit = (currentAccount.ArisingForeignCredit ?? 0) + amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingForeignCreditNB = (currentAccount.ArisingForeignCreditNB ?? 0) + amount;
                    }
                }
                else
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingForeignCredit = (currentAccount.ArisingForeignCredit ?? 0) - amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingForeignCreditNB = (currentAccount.ArisingForeignCreditNB ?? 0) - amount;
                    }
                }
            }
        }
        else // Không phải ngoại tệ
        {
            if (!isCredit)
            {
                if (_operator is "+")
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingDebit = (currentAccount.ArisingDebit ?? 0) + amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingDebitNB = (currentAccount.ArisingDebitNB ?? 0) + amount;
                    }
                }
                else
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingDebit = (currentAccount.ArisingDebit ?? 0) - amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingDebitNB = (currentAccount.ArisingDebitNB ?? 0) - amount;
                    }
                }
            }
            else
            {
                if (_operator is "+")
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingCredit = (currentAccount.ArisingCredit ?? 0) + amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingCreditNB = (currentAccount.ArisingCreditNB ?? 0) + amount;
                    }
                }
                else
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingCredit = (currentAccount.ArisingCredit ?? 0) - amount;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingCreditNB = (currentAccount.ArisingCreditNB ?? 0) - amount;
                    }
                }
            }
        }

        // Tính toán số lượng
        if (stockQuantity != null)
        {
            if (!isCredit)
            {
                if (_operator is "+")
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingStockQuantity = (currentAccount.ArisingStockQuantity ?? 0) + stockQuantity;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingStockQuantityNB = (currentAccount.ArisingStockQuantityNB ?? 0) + stockQuantity;
                    }
                }
                else
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingStockQuantity = (currentAccount.ArisingStockQuantity ?? 0) - stockQuantity;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingStockQuantityNB = (currentAccount.ArisingStockQuantityNB ?? 0) - stockQuantity;
                    }
                }
            }
            else
            {
                if (_operator is "+")
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingStockQuantity = (currentAccount.ArisingStockQuantity ?? 0) - stockQuantity;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingStockQuantityNB = (currentAccount.ArisingStockQuantityNB ?? 0) - stockQuantity;
                    }
                }
                else
                {
                    if (isInternal == 1)
                    {
                        currentAccount.ArisingStockQuantity = (currentAccount.ArisingStockQuantity ?? 0) + stockQuantity;
                    }
                    if (isInternal == 3)
                    {
                        currentAccount.ArisingStockQuantityNB = (currentAccount.ArisingStockQuantityNB ?? 0) + stockQuantity;
                    }
                }
            }
        }

        _chartOfAccountService.UpdateAccount(currentAccount);
        return currentAccount;
    }

    public async Task<Ledger> Create(Ledger entity, int year)
    {
        entity.CreateAt = DateTime.UtcNow;
        entity.UpdateAt = DateTime.UtcNow;
        ValidateOriginDate(entity, year);

        int.TryParse(entity.OrginalVoucherNumber.Split('-').Last(), out int orderNum);
        if (orderNum > 0)
        {
            entity.Order = orderNum;
        }

        //Cập nhật tên định khoản
        entity = await _ledgerUpdateChartOfAccountNameService.UpdateChartOfAccountName(entity, year);
        await updateChartOfAccountLedger(entity, year);
      
       

        if (entity.IsInternal == 0) 
        {
            string entityInternalString = JsonConvert.SerializeObject(entity);
            Ledger entityInternal = JsonConvert.DeserializeObject<Ledger>(entityInternalString);

            entityInternal.Id = 0;
            entityInternal.IsInternal = 3;
            await _context.Ledgers.AddAsync(entityInternal);
        }
        if (entity.IsInternal == 0)
        {
            entity.IsInternal = 1; // Set default value for IsInternal if not set
        }

        await _context.Ledgers.AddAsync(entity);
        await _context.SaveChangesAsync();

        if (entity.IsInternal == 1 || entity.IsInternal == 3)
        {
            bool isCheckNhapKho = entity.Type == "NK" || entity.Type == "PC" && !string.IsNullOrEmpty(entity.ReferenceVoucherNumber);
            if (isCheckNhapKho)
            {
                await _goodWarehousesService.Create(entity, year);
            }
        }
        return entity;
    }

    private async Task UpdateRelationData(Ledger currentLedger, Ledger entity, int year)
    {
        var relations = await _context.GetLedger(year, currentLedger.IsInternal).Where(x =>
        x.OrginalVoucherNumber == currentLedger.OrginalVoucherNumber &&
        x.InvoiceTaxCode == currentLedger.InvoiceTaxCode &&
        x.InvoiceName == currentLedger.InvoiceName &&
        x.InvoiceAddress == currentLedger.InvoiceAddress &&
        x.Id != currentLedger.Id).ToListAsync();

        relations.ForEach(x =>
        {
            x.InvoiceTaxCode = entity.InvoiceTaxCode;
            x.InvoiceName = entity.InvoiceName;
            x.InvoiceAddress = entity.InvoiceAddress;
        });

        _context.Ledgers.UpdateRange(relations);
        await _context.SaveChangesAsync();
    }

    public async Task<Ledger> Update(Ledger currentLedger, Ledger entity, int year)
    {
        ValidateOriginDate(currentLedger, year);
        // validate for good warehouse

        #region Xóa các giá trị theo thực thể cũ

        await updateChartOfAccountLedger(entity, year, "+", currentLedger);

        #endregion Xóa các giá trị theo thực thể cũ

        // Cập nhật thay đổi cho các thực thể tương tự có liên quan
        await UpdateRelationData(currentLedger, entity, year);

        if (entity.OrginalVoucherNumber.Contains("-"))
        {
            int.TryParse(entity.OrginalVoucherNumber.Split('-').Last(), out int orderNum);
            if (orderNum > 0)
            {
                entity.Order = orderNum;
            }
        }

        currentLedger.UpdateAt = DateTime.Now;

        //Cập nhật tên định khoản
        entity = await _ledgerUpdateChartOfAccountNameService.UpdateChartOfAccountName(entity, year);

        // Map giá trị mới sang giá trị cũ
        currentLedger.CheckAndMap(entity);
        currentLedger.UpdateAt = DateTime.Now;

        _context.Ledgers.Update(currentLedger);
        await _context.SaveChangesAsync();

        // check ccdc
        await _ledgerFixedAssetService.UpdateAsync(currentLedger, year);

        // Nếu update phiếu xuất kho thì update goodwarenhouse  Hùng làm
        var item = await _context.GoodWarehouses.FirstOrDefaultAsync(x => x.LedgerId == entity.Id);


        if (item != null)
        {
            var shelve = await _context.WareHouseShelves.FirstOrDefaultAsync();
            var position = await _context.GoodWarehousesPositions.FirstOrDefaultAsync();
            var floor = await _context.WareHouseFloors.FirstOrDefaultAsync();

            var goodWarehousesPositions = new List<GoodWarehousesPositionUpdateModel>
            {
                new GoodWarehousesPositionUpdateModel
                {
                    Quantity = entity.Quantity,
                    Warehouse = entity.DebitWarehouse,
                    WareHouseShelvesId = shelve?.Id ?? 0,
                    WareHouseFloorId = floor?.Id ?? 0,
                    WareHousePositionId = position?.Id ?? 0
                }
            };
            var goodWarehouse = new GoodWarehousesUpdateModel
            {
                Id = item.Id,
                Account = entity.DebitCode,
                AccountName = entity.DebitCodeName,
                Detail1 = entity.DebitDetailCodeFirst,
                DetailName1 = entity.DebitDetailCodeFirstName,
                Detail2 = entity.DebitDetailCodeSecond,
                DetailName2 = entity.DebitDetailCodeSecondName,
                Warehouse = entity.DebitWarehouse,
                WarehouseName = entity.DebitWarehouseName,
                Quantity = entity.Quantity,
                Order = item.Order,
                OrginalVoucherNumber = entity.OrginalVoucherNumber,
                LedgerId = (int?)entity.Id,
                Status = 1,
                Positions = goodWarehousesPositions
            };
            await _goodWarehousesService.Update(goodWarehouse, entity.Quantity);
        }
        else
        {
            // Nếu phát sinh là Cả hai hoặc Nội bộ và tồn tại mã phiếu chi nhập kho và không tồn tại hàng hóa thì thêm mới
            if ((entity.IsInternal == 0 || entity.IsInternal == 3) && entity.Type == "PC" && !string.IsNullOrWhiteSpace(entity.ReferenceVoucherNumber))
            {
                await _goodWarehousesService.Create(entity, year);
            }
            // Ghi chú, ghi chú đặt biệt: Khi chuyển chứng từ F7 số 9. Dropdown Ẩn Chứng từ Nhập Kho
            // Vì nếu chuyển 1 chứng từ khác vào Nhập kho là Ko lưu trong GoodWarenhouse. Và mình cũng ko biết đường nhấn Sửa để Update. Nên Loại ra
            // Khi chuyển chứng từ Phiếu chi có Nhập kho qua Chứng từ Khác thì ko cho chuyển.
        }

        await _context.SaveChangesAsync();
        return currentLedger;
    }

    public async Task<string> Delete(string idsStr, int isInternal, int year)
    {
        var ids = idsStr.Split(",").Select(x => Convert.ToInt64(x));
        StringBuilder message = new();
        if (ids.Any())
        {
            foreach (var id in ids)
            {
                var entity = await _context.GetLedger(year, isInternal).Where(x => x.Id == Convert.ToInt64(id)).FirstOrDefaultAsync();
                if (entity != null)
                {
                    // Nếu update phiếu xuất kho thì update goodwarenhouse  Hùng làm
                    var item = _context.GoodWarehouses.Where(x => x.LedgerId == entity.Id).FirstOrDefault();

                    if (item != null)
                    {
                        var goodWareHousesPositionItems = _context.GoodWarehousesPositions.Where(x => x.GoodWarehousesId == item.Id)
                            .ToList();
                        _context.GoodWarehousesPositions.RemoveRange(goodWareHousesPositionItems);
                        _context.GoodWarehouses.Remove(item);
                    }

                    _context.Ledgers.Remove(entity);
                    await _context.SaveChangesAsync();
                    await updateChartOfAccountLedger(entity, year, "-");
                }
                else
                {
                    message.Append(ErrorMessages.DataNotFound);
                }
            }
        }
        return message.ToString();
    }

    public async Task<bool> XoaXuatKhoGiaVon()
    {
        var oldLegers = await _context.Ledgers.Where(x => x.OrginalDescription.StartsWith("Xuất kho giá vốn"))
            .AsNoTracking().ToListAsync();
        _context.Ledgers.RemoveRange(oldLegers);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    private void ValidateOriginDate(Ledger entity, int year)
    {
        if (entity.OrginalBookDate.Value.Year != year)
            throw new Exception("Năm của ngảy ghi sổ đang khác với năm bạn đang làm việc");
    }

    private async Task updateChartOfAccountLedger(Ledger entity, int year, string _operator = "+", Ledger currentEntity = null)
    {
        // Cập nhật tài khoản nợ
        double amount = currentEntity?.Amount ?? 0;
        double quantity = currentEntity?.Quantity ?? 0;

        await UpdateChartOfAccount(entity.DebitCode, entity.Amount - amount, entity.Quantity - quantity, year, false, string.Empty, string.Empty, _operator: _operator, isInternal: entity.IsInternal);
        // Cập nhật chi tiết nợ 1
        await UpdateChartOfAccount(entity.DebitDetailCodeFirst, entity.Amount - amount, entity.Quantity - quantity, year, false, entity.DebitCode, entity.DebitWarehouse, _operator: _operator, isInternal: entity.IsInternal);
        // Cập nhật chi tiết nợ 2
        await UpdateChartOfAccount(entity.DebitDetailCodeSecond, entity.Amount - amount, entity.Quantity - quantity, year, false,
entity.DebitCode + ":" + entity.DebitDetailCodeFirst, entity.DebitWarehouse, _operator: _operator, isInternal: entity.IsInternal);

        // Cập nhật tài khoản có
        await UpdateChartOfAccount(entity.CreditCode, entity.Amount - amount, entity.Quantity - quantity, year, true, string.Empty, string.Empty, _operator: _operator, isInternal: entity.IsInternal);
        // Cập nhật chi tiết có 1
        await UpdateChartOfAccount(entity.CreditDetailCodeFirst, entity.Amount - amount, entity.Quantity - quantity, year, true, entity.CreditCode, entity.CreditWarehouse, _operator: _operator, isInternal: entity.IsInternal);
        // Cập nhật chi tiết có 2
        await UpdateChartOfAccount(entity.CreditDetailCodeSecond, entity.Amount - amount, entity.Quantity - quantity, year, true,
entity.CreditCode + ":" + entity.CreditDetailCodeFirst, entity.CreditWarehouse, _operator: _operator, isInternal: entity.IsInternal);
    }

    public async Task<Ledger> GetLedgerById(long Id, int isInternal)
    {
        return await _context.Ledgers.FindAsync(Id);
    }

    public async Task<List<LedgerCostOfGoodsModel>> GetCostOfGoods(int iMonth, int iYear, int isInternal, int year)
    {
        List<LedgerCostOfGoodsModel> lstCostOfGoods = new();

        DateTime dtEntryDate = new DateTime(iYear, iMonth, 1, 0, 0, 0, DateTimeKind.Local).AddMonths(1).AddDays(-1);

        _ledgerHelperService.SetAutoIncrement(dtEntryDate, AssetsType.XH, isInternal, year);
        List<string> lstCodeSync = await (
                                        from gl in _context.GetChartOfAccountGroupLink(year)
                                        join co in _context.GetChartOfAccount(year) on gl.CodeChartOfAccount equals co.Code
                                        where co.Code.Substring(0, 1) == "5"
                                        select co.Code
                                    ).Distinct().ToListAsync();

        int iMethodCalcExportPrice = await _context.Companies.OrderByDescending(t => t.UpdateAt).Select(x => x.MethodCalcExportPrice).FirstOrDefaultAsync();
        var maxOriginalVoucher = 0;

        var lstLedger = await _context.GetLedger(year, isInternal).Where(t => lstCodeSync.Contains(t.CreditCode)
                                                             && t.Month == iMonth
                                                             && t.OrginalBookDate.Value.Year == iYear
                                                  )
                                .OrderBy(x => x.OrginalBookDate).ToListAsync();
        var ledgerExist = await _context.GetLedger(year, isInternal).Where(x => !x.IsDelete && x.Type == "XK"
                                                 && x.OrginalBookDate.Value.Month == iMonth).ToListAsync();
        if (ledgerExist != null && ledgerExist.Count > 0)
        {
            maxOriginalVoucher = ledgerExist.Max(x => int.Parse(x.OrginalVoucherNumber.Split('-').Last()));
        }
        maxOriginalVoucher++;

        List<ChartOfAccount> listCoaDebitDefault = await _context.GetChartOfAccount(year).Where(co => co.Type < 5).ToListAsync();

        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
        DateTime toDate = new DateTime(iYear, iMonth, 1, 0, 0, 0, DateTimeKind.Local);
        List<ChartOfAccountGroupLink> listDebitCodeGroupLink = await (
                                        from gl in _context.GetChartOfAccountGroupLink(year)
                                        join co in _context.GetChartOfAccount(year) on gl.CodeChartOfAccount equals co.Code
                                        select gl).ToListAsync();
        List<SoChiTietViewModel> listXuatKho = null;
        var dayOrginalBookDate = lstLedger.FirstOrDefault()?.OrginalBookDate.Value.Day;

        foreach (var ledger in lstLedger)
        {
            if (ledger.OrginalBookDate.Value.Day != dayOrginalBookDate)
            {
                dayOrginalBookDate = ledger.OrginalBookDate.Value.Day;
                maxOriginalVoucher++;
                orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
            }
            ChartOfAccountGroupLink debitCodeGroupLink = listDebitCodeGroupLink.Find(x => x.CodeChartOfAccount == ledger.CreditCode);
            List<string> crebitCodeGroupLink = listDebitCodeGroupLink.Where(x => x.CodeChartOfAccountGroup == debitCodeGroupLink.CodeChartOfAccountGroup).Select(x => x.CodeChartOfAccount).ToList();

            string account6Code = crebitCodeGroupLink.Find(x => x.Substring(0, 1) == "6");
            string account1Code = crebitCodeGroupLink.Find(x => x.Substring(0, 1) == "1");
            ChartOfAccount account6 = listCoaDebitDefault.Find(x => x.Code == account6Code);
            ChartOfAccount account1 = listCoaDebitDefault.Find(x => x.Code == account1Code);

            if (iMethodCalcExportPrice == 1)
            {
                var ledgerCheck = lstCostOfGoods.Find(x => x.CreditDetailCodeFirst == ledger.CreditDetailCodeFirst
                && x.CreditDetailCodeSecond == ledger.CreditDetailCodeSecond && x.CreditWarehouse == ledger.CreditWarehouse
                && x.CreditCode == account1.Code);
                if (ledgerCheck != null)
                {
                    ledgerCheck.Quantity += ledger.Quantity;
                    ledgerCheck.Amount += ledger.UnitPrice * ledger.Quantity;
                    ledgerCheck.RevenueAmmountPrice = ledgerCheck.Amount;
                    continue;
                }
            }
            double dUnitPriceAvg = await TinhGiaXuatKho(account1.Code, ledger.CreditDetailCodeFirst, ledger.CreditDetailCodeSecond,
                ledger.CreditWarehouse, iMethodCalcExportPrice == 1 ? toDate : ledger.OrginalBookDate ?? DateTime.Today, year, iMethodCalcExportPrice, isInternal == 3, listXuatKho);

            string maHang = "";
            if (!string.IsNullOrEmpty(ledger.CreditDetailCodeSecondName))
                maHang = ledger.CreditDetailCodeSecondName;
            else if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirstName))
                maHang = ledger.CreditDetailCodeFirstName;
            else
                maHang = ledger.CreditCodeName;
            string voucherMonth = iMonth < 10 ? "0" + iMonth : iMonth.ToString();

            LedgerCostOfGoodsModel item = new LedgerCostOfGoodsModel
            {
                Type = "XK",
                Id = ledger.Id,
                Month = iMonth,
                OrginalBookDate = ledger.OrginalBookDate,
                OrginalDescription = $"Xuất kho giá vốn {maHang}",
                DebitCode = account6?.Code,
                DebitCodeName = account6?.Name,
                DebitDetailCodeFirst = ledger.CreditDetailCodeFirst,
                DebitDetailCodeFirstName = ledger.CreditDetailCodeFirstName,
                DebitDetailCodeSecond = ledger.CreditDetailCodeSecond,
                DebitDetailCodeSecondName = ledger.CreditDetailCodeSecondName,

                CreditCode = account1?.Code,
                CreditCodeName = account1?.Name,
                CreditDetailCodeFirst = ledger.CreditDetailCodeFirst,
                CreditDetailCodeFirstName = ledger.CreditDetailCodeFirstName,
                CreditDetailCodeSecond = ledger.CreditDetailCodeSecond,
                CreditDetailCodeSecondName = ledger.CreditDetailCodeSecondName,
                Quantity = ledger.Quantity,
                CreditWarehouse = ledger.CreditWarehouse,
                CreditWarehouseName = ledger.CreditWarehouseName,
                VoucherNumber = voucherMonth + "/" + "XK",
                OrginalVoucherNumber = $"XK{voucherMonth}-{DateTime.Now.Year.ToString().Substring(2, 2)}-{orderString}",
                Order = maxOriginalVoucher,
                RevenueCode = ledger.CreditCode,
                RevenueUnitPrice = Math.Round(dUnitPriceAvg, 0),
                RevenueAmmountPrice = Math.Round(dUnitPriceAvg, 0) * ledger.Quantity,
                UnitPrice = ledger.UnitPrice,
                Amount = ledger.UnitPrice * ledger.Quantity,
            };

            lstCostOfGoods.Add(item);
            if (iMethodCalcExportPrice == 2)
            {
                if (listXuatKho == null)
                    listXuatKho = new List<SoChiTietViewModel>();
                SoChiTietViewModel xuatKho = new SoChiTietViewModel
                {
                    DebitCode = item.DebitCode,
                    DebitDetailCodeSecond = item.DebitDetailCodeSecond,
                    DebitDetailCodeFirst = item.DebitDetailCodeFirst,
                    CreditCode = item.CreditCode,
                    CreditDetailCodeFirst = item.CreditDetailCodeFirst,
                    CreditDetailCodeSecond = item.CreditDetailCodeSecond,
                    CreditWarehouseCode = item.CreditWarehouse,
                    Quantity = item.Quantity,
                    Amount = item.RevenueAmmountPrice,
                    OrginalBookDate = ledger.OrginalBookDate,
                    DebitWarehouseCode = ledger.DebitWarehouse,
                    UnitPrice = item.RevenueUnitPrice
                };
                listXuatKho.Add(xuatKho);
            }
        }

        lstCostOfGoods = lstCostOfGoods.OrderBy(t => t.OrginalBookDate).ToList();

        return lstCostOfGoods;
    }

    public async Task<double> TinhGiaXuatKho(string code, string detail1, string detail2, string wareHouseCode, DateTime toDate,
        int year, int iMethodCalcExportPrice = 3, bool isInternal = false, List<SoChiTietViewModel> listXuatKho = null)
    {

        var _param = new LedgerReportParamDetail
        {
            AccountCode = code,
            AccountCodeDetail1 = detail1,
            AccountCodeDetail2 = detail2,
            


            FromDate = new DateTime(toDate.Year, 1, 1, 0, 0, 0, DateTimeKind.Local),
            ToDate = toDate,
            FilterType = 2,            
            IsNoiBo = isInternal
        };


        var veTrai = _context.ConvertProducts.Where(x => x.Account == _param.AccountCode
        && x.Detail1 == _param.AccountCodeDetail1
        && x.Detail2 == _param.AccountCodeDetail2)
            .FirstOrDefault();

        string filterAccount = _param.AccountCode;
        string filterDetail1 = _param.AccountCodeDetail1;
        string filterDetail2 = _param.AccountCodeDetail2;

        if (veTrai != null)
        {
            filterAccount = veTrai.OppositeAccount;
            filterDetail1 = veTrai.OppositeDetail1;
            filterDetail2 = veTrai.OppositeDetail2;
        }

        var allConvertionItems = _context.ConvertProducts.Where(x => x.OppositeAccount == filterAccount
              && x.OppositeDetail1 == filterDetail1
              && x.OppositeDetail2 == filterDetail2)
                  .ToList();

        List<LedgerReportParamDetail> itemParams = new List<LedgerReportParamDetail>();
        if (veTrai != null)
        {
            itemParams.Add(new LedgerReportParamDetail()
            {
                AccountCode = veTrai.OppositeAccount,
                AccountCodeDetail1 = veTrai.OppositeDetail1,
                AccountCodeDetail2 = veTrai.OppositeDetail2,
                FromDate = _param.FromDate,
                ToDate = toDate,
                FilterType = 2,
                IsNoiBo = isInternal,
                Quantity = 1,
                ConvertQuantity = 1
            });
        }

        foreach (var item in allConvertionItems)
        {
            itemParams.Add(new LedgerReportParamDetail()
            {
                AccountCode = item.Account,
                AccountCodeDetail1 = item.Detail1,
                AccountCodeDetail2 = item.Detail2,
                FromDate = _param.FromDate,
                ToDate = toDate,
                FilterType = 2,
                IsNoiBo = isInternal,
                Quantity = item.Quantity,
                ConvertQuantity = item.ConvertQuantity
            });
        }

        double soLuongTheoItem = 0;
        double tongTienTheoItem = 0;
        
        foreach (var prItem in itemParams)
        {
            var data = await GetDataReport_SoChiTiet_Six_data(prItem, year, wareHouseCode, listXuatKho);

            if (data.Count > 0)
            {
                var dataResult = data.FirstOrDefault();
                double SoLuong = 0;
                double amountTotal = 0;
                if (iMethodCalcExportPrice == 3 || iMethodCalcExportPrice == 1)
                {
                    List<Ledger> ledgers = await _context.GetLedger(year, iMethodCalcExportPrice).AsNoTracking().Where(x => x.OrginalBookDate <= toDate
                    && x.DebitCode == prItem.AccountCode && x.DebitDetailCodeFirst == prItem.AccountCodeDetail1
                    && (string.IsNullOrEmpty(detail2) || x.DebitDetailCodeSecond == prItem.AccountCodeDetail2)
                    && (string.IsNullOrEmpty(wareHouseCode) || x.DebitWarehouse == wareHouseCode)).ToListAsync();
                    SoLuong = ledgers.Sum(x => x.Quantity) + (dataResult?.OpenQuantity ?? 0);
                    amountTotal = ledgers.Sum(x => x.PercentImportTax > 0 || x.AmountTransport > 0
                                            ? x.UnitPrice * x.Quantity + x.UnitPrice * x.Quantity * (x.PercentImportTax ?? 0) + (x.AmountTransport ?? 0)
                                            : x.Amount
                                            ) + (dataResult?.OpenAmount ?? 0);
                    if (ledgers.Count == 0)
                    {
                        SoLuong = (dataResult?.OpenQuantity ?? 0) + (dataResult?.InputQuantity ?? 0) - (dataResult?.OutputQuantity ?? 0);
                        amountTotal = (dataResult?.OpenAmount ?? 0) + (dataResult?.InputAmount ?? 0) - (dataResult?.OutputAmount ?? 0);
                    }
                    soLuongTheoItem = soLuongTheoItem + SoLuong * prItem.ConvertQuantity;
                    tongTienTheoItem = tongTienTheoItem + amountTotal;
                }
            }
        }

        if (soLuongTheoItem == 0)
            return 1;

        if (veTrai != null)
        {
            return (tongTienTheoItem / soLuongTheoItem) * veTrai.ConvertQuantity;
        }
        else
        {
            return (tongTienTheoItem / soLuongTheoItem);
        }

            // SAI

            var aParentRef = "";
        var aCode = "";
        if (_param.AccountCodeDetail2 == "")
        {
            aParentRef = _param.AccountCode;
            aCode = _param.AccountCodeDetail1;
        }
        else
        {
            aParentRef = _param.AccountCode + ":" + _param.AccountCodeDetail1;
            aCode = _param.AccountCodeDetail2;
        }
        var ret = 0d;
        if (_param.IsNoiBo == true)
        {
            ret = _context.ChartOfAccounts.Where(x => x.Code == aCode && x.ParentRef == aParentRef).FirstOrDefault()?.StockUnitPriceNB ?? 0;
        }
        else
        {
            ret = _context.ChartOfAccounts.Where(x => x.Code == aCode && x.ParentRef == aParentRef).FirstOrDefault()?.StockUnitPrice ?? 0;
        }   
       
        return (double)ret;
    }

    public async Task<List<LedgerReportTonSLViewModel>> GetDataReport_SoChiTiet_Six_data(LedgerReportParamDetail _param,
        int year, string wareHouseCode = "", List<SoChiTietViewModel> listXuatKho = null)
    {
        try
        {
            DateTime dtFrom, dtTo;

            if (string.IsNullOrEmpty(_param.AccountCodeDetail1)) _param.AccountCodeDetail1 = string.Empty;
            _param.AccountCodeDetail1 = _param.AccountCodeDetail1.Trim();
            if (string.IsNullOrEmpty(_param.AccountCodeDetail2)) _param.AccountCodeDetail2 = string.Empty;
            _param.AccountCodeDetail2 = _param.AccountCodeDetail2.Trim();

            if (_param.FilterType == 1)
            {
                dtFrom = new DateTime(year, _param.FromMonth.Value, 1, 0, 0, 0, DateTimeKind.Local);
                dtTo = new DateTime(year, _param.ToMonth.Value, 1, 0, 0, 0, DateTimeKind.Local);
                dtTo = dtTo.AddMonths(1);
                _param.FromDate = dtFrom;
                _param.ToDate = dtTo;
            }
            else
            {
                dtFrom = new DateTime(_param.FromDate.Value.Year, _param.FromDate.Value.Month, _param.FromDate.Value.Day, 0, 0, 0, DateTimeKind.Local);
                dtTo = new DateTime(_param.ToDate.Value.Year, _param.ToDate.Value.Month, _param.ToDate.Value.Day, 0, 0, 0, DateTimeKind.Local).AddDays(1);
            }

            if (string.IsNullOrEmpty(_param.AccountCode))
            {
                return new List<LedgerReportTonSLViewModel>();
            }
            List<SoChiTietViewModel> relations = await _context.GetLedgerNotForYear(_param.IsNoiBo ? 3 : 2).AsNoTracking()
                    .Where(x => x.IsInternal != LedgerInternalConst.LedgerTemporary && x.OrginalBookDate.Value >= dtFrom && x.OrginalBookDate.Value < dtTo)
                    .Where(x =>
                        (string.IsNullOrEmpty(_param.AccountCode) || x.DebitCode == _param.AccountCode)
                        && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || x.DebitDetailCodeFirst == _param.AccountCodeDetail1)
                        && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || x.DebitDetailCodeSecond == _param.AccountCodeDetail2)
                    ||
                    (string.IsNullOrEmpty(_param.AccountCode) || x.CreditCode == _param.AccountCode)
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || x.CreditDetailCodeFirst == _param.AccountCodeDetail1)
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || x.CreditDetailCodeSecond == _param.AccountCodeDetail2)

                    && (string.IsNullOrEmpty(_param.AccountCodeReciprocal) || x.DebitCode == _param.AccountCodeReciprocal || x.CreditCode == _param.AccountCodeReciprocal)
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail1Reciprocal) || x.DebitDetailCodeFirst == _param.AccountCodeDetail1Reciprocal || x.CreditDetailCodeFirst == _param.AccountCodeDetail1Reciprocal)
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail2Reciprocal) || x.DebitDetailCodeSecond == _param.AccountCodeDetail2Reciprocal || x.CreditDetailCodeSecond == _param.AccountCodeDetail2Reciprocal)
                    )
                    .Select(k => new SoChiTietViewModel
                    {
                        DebitCode = k.DebitCode,
                        DebitDetailCodeSecond = k.DebitDetailCodeSecond,
                        DebitDetailCodeFirst = k.DebitDetailCodeFirst,
                        CreditCode = k.CreditCode,
                        CreditDetailCodeFirst = k.CreditDetailCodeFirst,
                        CreditDetailCodeSecond = k.CreditDetailCodeSecond,
                        CreditWarehouseCode = k.CreditWarehouse,
                        DebitWarehouseCode = k.DebitWarehouse,
                        NameGood = string.IsNullOrEmpty(k.DebitDetailCodeSecondName) ? k.DebitDetailCodeFirstName : k.DebitDetailCodeSecondName,
                        Quantity = k.Quantity,
                        Amount = k.PercentImportTax > 0 || k.AmountTransport > 0
                                                ? k.UnitPrice * k.Quantity + k.UnitPrice * k.Quantity * (k.PercentImportTax ?? 0) + (k.AmountTransport ?? 0)
                                                : k.Amount,
                        OrginalBookDate = k.OrginalBookDate
                    })
                    .OrderBy(x => x.DebitCode)
                    .ToListAsync();
            if (listXuatKho != null)
            {
                relations.AddRange(listXuatKho);
            }
            List<ChartOfAccount> listAccount = await _context.GetChartOfAccount(year)
                .Where(x => x.ParentRef.Contains(_param.AccountCode)).ToListAsync();
            if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
            {
                var parentRef = $"{_param.AccountCode}:{_param.AccountCodeDetail1}";
                listAccount = listAccount.Where(x => x.Code == _param.AccountCodeDetail2 && x.ParentRef == parentRef).ToList();
            }
            else if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
            {
                var parentRef = $"{_param.AccountCode}";
                listAccount = listAccount.Where(x => x.ParentRef == parentRef).ToList();
            }
            else if (!string.IsNullOrEmpty(wareHouseCode))
                listAccount = listAccount.Where(x => x.WarehouseCode == wareHouseCode).ToList();
            var listLedger = _context.GetLedger(year, 2)
                .Where(y => y.OrginalBookDate < dtFrom && (y.CreditCode == _param.AccountCode || y.DebitCode == _param.AccountCode));
            List<LedgerReportTonSLViewModel> relationReturn = new List<LedgerReportTonSLViewModel>();
            foreach (var account in listAccount)
            {
                if (_param.FromDate.Value.Year != year)
                {
                    var accountOld = await _context.ChartOfAccounts.FirstOrDefaultAsync(x => x.Code == account.Code && x.ParentRef == account.ParentRef
                                    && (string.IsNullOrEmpty(account.WarehouseCode) || x.WarehouseCode == account.WarehouseCode) && x.Year == _param.FromDate.Value.Year);
                    account.OpeningDebit = accountOld?.OpeningDebit;
                    account.OpeningCredit = accountOld?.OpeningCredit;
                    account.OpeningStockQuantity = accountOld?.OpeningStockQuantity;

                    account.OpeningDebitNB = accountOld?.OpeningDebitNB;
                    account.OpeningCreditNB = accountOld?.OpeningCreditNB;
                    account.OpeningStockQuantityNB = accountOld?.OpeningStockQuantityNB;
                }
                if (_param.IsNoiBo)
                {
                    account.OpeningDebit = account.OpeningDebitNB;
                    account.OpeningCredit = account.OpeningCreditNB;
                    account.OpeningStockQuantity = account.OpeningStockQuantityNB;
                }
                string detail1 = account.Code;
                string detail2 = "";
                if (account.ParentRef.Contains(":"))
                {
                    detail2 = account.Code;
                    detail1 = account.ParentRef.Split(':')[1];
                }
                LedgerReportTonSLViewModel itemOutCredit = new LedgerReportTonSLViewModel();
                itemOutCredit.Account = _param.AccountCode;
                itemOutCredit.Detail1 = detail1;
                itemOutCredit.Detail2 = string.IsNullOrEmpty(detail2) ? null : detail2;
                itemOutCredit.Warehouse = account.WarehouseCode;
                itemOutCredit.NameGood = account.Name;

                if (dtFrom.Month == 1 && dtFrom.Day == 1)
                {
                    itemOutCredit.OpenQuantity = account.OpeningStockQuantity ?? 0;
                    itemOutCredit.OpenAmount = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0);
                }
                else
                {
                    var dauKyCo = await listLedger.Where(y => y.CreditCode == _param.AccountCode && y.CreditDetailCodeFirst == detail1
                                                && (string.IsNullOrEmpty(detail2) || y.CreditDetailCodeSecond == detail2)
                                                && (string.IsNullOrEmpty(account.WarehouseCode) || y.CreditWarehouse == account.WarehouseCode)).ToListAsync();
                    double _dauKyCo_SoLuong = dauKyCo.Sum(q => q.Quantity);
                    double _dauKyCo_Amount = dauKyCo.Sum(q => q.Amount);
                    var dauKyNo = await listLedger.Where(y => y.DebitCode == _param.AccountCode && y.DebitDetailCodeFirst == detail1
                                                && (string.IsNullOrEmpty(detail2) || y.DebitDetailCodeSecond == detail2)
                                                && (string.IsNullOrEmpty(account.WarehouseCode) || y.DebitWarehouse == account.WarehouseCode)).ToListAsync();
                    double _dauKyNo_SoLuong = dauKyNo.Sum(q => q.Quantity);
                    double _dauKyNo_Amount = dauKyNo.Sum(q => q.Amount);
                    itemOutCredit.OpenQuantity = (account.OpeningStockQuantity ?? 0) + _dauKyNo_SoLuong - _dauKyCo_SoLuong;
                    itemOutCredit.OpenAmount = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0) + _dauKyNo_Amount - _dauKyCo_Amount;
                }

                List<SoChiTietViewModel> relationCredit = relations.Where(x => x.CreditCode == _param.AccountCode && x.CreditDetailCodeFirst == detail1
                   && (string.IsNullOrEmpty(detail2) || x.CreditDetailCodeSecond == detail2) && (string.IsNullOrEmpty(account.WarehouseCode) || x.CreditWarehouseCode == account.WarehouseCode)).ToList();
                itemOutCredit.OutputQuantity = relationCredit.Sum(x => x.Quantity);
                itemOutCredit.OutputAmount = relationCredit.Sum(x => x.Amount);

                List<SoChiTietViewModel> relationDebit = relations.Where(x => x.DebitCode == _param.AccountCode && x.DebitDetailCodeFirst == detail1
                    && (string.IsNullOrEmpty(detail2) || x.DebitDetailCodeSecond == detail2) && (string.IsNullOrEmpty(account.WarehouseCode) || x.DebitWarehouseCode == account.WarehouseCode)).ToList();
                if (relationDebit.Count > 0)
                {
                    itemOutCredit.InputQuantity = relationDebit.Sum(x => x.Quantity);
                    itemOutCredit.InputAmount = relationDebit.Sum(x => x.Amount);
                }
                if (itemOutCredit.OpenQuantity != 0 || itemOutCredit.InputQuantity != 0 || itemOutCredit.OutputQuantity != 0)
                    relationReturn.Add(itemOutCredit);
            }

            return relationReturn;
        }
        catch
        {
            return new List<LedgerReportTonSLViewModel>();
        }
    }

    public async Task<string> CreateCostOfGoods(List<LedgerCostOfGoodsModel> entites, int isInternal, int year)
    {
        foreach (LedgerCostOfGoodsModel entity in entites)
        {
            Ledger ledger = _mapper.Map<Ledger>(entity);
            ledger.Id = 0;
            ledger.BookDate = DateTime.Today;
            ledger.IsInternal = isInternal == 3 ? 3 : 1;
            ledger.UnitPrice = entity.RevenueUnitPrice;
            ledger.Amount = entity.RevenueAmmountPrice;
            ledger.ReferenceBookDate = DateTime.Today;
            await Create(ledger, year);
        }

        return string.Empty;
    }

    public async Task<string> EditOrder(EditOrderRequestModel request, int year)
    {
        StringBuilder message = new StringBuilder();
        var ledgerUpdateOrginalVoucher = new Ledger();
        var isUpdateOrginalVoucher = false;
        if (request.OrderType == 1)// Giảm
        {
            for (int i = request.EditOrderStart; i <= request.EditOrderEnd; i++)
            {
                try
                {
                    var ledger = await _context.GetLedger(year, request.IsInternal)
                        .Where(x => x.Order == i).ToListAsync();
                    if (ledger.Any())
                    {
                        foreach (var item in ledger)
                        {
                            item.Order -= request.EditValue;

                            var orderString = LedgerHelper.GetOriginalVoucher(item.Order);
                            item.OrginalVoucherNumber = $"{item.Type}{item.Month}-{year.ToString().Substring(2, 2)}-{orderString}";

                            _context.Ledgers.Update(item);
                        }
                        await _context.SaveChangesAsync();
                    }
                    if (!isUpdateOrginalVoucher && ledger.Count > 0)
                    {
                        isUpdateOrginalVoucher = true;
                        ledgerUpdateOrginalVoucher = ledger.FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    message.Append(e.Message);
                }
            }
        }
        else if (request.OrderType == 2)// Tăng
        {
            for (int i = request.EditOrderEnd; i >= request.EditOrderStart; i--)
            {
                try
                {
                    var ledger = await _context.GetLedger(year, request.IsInternal).Where(x => x.Order == i).ToListAsync();
                    if (ledger.Any())
                    {
                        foreach (var item in ledger)
                        {
                            item.Order += request.EditValue;

                            var orderString = LedgerHelper.GetOriginalVoucher(item.Order);
                            item.OrginalVoucherNumber = $"{item.Type}{item.Month}-{year.ToString().Substring(2, 2)}-{orderString}";

                            _context.Ledgers.Update(item);
                        }
                        await _context.SaveChangesAsync();
                    }
                    if (!isUpdateOrginalVoucher && ledger.Count > 0)
                    {
                        isUpdateOrginalVoucher = true;
                        ledgerUpdateOrginalVoucher = ledger.FirstOrDefault();
                    }
                }
                catch (Exception e)
                {
                    message.Append(e.Message);
                }
            }
        }
        return message.ToString();
    }

    public async Task<string> GetDataReport(LedgerReportParam _param, int year, bool isNoiBo = false)
    {
        try
        {
            if (_param.FromMonth == null) _param.FromMonth = 0;
            if (_param.ToMonth == null) _param.ToMonth = 0;
            if (_param.FromDate == null) _param.FromDate = new DateTime();
            if (_param.ToDate == null) _param.ToDate = new DateTime();
            if (string.IsNullOrEmpty(_param.AccountCode)) _param.AccountCode = string.Empty;
            var company = await _companyService.GetCompany();
            var _account = await _context.GetChartOfAccount(year).Where(x => x.Code == _param.AccountCode).FirstOrDefaultAsync();

            DateTime dtFrom = _param.FromDate.Value, dtTo = _param.ToDate.Value;
            if (_param.FromMonth > 0 && _param.ToMonth > 0)
            {
                dtFrom = new DateTime(DateTime.UtcNow.Year, _param.FromMonth.Value, 1, 0, 0, 0, DateTimeKind.Local);
                dtTo = new DateTime(DateTime.UtcNow.Year, _param.ToMonth.Value, 1, 0, 0, 0, DateTimeKind.Local);
            }
            List<LedgerReportNBViewModel> _LedgerDetail = await _context.GetLedger(year, isNoiBo ? 3 : 2)
                .Where(x => x.DebitCode.StartsWith(_account.Code) || x.CreditCode.StartsWith(_account.Code))
                .Select(x => new LedgerReportNBViewModel
                {
                    Type = x.Type,
                    Month = x.Month,
                    Year = x.OrginalBookDate.Value.Year,
                    VoucherNumber = x.VoucherNumber,
                    DebitCode = x.DebitCode,
                    CreditCode = x.CreditCode,
                    CreditCodeParent = !string.IsNullOrEmpty(x.CreditCode) ? x.CreditCode.Substring(0, 3) : string.Empty,
                    DebitCodeParent = !string.IsNullOrEmpty(x.DebitCode) ? x.DebitCode.Substring(0, 3) : string.Empty,
                    OrginalDescription = x.OrginalDescription,
                    Amount = x.Amount,
                    IsDebit = x.DebitCode.StartsWith(_account.Code),
                    AmountDebit = x.DebitCode.StartsWith(_account.Code) ? x.Amount : 0,
                    AmountCredit = !x.DebitCode.StartsWith(_account.Code) ? x.Amount : 0,
                    BookDate = x.BookDate,
                    OrginalBookDate = x.OrginalBookDate
                }).ToListAsync();

            List<LedgerReportViewModel> _filterLedger = _LedgerDetail.GroupBy(x => new { x.Type, x.Month, x.Year, x.DebitCodeParent, x.DebitCode, x.CreditCodeParent, x.IsDebit })
                .Select(x => new LedgerReportViewModel
                {
                    BookDate = x.Max(y => y.BookDate),
                    OrginalBookDate = x.Max(y => y.OrginalBookDate),
                    DebitCode = x.Key.DebitCode,
                    CreditCode = string.Join(", ", x.Select(y => y.CreditCode).ToList()),
                    CreditCodeParent = x.Key.CreditCodeParent,
                    DebitCodeParent = !string.IsNullOrEmpty(x.Key.DebitCode) ? x.Key.DebitCode.Substring(0, 3) : string.Empty,
                    TakeNote = string.Empty,
                    IsDebit = x.Key.IsDebit,
                    VoucherNumber = x.Where(q => q.Type == x.Key.Type && q.Month == x.Key.Month && q.Year == x.Key.Year && q.CreditCodeParent == x.Key.CreditCodeParent).Select(y => y.VoucherNumber).Distinct().First(),
                    Description = string.Join(", ", x.Select(y => y.OrginalDescription).ToList()),
                    DebitAmount = x.Sum(y => y.AmountDebit),
                    CreditAmount = x.Sum(y => y.AmountCredit),
                    ReciprocalCode = x.Key.IsDebit ? x.Key.CreditCodeParent : x.Key.DebitCodeParent,
                    Month = x.Key.Month,
                    Year = x.Key.Year
                }).ToList();

            List<LedgerReportViewModel> _filterLedger_GroupBy_TKDX = _filterLedger.GroupBy(x => new { x.ReciprocalCode, x.Month, x.Year })
                .Select(x => new LedgerReportViewModel
                {
                    BookDate = x.Max(y => y.BookDate),
                    OrginalBookDate = x.Max(y => y.OrginalBookDate),
                    DebitCode = x.FirstOrDefault()?.DebitCode,
                    CreditCode = string.Join(", ", x.Select(y => y.CreditCode).ToList()),
                    CreditCodeParent = x.FirstOrDefault()?.CreditCodeParent,
                    DebitCodeParent = x.FirstOrDefault()?.DebitCodeParent,
                    TakeNote = string.Empty,
                    IsDebit = x.FirstOrDefault()?.IsDebit ?? false,
                    VoucherNumber = x.Select(y => y.VoucherNumber).Distinct().First(),
                    Description = string.Join(", ", x.Select(y => y.Description).ToList()),
                    DebitAmount = x.Sum(y => y.DebitAmount),
                    CreditAmount = x.Sum(y => y.CreditAmount),
                    ReciprocalCode = x.Key.ReciprocalCode,
                    Month = x.Key.Month,
                    Year = x.Key.Year
                }).ToList();

            List<LedgerReportViewModel> relations;
            List<LedgerReportModel> _lstModelReport = new List<LedgerReportModel>();
            LedgerReportModel _modelExcel = new LedgerReportModel();
            var _accountGet = await _accountBalanceSheet.
                GenerateAccrualAccounting("date", dtFrom, dtTo, _param.AccountCode, string.Empty, string.Empty, isNoiBo);

            if (_param.FileType == "excel")
            {
                relations = _filterLedger_GroupBy_TKDX;

                LedgerReportSumRow _sumRow = new LedgerReportSumRow();
                _sumRow.SumDebit = relations.Sum(x => x.DebitAmount);
                _sumRow.SumCredit = relations.Sum(x => x.CreditAmount);

                LedgerReportCalculator _cal = CalculatorFollowMonth(_param, _filterLedger_GroupBy_TKDX, isNoiBo, year);
                _sumRow.SumDebitLuyKe = _sumRow.SumDebit + _cal.OpeningDebit;
                _sumRow.SumCreditLuyKe = _sumRow.SumCredit + _cal.OpeningCredit;

                //dòng cuối, nợ gì - có gì ???
                _sumRow.isDebit = _cal.ClosingBacklog > 0;
                _sumRow.SumDebitDuCT = _sumRow.isDebit ? _cal.ClosingBacklog : 0;
                _sumRow.SumCreditDuCT = !_sumRow.isDebit ? _cal.ClosingBacklog : 0;

                _modelExcel = new LedgerReportModel
                {
                    InfoSum = _sumRow,
                    Items = relations,
                    LedgerCalculator = _cal,
                    Address = company.Address,
                    Company = company.Name,
                    MethodCalcExportPrice = company.MethodCalcExportPrice,
                    TaxId = company.MST,
                    CEOName = company.NameOfCEO,
                    ChiefAccountantName = company.NameOfChiefAccountant,

                    CEONote = company.NoteOfCEO,
                    ChiefAccountantNote = company.NoteOfChiefAccountant,
                    AccountCode = _account?.Code,
                    AccountName = _account?.Name
                };
            }
            else
            {
                List<LedgerReportViewModel> listZ = _filterLedger_GroupBy_TKDX.Where(x => x.OrginalBookDate.Value.Month >= dtFrom.Month && x.OrginalBookDate.Value.Month <= dtTo.Month && x.OrginalBookDate.Value.Year == dtFrom.Year).ToList();
                List<LedgerReportViewModel> _eachMonth = listZ;

                LedgerReportSumRow _sumRow = new LedgerReportSumRow();
                _sumRow.SumDebit = _eachMonth.Sum(x => x.DebitAmount);
                _sumRow.SumCredit = _eachMonth.Sum(x => x.CreditAmount);

                LedgerReportCalculator _cal = CalculatorFollowMonth(_param, _eachMonth, isNoiBo, year);
                _sumRow.SumDebitLuyKe = _sumRow.SumDebit + _cal.OpeningDebit;
                _sumRow.SumCreditLuyKe = _sumRow.SumCredit + _cal.OpeningCredit;

                //dòng cuối, nợ gì - có gì ???
                _sumRow.isDebit = _cal.ClosingBacklog > 0;
                _sumRow.SumDebitDuCT = _sumRow.isDebit ? _cal.ClosingBacklog : 0;
                _sumRow.SumCreditDuCT = !_sumRow.isDebit ? _cal.ClosingBacklog : 0;

                LedgerReportModel _model = new LedgerReportModel
                {
                    InfoSum = _sumRow,
                    Items = _eachMonth,
                    LedgerCalculator = _cal,
                    Address = company.Address,
                    Company = company.Name,
                    MethodCalcExportPrice = company.MethodCalcExportPrice,
                    TaxId = company.MST,
                    CEOName = company.NameOfCEO,
                    ChiefAccountantName = company.NameOfChiefAccountant,
                    CEONote = company.NoteOfCEO,
                    ChiefAccountantNote = company.NoteOfChiefAccountant,
                    AccountCode = _account?.Code,
                    AccountName = _account?.Name
                };

                _lstModelReport.Add(_model);
            }

            return ExportDataTransactionList(_modelExcel, _param, _lstModelReport, _accountGet, year);
        }
        catch
        {
            return string.Empty;
        }
    }

    private LedgerReportCalculator CalculatorFollowMonth(LedgerReportParam _param, List<LedgerReportViewModel> _listData, bool isNoiBo, int year)
    {
        LedgerReportCalculator p = new LedgerReportCalculator();
        try
        {
            DateTime _fromDt, _toDt;
            _fromDt = _param.FromMonth > 0 ? new DateTime(DateTime.Now.Year, (int)_param.FromMonth, 1, 0, 0, 0, DateTimeKind.Local) : (DateTime)_param.FromDate;
            _toDt = _param.ToMonth > 0 ? new DateTime(DateTime.Now.Year, (int)_param.ToMonth, 1, 0, 0, 0, DateTimeKind.Local) : (DateTime)_param.ToDate;

            p.OpeningDebit = _context.GetChartOfAccount(year).Where(x => x.Code.Equals(_param.AccountCode))
                   .FirstOrDefault()?.OpeningDebit ?? 0;
            p.OpeningCredit = _context.GetChartOfAccount(year).Where(x => x.Code.Equals(_param.AccountCode))
               .FirstOrDefault()?.OpeningCredit ?? 0;

            if (_listData.Count > 0 && _listData[0].Month > 1)
            {
                p.OpeningDebit += _context.GetLedger(year, isNoiBo ? 3 : 2)
                    .Where(x => x.DebitCode.StartsWith(_param.AccountCode) && x.OrginalBookDate.Value < new DateTime(_listData[0].Year, _listData[0].Month, 1, 0, 0, 0, DateTimeKind.Local))
                    .Sum(x => x.Amount);

                p.OpeningCredit += _context.GetLedger(year, isNoiBo ? 3 : 2)
                    .Where(x => x.CreditCode.StartsWith(_param.AccountCode) && x.OrginalBookDate.Value < new DateTime(_listData[0].Year, _listData[0].Month, 1, 0, 0, 0, DateTimeKind.Local))
                    .Sum(x => x.Amount);
            }

            p.OpeningBacklog = p.OpeningDebit - p.OpeningCredit;

            p.ArisingDebit = _listData.Sum(x => x.DebitAmount);
            p.ArisingCredit = _listData.Sum(x => x.CreditAmount);
            p.ArisingBacklog = p.ArisingDebit - p.ArisingCredit;

            p.ClosingDebit = 0;
            p.ClosingCredit = 0;
            p.ClosingBacklog = p.OpeningBacklog + p.ArisingBacklog;

            return p;
        }
        catch
        {
            return null;
        }
    }

    private string ExportDataTransactionList(LedgerReportModel ledgers, LedgerReportParam param, List<LedgerReportModel> _lstModelReport, AccrualAccountingViewModel _sumAmount, int year)
    {
        try
        {
            string _path = string.Empty;
            switch (param.FileType)
            {
                case "html":
                    _path = ConvertToHTML_Ledger_V2(ledgers, _lstModelReport, param, _sumAmount, year);
                    break;

                case "excel":
                    _path = ExportExcel_Report_Ledger(ledgers, param, year);
                    break;

                case "pdf":
                    _path = ConvertToPDFFile_Ledger(ledgers, _lstModelReport, param, _sumAmount, year);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_Ledger_V2(LedgerReportModel p, List<LedgerReportModel> _lstModelReport, LedgerReportParam param,
        AccrualAccountingViewModel _sumAmount, int year)
    {
        try
        {
            if (p == null) return string.Empty;
            if (param.FromMonth == null) param.FromMonth = 0;
            if (param.ToMonth == null) param.ToMonth = 0;
            if (param.FromDate == null) param.FromDate = DateTime.Now;
            if (param.ToDate == null) param.ToDate = DateTime.Now;

            string _template = "SoCaiTotalTemplate.html",
               _folderPath = @"Uploads\Html",
               path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
               _allText = File.ReadAllText(path), resultHTML = string.Empty;

            _lstModelReport.ForEach(x =>
            {
                resultHTML += ConvertToHTML_Ledger_EachItem(x, param, _sumAmount, year);
            });

            _allText = _allText.Replace("##REPLACE_PLACE_BODY##", resultHTML);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_Ledger_EachItem(LedgerReportModel p, LedgerReportParam param, AccrualAccountingViewModel _sumAmount, int year)
    {
        try
        {
            if (p == null) return string.Empty;
            if (param.FromMonth == null) param.FromMonth = 0;
            if (param.ToMonth == null) param.ToMonth = 0;
            if (param.FromDate == null) param.FromDate = DateTime.Now;
            if (param.ToDate == null) param.ToDate = DateTime.Now;

            StringBuilder resultHTML = new StringBuilder();
            string _template = "SoCaiEachTemplate.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = File.ReadAllText(path),
                _month = param.FromMonth > 0 && param.ToMonth > 0 ? ((int)param.FromMonth).ToString("D2") + "/" + DateTime.Now.Year.ToString() : ((DateTime)param.FromDate).ToString("MM/yyyy");
            _month = p.Items.Count > 0 ? p.Items[0].OrginalBookDate.Value.ToString("MM/yyyy") : string.Empty;
            List<int> listMonth = p.Items.Select(x => x.Month).Distinct().ToList();

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanCT", p.AccountCode+" - "+p.AccountName },
                { "TuThang", (param.FromMonth > 0 && param.ToMonth > 0 ? (int)param.FromMonth : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                { "DenThang",(param.FromMonth > 0 && param.ToMonth > 0 ? (int)param.ToMonth : ((DateTime)param.ToDate).Month ).ToString("D2") },
                { "Nam", (param.FromMonth > 0 && param.ToMonth > 0 ?  year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", " ..... " },
                { "Thang", " ..... " },
                { "NamSign", " ..... " },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },

                { "KeToanTruong_CV", p.ChiefAccountantNote},
                { "GiamDoc_CV", p.CEONote},

                { "LUY_KE_DAU_NAM",  string.Format("{0:N0}", _sumAmount.IncurExpenses.FirstOrDefault()?.OpeningStock ?? 0) },
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            string soThapPhan = "N" + p.MethodCalcExportPrice;

            foreach (var month in listMonth)
            {
                string _thang = "T" + month;

                IncurExpense _thangTong = _sumAmount.IncurExpenses.Find(x => x.Name == _thang) ?? new IncurExpense();

                string _txtLK = @"<tr class='font-b'>
                                                                <td colspan='4'>Lũy kế đầu năm</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{LUY_KE_DAU_NAM_NO}}</td>
                                                                <td class='txt-right'>{{LUY_KE_DAU_NAM_CO}}</td>
                                                            </tr>";
                _txtLK = _txtLK.Replace("{{LUY_KE_DAU_NAM_NO}}", _thangTong.OpeningStock > 0 ? string.Format("{0:N0}", Math.Abs(_thangTong.OpeningStock)) : string.Empty)
                                   .Replace("{{LUY_KE_DAU_NAM_CO}}", _thangTong.OpeningStock < 0 ? string.Format("{0:N0}", _thangTong.OpeningStock) : string.Empty);

                resultHTML.Append(_txtLK);

                if (p.Items.Count > 0)
                {
                    List<LedgerReportViewModel> listItem = p.Items.Where(x => x.Month == month).ToList();
                    listItem.ForEach(x =>
                    {
                        string _txt = @"<tr>
                                            <td>{{{NGAY_GHI_SO}}}</td>
                                    <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>
                                            <td>{{{CHUNG_TU_NGAY}}}</td>
                                            <td>{{{DIEN_GIAI}}}</td>
                                            <td>{{{TRANG_NHAT_KY}}}</td>
                                            <td>{{{TK_DOI_UNG}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_NO}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_CO}}}</td>
                                        </tr>";

                        _txt = _txt.Replace("{{{NGAY_GHI_SO}}}", x.BookDate.HasValue ? x.BookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                        .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise)
                                            .Replace("{{{CHUNG_TU_SO}}}", x.VoucherNumber)
                                            .Replace("{{{CHUNG_TU_NGAY}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                            .Replace("{{{DIEN_GIAI}}}", StringHelpers.GetStringWithMaxLength(x.Description, 400))
                                            .Replace("{{{TRANG_NHAT_KY}}}", string.Empty)
                                            .Replace("{{{TK_DOI_UNG}}}", x.IsDebit ? x.CreditCodeParent : x.DebitCodeParent)
                                            .Replace("{{{SO_TIEN_NO}}}", x.IsDebit ? string.Format("{0:" + soThapPhan + "}", x.DebitAmount) : string.Empty)
                                            .Replace("{{{SO_TIEN_CO}}}", !x.IsDebit ? string.Format("{0:" + soThapPhan + "}", x.CreditAmount) : string.Empty)

                                            .Replace("{{{FILTER_TYPE}}}", x.VoucherNumber.Split('/')[1])
                                   .Replace("{{{FILTER_MONTH}}}", x.Month.ToString())
                                    .Replace("{{{FILTER_ISINTERNAL}}}", param.IsNoiBo ? "3" : "1")

                                   .Replace("{{{FILTER_TEXT}}}", x.IsDebit ? x.CreditCodeParent : x.DebitCodeParent)
                                            ;

                        resultHTML.Append(_txt);
                    });
                    string _tr_Sum = @"<tr class='font - b'>
                              <td colspan = '3' ></ td >
                              <td>{{{NOTE_CONG_PHAT_SINH}}}</td>
                              <td></td>
                              <td></td>
                              <td class='txt-right'>{{{TONG_SO_TIEN_NO}}}</td>
                              <td class='txt-right'>{{{TONG_SO_TIEN_CO}}}</td>
                            </tr>
                            <tr class='font-b'>
                                <td colspan = '3' ></td>
                                <td>{{{NOTE_LUY_KET_PS}}}</td>
                                <td></td>
                                <td></td>
                                <td class='txt-right'>{{{TONG_SO_TIEN_NO_PS}}}</td>
                                <td class='txt-right'>{{{TONG_SO_TIEN_CO_PS}}}</td>
                            </tr>
                            <tr class='font-b'>
                                <td colspan = '3' ></td>
                                <td>{{{NOTE_SO_DU_CUOI_THANG}}}</td>
                                <td></td>
                                <td></td>
                                <td class='txt-right'>{{{SO_DU_NO}}}</td>
                                <td class='txt-right'>{{{SO_DU_CO}}}</td>
                            </tr>   ";

                    _tr_Sum = _tr_Sum.Replace("{{{NOTE_CONG_PHAT_SINH}}}", "Cộng phát sinh tháng " + _month)
                        .Replace("{{{NOTE_LUY_KET_PS}}}", "Lũy kế phát sinh từ đầu năm")
                        .Replace("{{{NOTE_SO_DU_CUOI_THANG}}}", "Số dư cuối tháng " + _month)
                        .Replace("{{{TONG_SO_TIEN_NO}}}", string.Format("{0:" + soThapPhan + "}", _thangTong != null ? _thangTong.SumDebit : 0))
                        .Replace("{{{TONG_SO_TIEN_CO}}}", string.Format("{0:" + soThapPhan + "}", _thangTong != null ? _thangTong.SumCredit : 0))
                        .Replace("{{{TONG_SO_TIEN_NO_PS}}}", string.Format("{0:" + soThapPhan + "}", _thangTong != null ? _thangTong.AccumulatedDebit : 0))
                        .Replace("{{{TONG_SO_TIEN_CO_PS}}}", string.Format("{0:" + soThapPhan + "}", _thangTong != null ? _thangTong.AccumulatedCredit : 0))
                        .Replace("{{{SO_DU_NO}}}", string.Format("{0:" + soThapPhan + "}", _thangTong?.Balance > 0 ? Math.Abs(_thangTong.Balance) : 0))
                        .Replace("{{{SO_DU_CO}}}", string.Format("{0:" + soThapPhan + "}", _thangTong?.Balance < 0 ? Math.Abs(_thangTong.Balance) : 0));
                    resultHTML.Append(_tr_Sum);
                }
            }

            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML.ToString());

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report_Ledger(LedgerReportModel p, LedgerReportParam param, int year)
    {
        try
        {
            if (p == null) return string.Empty;
            if (param.FromMonth == null) param.FromMonth = 0;
            if (param.ToMonth == null) param.ToMonth = 0;
            if (param.FromDate == null) param.FromDate = DateTime.Now;
            if (param.ToDate == null) param.ToDate = DateTime.Now;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetName");

            //A => J
            worksheet.Cells["A1:H1"].Merge = true;
            worksheet.Cells["A2:H2"].Merge = true;
            worksheet.Cells["A3:H3"].Merge = true;
            worksheet.Cells["A1:H1"].Value = p.Company;
            worksheet.Cells["A2:H2"].Value = p.Address;
            worksheet.Cells["A3:H3"].Value = p.TaxId;

            worksheet.Cells["A5:H5"].Merge = true;

            worksheet.Cells["A5:H5"].Merge = true;
            worksheet.Cells["A5:H5"].Value = "SỔ CÁI";

            worksheet.Cells["A6:H6"].Merge = true;
            worksheet.Cells["A6:H6"].Value = "Từ tháng ... đến tháng ... năm ... ";

            worksheet.Cells["A7:H7"].Merge = true;
            worksheet.Cells["A7:H7"].Value = "Tài khoản: " + p.AccountCode + " - " + p.AccountName;

            worksheet.Cells["A8:C8"].Merge = true;
            worksheet.Cells["A8:C8"].Value = "Đơn vị tính: Đồng";

            worksheet.Cells["F8:H8"].Merge = true;
            worksheet.Cells["F8:H8"].Value = "Lũy kế đầu năm:";

            //table
            worksheet.Cells["A9:A10"].Merge = true;
            worksheet.Cells["A9:A10"].Value = "NGÀY GHI SỐ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["A9:A10"]);

            worksheet.Cells["B9:C9"].Merge = true;
            worksheet.Cells["B9:C9"].Value = "CHỨNG TỪ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["B9:C9"]);

            worksheet.Cells["B10"].Value = "SỐ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["B10"]);
            worksheet.Cells["C10"].Value = "NGÀY";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C10"]);

            worksheet.Cells["D9:D10"].Merge = true;
            worksheet.Cells["D9:D10"].Value = "DIỄN GIẢI";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D9:D10"]);

            worksheet.Cells["E9:E10"].Merge = true;
            worksheet.Cells["E9:E10"].Value = "TRANG NHẬT KÝ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E9:E10"]);

            worksheet.Cells["F9:F10"].Merge = true;
            worksheet.Cells["F9:F10"].Value = "TK ĐỐI ỨNG";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["F9:F10"]);

            worksheet.Cells["G9:H9"].Merge = true;
            worksheet.Cells["G9:H9"].Value = "SỐ PHÁT SINH";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G9:H9"]);

            worksheet.Cells["G10"].Value = "NỢ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G10"]);
            worksheet.Cells["H10"].Value = "CÓ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["H10"]);

            int currentRowNoBegin = 10;
            int currentRowNo = currentRowNoBegin, flagRowNo = 0;

            for (int i = 0; i < p.Items.Count; i++)
            {
                currentRowNo++;
                LedgerReportViewModel _model = p.Items[i];
                worksheet.Cells[currentRowNo, 1].Value = _model.BookDate.HasValue ? _model.BookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                worksheet.Cells[currentRowNo, 2].Value = _model.VoucherNumber;
                worksheet.Cells[currentRowNo, 3].Value = _model.OrginalBookDate.HasValue ? _model.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                worksheet.Cells[currentRowNo, 4].Value = _model.Description;
                worksheet.Cells[currentRowNo, 5].Value = string.Empty;
                worksheet.Cells[currentRowNo, 6].Value = _model.IsDebit ? _model.CreditCodeParent : _model.DebitCodeParent;
                worksheet.Cells[currentRowNo, 7].Value = _model.IsDebit ? _model.DebitAmount : 0;
                worksheet.Cells[currentRowNo, 8].Value = !_model.IsDebit ? _model.CreditAmount : 0;
            }

            flagRowNo = currentRowNo;

            string _month = string.Empty;
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company ?? "" },
                { "DiaChi", p.Address  ?? "" },
                { "MST", p.TaxId  ?? "" },
                { "NgayChungTu", string.Empty },
                { "TuThang", (param.FromMonth > 0 && param.ToMonth > 0 ? (int)param.FromMonth : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                { "DenThang", ( param.FromMonth > 0 && param.ToMonth > 0 ? (int)param.ToMonth : ((DateTime)param.ToDate).Month ).ToString("D2") },
                { "Nam", (param.FromMonth > 0 && param.ToMonth > 0 ? year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", " ..... " },
                { "Thang", " ..... " },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty  },
                { "KeToanTruong_CV", p.ChiefAccountantNote},
                { "GiamDoc_CV", p.CEONote},
                //last row sum
                { "NOTE_CONG_PHAT_SINH", "Cộng phát sinh tháng " +  _month},
                { "TONG_SO_TIEN_NO", p.InfoSum.SumDebit.ToString()},
                { "TONG_SO_TIEN_CO", p.InfoSum.SumCredit.ToString()},
                { "NOTE_LUY_KET_PS", "Lũy kế phát sinh từ đầu năm" },
                { "TONG_SO_TIEN_NO_PS", p.InfoSum.SumDebitLuyKe.ToString()},
                { "TONG_SO_TIEN_CO_PS", p.InfoSum.SumCreditLuyKe.ToString()},
                { "NOTE_SO_DU_CUOI_THANG", "Số dư cuối tháng "+_month },
                { "SO_DU_NO", p.InfoSum.isDebit ? p.InfoSum.SumDebitDuCT.ToString() : string.Empty },
                { "SO_DU_CO", !p.InfoSum.isDebit ? p.InfoSum.SumCreditDuCT.ToString() : string.Empty },
            };

            currentRowNo++;

            worksheet.Cells[currentRowNo, 4].Value = v_dicFixed["NOTE_CONG_PHAT_SINH"];
            worksheet.Cells[currentRowNo, 7].Value = p.InfoSum.SumDebit;
            worksheet.Cells[currentRowNo, 8].Value = p.InfoSum.SumCredit;

            currentRowNo++;
            worksheet.Cells[currentRowNo, 4].Value = v_dicFixed["NOTE_LUY_KET_PS"];
            worksheet.Cells[currentRowNo, 7].Value = p.InfoSum.SumDebitLuyKe;
            worksheet.Cells[currentRowNo, 8].Value = p.InfoSum.SumCreditLuyKe;

            currentRowNo++;
            worksheet.Cells[currentRowNo, 4].Value = v_dicFixed["NOTE_SO_DU_CUOI_THANG"];
            worksheet.Cells[currentRowNo, 7].Value = p.InfoSum.isDebit ? p.InfoSum.SumDebitDuCT : 0;
            worksheet.Cells[currentRowNo, 8].Value = !p.InfoSum.isDebit ? p.InfoSum.SumCreditDuCT : 0;

            worksheet.Cells[currentRowNoBegin, 7, currentRowNo, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

            currentRowNo++;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = "Ngày ... tháng ... năm";

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người ghi sổ";
            worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong_CV"];
            worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = v_dicFixed["GiamDoc_CV"];

            currentRowNo += 4;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = v_dicFixed["NguoiLap"];

            worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
            worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong"];

            worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = v_dicFixed["GiamDoc"];

            worksheet.Column(1).AutoFit(20);
            worksheet.Column(2).AutoFit(15);
            worksheet.Column(3).AutoFit(15);
            worksheet.Column(4).AutoFit(15);
            worksheet.Column(5).AutoFit(15);
            worksheet.Column(6).AutoFit(15);
            worksheet.Column(7).AutoFit(15);
            worksheet.Column(8).AutoFit(15);

            worksheet.SelectedRange["A1:H3"].Style.Font.Size = 12;
            worksheet.SelectedRange["A6:H8"].Style.Font.Size = 12;

            worksheet.SelectedRange["A5:H5"].Style.Font.Size = 16;
            worksheet.SelectedRange["A5:H5"].Style.Font.Bold = true;
            worksheet.SelectedRange["A7:H7"].Style.Font.Bold = true;
            worksheet.Row(5).Height = 30;

            worksheet.SelectedRange["A9:H10"].Style.Font.Bold = true;
            worksheet.SelectedRange["A9:H10"].Style.Font.Size = 14;

            worksheet.Cells["A9:H10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.SelectedRange["A9:H10"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 213, 180));
            worksheet.SelectedRange["A9:H10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["A11:H" + flagRowNo].Style.Font.Bold = false;
            worksheet.SelectedRange["A11:H" + currentRowNo].Style.Font.Size = 12;

            worksheet.SelectedRange["A1:H" + currentRowNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

            worksheet.SelectedRange["A4:H" + currentRowNo].Style.WrapText = true;

            worksheet.SelectedRange["A5:H8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["A11:F" + currentRowNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["G11:H" + (flagRowNo + 3)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            worksheet.SelectedRange[currentRowNo - 8, 1, currentRowNo, 8].Style.Font.Bold = true;
            worksheet.SelectedRange[currentRowNo - 5, 1, currentRowNo, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(4).Width = 50;

            return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SoCai");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToPDFFile_Ledger(LedgerReportModel p, List<LedgerReportModel> _lstModelReport, LedgerReportParam param, AccrualAccountingViewModel _sumAmount, int year)
    {
        try
        {
            string _allText = ConvertToHTML_Ledger_V2(p, _lstModelReport, param, _sumAmount, year);
            return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "SoCai");
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<double> DinhKhoanThue(LedgerRequestDinhKhoanThue request, int year)
    {
        request.OrginalBookDate = new DateTime(year, request.OrginalBookDate.Value.Month, request.OrginalBookDate.Value.Day, 0, 0, 0, DateTimeKind.Local);

        return await _context.GetLedger(year, request.IsInternal).Where(x =>
        x.OrginalBookDate == request.OrginalBookDate &&
          x.OrginalVoucherNumber == request.OrginalVoucherNumber
          && (string.IsNullOrEmpty(request.InvoiceNumber) || x.InvoiceNumber == request.InvoiceNumber)
          && (string.IsNullOrEmpty(request.InvoiceTaxCode) || x.InvoiceTaxCode == request.InvoiceTaxCode)).SumAsync(x => x.Amount);
    }



    public async Task<List<LedgerPrint>> GetListDataPrint(string OrginalVoucherNumber, int isInternal, int year)
    {
        var ledgers = await _context.GetLedger(year, isInternal).Where(x => x.OrginalVoucherNumber == OrginalVoucherNumber)
                            .ToListAsync();

        var ledgerPrints = new List<LedgerPrint>();

        foreach (var ledger in ledgers)
        {
            var ledgerPrint = _mapper.Map<LedgerPrint>(ledger);

            ChartOfAccount account = new ChartOfAccount();

            if (OrginalVoucherNumber.StartsWith("NK"))
            {
                if (!string.IsNullOrEmpty(ledger.DebitDetailCodeSecond))
                {
                    string parentRef = ledger.DebitCode + ":" + ledger.DebitDetailCodeFirst;
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.DebitDetailCodeSecond && x.Type == 6
                                    && x.ParentRef == parentRef);
                }
                else if (!string.IsNullOrEmpty(ledger.DebitDetailCodeFirst))
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.DebitDetailCodeFirst && x.Type == 5
                                   && x.ParentRef == ledger.DebitCode);
                }
                else
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.DebitCode);
                }
            }
            else if (OrginalVoucherNumber.StartsWith("XK"))
            {
                if (!string.IsNullOrEmpty(ledger.CreditDetailCodeSecond))
                {
                    string parentRef = ledger.CreditCode + ":" + ledger.CreditDetailCodeFirst;
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.CreditDetailCodeSecond && x.Type == 6
                                    && x.ParentRef == parentRef);
                }
                else if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.CreditDetailCodeFirst && x.Type == 5
                                   && x.ParentRef == ledger.CreditCode);
                }
                else
                {
                    account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == ledger.CreditCode);
                }
            }
            ledgerPrint.StockUnit = account?.StockUnit;

            ledgerPrints.Add(ledgerPrint);
        }
        return ledgerPrints;
    }

    // bước 2 Tạo bút toán Phân bổ CCDC / Khấu hao TSCĐ và F5
    public async Task<CustomActionResult<Ledger>> CreateFromFixedAsset(FixedAssetsModelEdit assets, AssetsType assetsType, int isInternal, int year)
    {
        CustomActionResult<Ledger> result = new CustomActionResult<Ledger>
        {
            IsSuccess = false
        };

        DateTime dtPeriodDate = assets.PeriodDate;
        DateTime dtReferenceBookDate = new DateTime(dtPeriodDate.Year, dtPeriodDate.Month, DateTime.DaysInMonth(dtPeriodDate.Year, dtPeriodDate.Month));

        AssetsType eAssetsType = assetsType == AssetsType.CCDCSD ? AssetsType.PB : assetsType;

        _ledgerHelperService.SetAutoIncrement(dtReferenceBookDate, eAssetsType, isInternal, year);

        CustomActionResult<Ledger> rsLedger = await FindByFixedAssets(assets, isInternal, year);
        FixedAsset242 cAssets = await _context.FixedAsset242.FindAsync(assets.Id);
        bool isUpdate = rsLedger.IsSuccess;
        var ledgerFixedAssetStored = await _context.LedgerFixedAssets.FirstOrDefaultAsync(x => x.FixedAsset242Id == cAssets.Id);

        if (!isUpdate)
        {
            Ledger ledger = new()
            {
                Id = 0,
                Type = eAssetsType.ToString(),
                Month = dtReferenceBookDate.Month,
                VoucherNumber = _ledgerHelperService.VoucherNumber,
                ReferenceBookDate = dtReferenceBookDate,
                OrginalVoucherNumber = _ledgerHelperService.OrginalVoucherNumber,
                OrginalBookDate = dtReferenceBookDate,
                BookDate = dtReferenceBookDate,
                DebitCode = cAssets.DebitCode,
                DebitDetailCodeFirst = cAssets.DebitDetailCodeFirst,
                DebitDetailCodeSecond = cAssets.DebitDetailCodeSecond,
                CreditCode = cAssets.CreditCode,
                CreditDetailCodeFirst = cAssets.CreditDetailCodeFirst,
                CreditDetailCodeSecond = cAssets.CreditDetailCodeSecond,
                Amount = cAssets.DepreciationOfThisPeriod ?? 0,
                IsInternal = ledgerFixedAssetStored.IsInternal,
            };
            // F4 bước 2
            if (eAssetsType == AssetsType.PB)
            {
                ledger.OrginalDescription = $"Phân bổ CCDC sử dụng tháng {dtReferenceBookDate.Month}/{dtReferenceBookDate.Year}";
            }
            //F5
            else
            {
                ledger.OrginalDescription = $"Khấu hao TSCĐ tháng {dtReferenceBookDate.Month}/{dtReferenceBookDate.Year}";
            }
            result.SuccessData = await Create(ledger, year);

            // add LedgerFixedAsset
            LedgerFixedAsset ledgerFixedAsset = new LedgerFixedAsset()
            {
                LedgerId = result.SuccessData.Id,
                FixedAsset242Id = cAssets.Id,
                IsInternal = ledgerFixedAssetStored.IsInternal
            };
            await _ledgerFixedAssetService.CreateAsync(ledgerFixedAsset);
        }
        else
        {
            Ledger cLedger = rsLedger.SuccessData;
            cLedger.DebitCode = cAssets.DebitCode;
            cLedger.DebitDetailCodeFirst = cAssets.DebitDetailCodeFirst;
            cLedger.DebitDetailCodeSecond = cAssets.DebitDetailCodeSecond;
            cLedger.CreditCode = cAssets.CreditCode;
            cLedger.CreditDetailCodeFirst = cAssets.CreditDetailCodeFirst;
            cLedger.CreditDetailCodeSecond = cAssets.CreditDetailCodeSecond;
            cLedger.Amount = cAssets.DepreciationOfThisPeriod ?? 0;

            result.SuccessData = await Update(cLedger, cLedger, year);
        }
        return result;
    }

    // bước 1 số 4
    public async Task CreateFromFixedAsset242(FixedAssetsModelEdit assets, AssetsType assetsType, int year)
    {
        DateTime dtReferenceBookDate = assets.UsedDate ?? DateTime.Now;

        AssetsType eAssetsType = assetsType == AssetsType.CCDCSD ? AssetsType.PB : assetsType;
        var ledgerFixedAssetStored = await _context.LedgerFixedAssets.FirstOrDefaultAsync(x => x.FixedAsset242Id == assets.Id);

        if (ledgerFixedAssetStored is null)
            return;

        _ledgerHelperService.SetAutoIncrement(dtReferenceBookDate, eAssetsType, ledgerFixedAssetStored.IsInternal, year);

        Ledger ledger = new()
        {
            Id = 0,
            Type = eAssetsType.ToString(),
            Month = dtReferenceBookDate.Month,
            VoucherNumber = _ledgerHelperService.VoucherNumber,
            ReferenceBookDate = dtReferenceBookDate,
            OrginalVoucherNumber = _ledgerHelperService.OrginalVoucherNumber,
            OrginalBookDate = dtReferenceBookDate,
            BookDate = dtReferenceBookDate,
            DebitCode = assets.DebitCode,
            DebitDetailCodeFirst = assets.DebitDetailCodeFirst,
            DebitDetailCodeSecond = assets.DebitDetailCodeSecond,
            CreditCode = assets.CreditCode,
            CreditDetailCodeFirst = assets.CreditDetailCodeFirst,
            CreditDetailCodeSecond = assets.CreditDetailCodeSecond,
            IsInternal = ledgerFixedAssetStored.IsInternal,
            Quantity = assets.Quantity ?? 0,
            UnitPrice = assets.UnitPrice ?? 0,
            Amount = (assets.Quantity ?? 0) * (assets.UnitPrice ?? 0),
        };

        var name = ledger.DebitDetailCodeSecond;
        if (string.IsNullOrEmpty(name))
            name = ledger.DebitDetailCodeFirst;
        ledger.OrginalDescription = $"Xuất kho CCDC {name} vào sử dụng {dtReferenceBookDate.Month}/{dtReferenceBookDate.Year}";
        await Create(ledger, year);

        // add LedgerFixedAsset
        LedgerFixedAsset ledgerFixedAsset = new LedgerFixedAsset()
        {
            LedgerId = ledger.Id,
            FixedAsset242Id = assets.Id,
            IsInternal = ledgerFixedAssetStored.IsInternal
        };
        await _ledgerFixedAssetService.CreateAsync(ledgerFixedAsset);
    }

    public async Task<CustomActionResult<Ledger>> FindByFixedAssets(FixedAssetsModelEdit assets, int isInternal, int year)
    {
        DateTime dtPeriodDate = assets.PeriodDate;
        DateTime dtReferenceBookDate = new DateTime(dtPeriodDate.Year, dtPeriodDate.Month, DateTime.DaysInMonth(dtPeriodDate.Year, dtPeriodDate.Month));
        string eAssetsType = assets.Type == AssetsType.CCDCSD.ToString() ? AssetsType.PB.ToString() : assets.Type;

        var qLedger = await _context.GetLedger(year, isInternal)
       .GroupJoin(_context.LedgerFixedAssets,
       l => l.Id,
       f => f.LedgerId,
       (l, f) => new { ledger = l, asset = f })
       .SelectMany(x => x.asset.DefaultIfEmpty(),
       (l, f) => new { l.ledger, asset = f })
       .Where(t => (t.ledger.CreditCode ?? string.Empty) == (assets.CreditCode ?? string.Empty)
                                           && (t.ledger.CreditDetailCodeFirst ?? string.Empty) == (assets.CreditDetailCodeFirst ?? string.Empty)
                                           && (t.ledger.CreditDetailCodeSecond ?? string.Empty) == (assets.CreditDetailCodeSecond ?? string.Empty)
                                           && (t.ledger.DebitCode ?? string.Empty) == (assets.DebitCode ?? string.Empty)
                                           && (t.ledger.DebitDetailCodeFirst ?? string.Empty) == (assets.DebitDetailCodeFirst ?? string.Empty)
                                           && (t.ledger.DebitDetailCodeSecond ?? string.Empty) == (assets.DebitDetailCodeSecond ?? string.Empty)
                                           && t.ledger.Type == eAssetsType
                                           && t.ledger.ReferenceBookDate == dtReferenceBookDate
                                           && t.ledger.Month == dtReferenceBookDate.Month
                                           && t.asset.FixedAsset242Id == assets.Id
                                   ).OrderByDescending(t => t.ledger.ReferenceBookDate)
                                   .Select(x => x.ledger).FirstOrDefaultAsync();

        return new CustomActionResult<Ledger>
        {
            IsSuccess = qLedger != null,
            SuccessData = qLedger
        };
    }

    public async Task<int> FindByFixedAssetCheckMonths(FixedAssetsModelEdit assets, int year)
    {
        string eAssetsType = assets.Type == AssetsType.CCDCSD.ToString() ? AssetsType.PB.ToString() : assets.Type;

        var ledgerFixedAssetStored = await _context.LedgerFixedAssets.FirstOrDefaultAsync(x => x.FixedAsset242Id == assets.Id);

        var qLedger = await _context.GetLedger(year, ledgerFixedAssetStored.IsInternal)
        .GroupJoin(_context.LedgerFixedAssets,
        l => l.Id,
        f => f.LedgerId,
        (l, f) => new { ledger = l, asset = f })
        .SelectMany(x => x.asset.DefaultIfEmpty(),
        (l, f) => new { l.ledger, asset = f })
        .Where(t => (t.ledger.CreditCode ?? string.Empty) == (assets.CreditCode ?? string.Empty)
                                            && (t.ledger.CreditDetailCodeFirst ?? string.Empty) == (assets.CreditDetailCodeFirst ?? string.Empty)
                                            && (t.ledger.CreditDetailCodeSecond ?? string.Empty) == (assets.CreditDetailCodeSecond ?? string.Empty)
                                            && (t.ledger.DebitCode ?? string.Empty) == (assets.DebitCode ?? string.Empty)
                                            && (t.ledger.DebitDetailCodeFirst ?? string.Empty) == (assets.DebitDetailCodeFirst ?? string.Empty)
                                            && (t.ledger.DebitDetailCodeSecond ?? string.Empty) == (assets.DebitDetailCodeSecond ?? string.Empty)
                                            && t.ledger.Type == eAssetsType
                                            && t.asset.FixedAsset242Id == assets.Id
                                    ).OrderByDescending(t => t.ledger.ReferenceBookDate).FirstOrDefaultAsync();

        if (qLedger?.ledger?.Month == null)
            return 0;
        return (qLedger.ledger?.Month ?? 0) + 1;
    }

    public void ValidateDataReport(LedgerReportParamDetail request)
    {
        if (request.FilterType == 1 && request.FromMonth > request.ToMonth)
        {
            throw new ErrorException("Đến tháng cần lớn hơn Từ tháng");
        }

        if (request.FilterType == 2 && request.FromDate > request.ToDate)
        {
            throw new ErrorException("Đến ngày cần lớn hơn Từ ngày");
        }
    }
}