using AutoMapper;
using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.V3;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Ledgers.V3;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.LedgerServices;

public class LedgerWareHouseService : ILedgerWareHouseService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ILedgerV3Service _ledgerV3Service;
    private readonly IMapper _mapper;
    public LedgerWareHouseService(ApplicationDbContext dbcontext, ILedgerV3Service ledgerV3Service, IMapper mapper)
    {
        _dbcontext = dbcontext;
        _ledgerV3Service = ledgerV3Service;
        _mapper = mapper;
    }

    public async Task Create(List<LedgerWarehouseCreate> requests, string typePay, int customerId, bool isPrintBill, int year)
    {
        if (!requests.Any())
        {
            throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
        }

        var listChartOfAcc = await _dbcontext.GetChartOfAccount(year).ToListAsync();
        string TypePayLedger = "PC";
        if (typePay == "NH")
            TypePayLedger = "CH";
        else if (typePay == "CN")
            TypePayLedger = "NK";
        int maxOriginalVoucher = 0;
        int monthLed = DateTime.Today.Month;
        string orginalVoucherNumber = "";
        string voucherMonth = monthLed < 10 ? "0" + monthLed : monthLed.ToString();

        var ledgerExist = await _dbcontext.GetLedger(year, 3).AsNoTracking().Where(x => !x.IsDelete && x.Type == TypePayLedger
                                                            && x.OrginalBookDate.Value.Year == DateTime.Today.Year && x.OrginalBookDate.Value.Month == DateTime.Today.Month).ToListAsync();
        if (ledgerExist != null && ledgerExist.Count > 0)
        {
            maxOriginalVoucher = ledgerExist.Max(x => int.Parse(x.OrginalVoucherNumber.Split('-').Last()));
        }
        maxOriginalVoucher++;

        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);

        orginalVoucherNumber = $"{TypePayLedger}{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";

        var accountPays = await _dbcontext.AccountPays.ToListAsync();
        var khachHang = await _dbcontext.Customers.FindAsync(customerId);
        var khachHang_tax = await _dbcontext.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == customerId);

        var invoiceDeclaration = await _dbcontext.InvoiceDeclarations.FirstOrDefaultAsync(x => x.Name == "R01" && x.Month == DateTime.Now.Month);
        var invoiceNumber = _dbcontext.GetLedger(year, 2).AsNoTracking().Where(x => x.InvoiceNumber != null && x.InvoiceNumber.Length == 7 && x.Month == DateTime.Now.Month).OrderByDescending(x => x.InvoiceNumber).FirstOrDefault()?.InvoiceNumber ?? "";
        if (isPrintBill)
        {
            if (invoiceDeclaration == null)
            {
                throw new ErrorException(ErrorMessage.NOT_DECLARE_INVOICE);
            }
            int invoiceNumberMax = invoiceDeclaration.FromOpening ?? 1;

            if (!string.IsNullOrEmpty(invoiceNumber))
                invoiceNumberMax = int.Parse(invoiceNumber) + 1;
            invoiceNumber = invoiceNumberMax.ToString();
            while (true)
            {
                if (invoiceNumber.Length >= 7)
                    break;
                invoiceNumber = "0" + invoiceNumber;
            }
        }

        var taxRates = await _dbcontext.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
        taxRates = taxRates.Where(x => x.Code.StartsWith('R')).ToList();

        var ledgers = new List<LedgerV3UpdateModel>();
        foreach (var request in requests)
        {
            var good = await _dbcontext.Goods.FindAsync(request.GoodsId);

            LedgerV3UpdateModel ledger = new LedgerV3UpdateModel();
            var taxRate = taxRates.Find(x => x.Id == good.TaxRateId);
            ledger.InvoiceCode = taxRate?.Code;

            ledger.Type = TypePayLedger;
            ledger.Month = monthLed;
            ledger.BookDate = DateTime.Today;
            ledger.VoucherNumber = voucherMonth + "/" + ledger.Type;
            ledger.IsVoucher = false;

            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.OrginalBookDate = DateTime.Today;
            ledger.ReferenceBookDate = DateTime.Today;
            ledger.InvoiceDate = DateTime.Today;

            ledger.DebitCode = good.Account;
            ledger.DebitCodeName = good.AccountName;
            ledger.DebitWarehouse = good.Warehouse;
            ledger.DebitWarehouseName = good.WarehouseName;
            ledger.DebitDetailCodeFirst = good.Detail1;
            ledger.DebitDetailCodeFirstName = good.DetailName1;
            ledger.DebitDetailCodeSecond = good.Detail2;
            ledger.DebitDetailCodeSecondName = good.DetailName2;

            ledger.Quantity = request.Quantity;
            ledger.UnitPrice = request.UnitPrice;
            ledger.Amount = request.Quantity * ledger.UnitPrice;
            ledger.OrginalCode = "";
            ledger.OrginalFullName = "";
            string maHang = "";
            if (!string.IsNullOrEmpty(ledger.DebitDetailCodeSecondName))
                maHang = ledger.DebitDetailCodeSecondName;
            else if (!string.IsNullOrEmpty(ledger.DebitDetailCodeFirstName))
                maHang = ledger.DebitDetailCodeFirstName;
            else
                maHang = ledger.DebitCodeName;

            ledger.OrginalDescription = "Nhập bán " + maHang;
            if (good.GoodsType == nameof(GoodsTypeEnum.COMBO))
            {
                ledger.OrginalDescription = "Nhập bán Combo " + maHang + " khuyến mãi";
            }
            ledger.OrginalDescriptionEN = "";
            ledger.AttachVoucher = "";

            if (typePay == "TM")
            {
                var accountPay = accountPays.Find(x => x.Code == "TM");

                ledger.CreditCode = accountPay?.Account;
                ledger.CreditCodeName = accountPay?.AccountName;
                ledger.CreditDetailCodeFirst = accountPay?.Detail1;
                ledger.CreditDetailCodeFirstName = accountPay?.DetailName1;
                ledger.CreditDetailCodeSecond = accountPay?.Detail2;
                ledger.CreditDetailCodeSecondName = accountPay?.DetailName2;
            }
            if (typePay == "NH")
            {
                var accountPayNH = accountPays.Find(x => x.Code == "NH");
                ledger.CreditCode = accountPayNH?.Account;
                ledger.CreditCodeName = accountPayNH?.AccountName;
                ledger.CreditDetailCodeFirst = accountPayNH?.Detail1;
                ledger.CreditDetailCodeFirstName = accountPayNH?.DetailName1;
                ledger.CreditDetailCodeSecond = accountPayNH?.Detail2;
                ledger.CreditDetailCodeSecondName = accountPayNH?.DetailName2;
            }
            if (khachHang != null)
            {
                if (typePay == "CN")
                {
                    ledger.CreditCode = khachHang.DebitCode;
                    ledger.CreditDetailCodeFirst = khachHang.DebitDetailCodeFirst;
                    ledger.CreditDetailCodeFirstName = listChartOfAcc.Find(x => x.Code == ledger.DebitDetailCodeFirst && x.ParentRef == ledger.DebitCode)?.Name;
                    if (!string.IsNullOrEmpty(khachHang.DebitDetailCodeSecond))
                    {
                        ledger.CreditDetailCodeSecond = khachHang.DebitDetailCodeSecond;
                        ledger.CreditDetailCodeSecondName = listChartOfAcc.Find(x => x.ParentRef.Contains(ledger.DebitDetailCodeFirst) && x.Code == ledger.DebitDetailCodeSecond)?.Name;
                    }
                }

                ledger.OrginalCompanyName = khachHang.Name;
                ledger.OrginalAddress = khachHang.Address;
            }
            else
            {
                ledger.OrginalAddress = "";
                ledger.OrginalCompanyName = "Khách hàng online";
            }

            ledger.DebitCodeName = listChartOfAcc.Find(x => x.Code == ledger.DebitCode)?.Name;

            ledger.ReferenceVoucherNumber = "";
            ledger.ReferenceFullName = "";
            ledger.ReferenceAddress = "";
            ledger.InvoiceAdditionalDeclarationCode = "BT";
            ledger.InvoiceProductItem = "";
            ledger.ProjectCode = "";
            ledger.IsInternal = 3;

            if (isPrintBill)
            {
                ledger.InvoiceCode = invoiceDeclaration.Name;
                ledger.InvoiceSerial = invoiceDeclaration.TemplateSymbol;
                ledger.InvoiceNumber = invoiceNumber;

                ledger.InvoiceTaxCode = khachHang_tax?.TaxCode;
                ledger.InvoiceName = khachHang_tax?.CompanyName;
                ledger.InvoiceAddress = khachHang_tax?.Address;
                ledger.IsInternal = 1;
            }
            ledger.CustomerId = khachHang.Id;

            ledgers.Add(ledger);
        }
        await _ledgerV3Service.UpdateAsync(ledgers, year);
    }

    public async Task<PagingResult<LedgerWarehousePaging>> GetListHistory(LedgerWarehousesRequestPaging param)
    {
        var query = _dbcontext.LedgerWareHouses.Where(x => (param.Month == 0 || x.Month == param.Month)
                    && (string.IsNullOrEmpty(param.Type) || x.Type == param.Type)
                    && (param.IsInternal == 0 || x.IsInternal == param.IsInternal))
            .OrderBy(x => x.Id);
        if (param.Page < 1)
            param.Page = 1;

        var items = await query.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToListAsync();
        var itemOuts = new List<LedgerWarehousePaging>();

        foreach (var item in items)
        {
            var ledgerIds = item.LedgerIds.Split(",").Select(x => long.Parse(x)).ToList();
            var ledgers = await _dbcontext.Ledgers.Where(x => ledgerIds.Contains(x.Id)).ToListAsync();
            var ledger = ledgers.FirstOrDefault();

            LedgerWarehousePaging itemOut = new LedgerWarehousePaging
            {
                Id = item.Id,
                OrginalBookDate = ledger?.OrginalBookDate,
                OrginalDescription = ledger?.OrginalDescription,
                OrginalVoucherNumber = ledger?.OrginalVoucherNumber,
                TotalAmount = ledgers.Sum(x => x.Amount),
                CustomerId = item.CustomerId,
                Month = item.Month,
                Type = item.Type,
                IsInternal = item.IsInternal
            };
            itemOuts.Add(itemOut);
        }
        return new PagingResult<LedgerWarehousePaging>
        {
            PageSize = param.PageSize,
            CurrentPage = param.Page,
            TotalItems = await query.CountAsync(),
            Data = itemOuts
        };
    }

    public async Task<LedgerWarehouseDetail> GetDetailHistory(int id)
    {
        var ledgerWareHouse = await _dbcontext.LedgerWareHouses.FindAsync(id);

        if (ledgerWareHouse is null)
        {
            throw new ErrorException();
        }
        var itemOut = _mapper.Map<LedgerWarehouseDetail>(ledgerWareHouse);

        var ledgerIds = ledgerWareHouse.LedgerIds.Split(",").Select(x => long.Parse(x)).ToList();
        itemOut.ledgers = await _dbcontext.Ledgers.Where(x => ledgerIds.Contains(x.Id)).ToListAsync();

        return itemOut;
    }
}