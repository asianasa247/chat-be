using AutoMapper;
using Common.Errors;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Entities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.UserEntites;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ManageEmployee.Dal.Migrations;

namespace ManageEmployee.Services.LedgerServices;
public class LedgerForSaleService : ILedgerForSaleService
{
    private readonly ApplicationDbContext _context;
    private readonly ILedgerHelperService _ledgerHelperService;
    private readonly ILedgerService _ledgerService;
    private readonly IMapper _mapper;

    private List<AccountPay> _accountPays;
    private List<ChartOfAccount> _chartOfAcounts;
    private List<BillDetail> _billDetails;
    private List<ChartOfAccountGroupLink> _accountLinks;
    private List<Warehouse> _wareHouses;
    private List<TaxRate> _taxRates;
    private InvoiceDeclaration _invoiceDeclaration;
    private Customer _customer;
    private CustomerTaxInformation _customer_tax;
    private Bill _bill;
    private string _invoiceNumber;

    public LedgerForSaleService(ApplicationDbContext context, ILedgerHelperService ledgerHelperService, ILedgerService ledgerService, IMapper mapper)
    {
        _context = context;
        _ledgerHelperService = ledgerHelperService;
        _ledgerService = ledgerService;
        _mapper = mapper;
    }

    public async Task AddLedgerBill(List<BillDetail> listBillDetail, int year)
    {
        if (!listBillDetail.Any())
        {
            throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
        }
        await InitForBill(listBillDetail);

        await Init(year);

        string typePayLedger = GetLedgerType();
        int monthLed = DateTime.Today.Month;
        string voucherMonth = monthLed < 10 ? "0" + monthLed : monthLed.ToString();
        int maxOriginalVoucher = await _ledgerHelperService.GetOriginalVoucher(_bill.IsPrintBill, typePayLedger, year);
        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
        string orginalVoucherNumber = $"{typePayLedger}{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";

        var maxOriginalVoucherXK = await _ledgerHelperService.GetOriginalVoucher(isNotInternal: true, "XK", year);
        var orderStringXK = LedgerHelper.GetOriginalVoucher(maxOriginalVoucherXK);

        Ledger ledger = null;
        foreach (BillDetail requests in listBillDetail)
        {
            ledger = await AddLedgerFromBillDetail(requests, voucherMonth, orginalVoucherNumber, maxOriginalVoucher, maxOriginalVoucherXK, orderStringXK, year);
        }

        if (listBillDetail.Sum(x => x.TaxVAT * x.Quantity) > 0
            || _bill.Vat > 0)
        {
            await AddLedgerVat(ledger, year);
        }
    }

    public async Task AddLedgerProduceProduct(List<ProduceProductDetail> produceProductDetails, string typePayLedger, int year, int? responsibleUserId)
    {
        if (!produceProductDetails.Any())
        {
            throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
        }
        await Init(year);

        int monthLed = DateTime.Today.Month;
        string voucherMonth = monthLed < 10 ? "0" + monthLed : monthLed.ToString();
        int maxOriginalVoucher = await _ledgerHelperService.GetOriginalVoucher(true, typePayLedger, year);
        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
        string orginalVoucherNumber = $"{typePayLedger}{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";
        var orginalCompanyName = await _context.Users.Where(x => x.Id == responsibleUserId).Select(x => x.FullName).FirstOrDefaultAsync();

        if (typePayLedger == "NK")
        {
            await AddLedgerFromImportProduceProduct(produceProductDetails, voucherMonth, orginalVoucherNumber, maxOriginalVoucher, typePayLedger, year, orginalCompanyName);
        }
        else
        {
            await AddLedgerFromExportProduceProduct(produceProductDetails, voucherMonth, orginalVoucherNumber, maxOriginalVoucher, typePayLedger, year, orginalCompanyName);
        }

       

    }

    private async Task Init(int year)
    {
        _accountPays = await _context.AccountPays.ToListAsync();

        if (_bill != null)
        {
            _customer = await _context.Customers.FindAsync(_bill.CustomerId);
            _customer_tax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == _bill.CustomerId);
        }

        _chartOfAcounts = await _context.GetChartOfAccount(year).ToListAsync();
        _wareHouses = await _context.Warehouses.ToListAsync();
        _accountLinks = await _context.GetChartOfAccountGroupLink(year).ToListAsync();
        _invoiceDeclaration = await _context.InvoiceDeclarations.FirstOrDefaultAsync(x => x.Name == "R01" && x.Month == DateTime.Now.Month);

        if (_bill != null)
        {
            _invoiceNumber = await GetInvoiceNumber(year);
        }
        _taxRates = await _context.TaxRates.Where(x => x.Code.Contains("R")).ToListAsync();
    }

    public async Task InitForBill(List<BillDetail> listBillDetail)
    {
        _billDetails = listBillDetail;
        _bill = await _context.Bills.FindAsync(listBillDetail.FirstOrDefault()?.BillId);
    }
    private async Task<string> GetInvoiceNumber(int year)
    {
        var invoiceNumber = await _context.GetLedger(year, 2).AsNoTracking().Where(x => x.InvoiceNumber != null && x.InvoiceNumber.Length == 7 && x.Month == DateTime.Now.Month).OrderByDescending(x => x.InvoiceNumber).Select(x => x.InvoiceNumber).FirstOrDefaultAsync();

        if (_bill != null && _bill.IsPrintBill)
        {
            if (_invoiceDeclaration == null)
            {
                throw new ErrorException(ErrorMessage.NOT_DECLARE_INVOICE);
            }
            int invoiceNumberMax = _invoiceDeclaration.FromOpening ?? 1;

            if (!string.IsNullOrEmpty(invoiceNumber))
                invoiceNumberMax = int.Parse(invoiceNumber) + 1;
            invoiceNumber = invoiceNumberMax.ToString();
            while (true)
            {
                if (invoiceNumber.Length >= 7)
                    break;
                invoiceNumber = "0" + invoiceNumber;
            }
            _bill.InvoiceNumber = invoiceNumber;
            _context.Update(_bill);
            await _context.SaveChangesAsync();
        }
        return invoiceNumber;
    }
    private string GetLedgerType()
    {
        string typePayLedger = "PT";
        if (_bill.TypePay == "NH")
            typePayLedger = "NH";
        else if (_bill.TypePay == "CN")
            typePayLedger = "BS";
        return typePayLedger;

    }
    private async Task<Ledger> AddLedgerFromBillDetail(BillDetail requests, string voucherMonth, string orginalVoucherNumber,
        int maxOriginalVoucher, int maxOriginalVoucherXK, string orderStringXK, int year)
    {
        var ledger = LedgerHelper.LedgerInit();
        ledger.CreditCode = "5111";
        ledger.InvoiceAdditionalDeclarationCode = "BT";

        var good = await _context.Goods.FindAsync(requests.GoodsId);

        ledger = MapLedgerFormGood(ledger, good, "5");
        var taxRate = _taxRates.Find(x => x.Id == good.TaxRateId);
        ledger.InvoiceCode = taxRate?.Code;
        ledger.BillId = requests.BillId;
        ledger.Type = GetLedgerType();
        ledger.VoucherNumber = voucherMonth + "/" + ledger.Type;
        ledger.OrginalVoucherNumber = orginalVoucherNumber;
        ledger.Order = maxOriginalVoucher;
        ledger.ProjectCode = _bill.ProjectCode;

        ledger.Quantity = requests.Quantity;
        ledger.UnitPrice = requests.UnitPrice > 0 ? (requests.UnitPrice - (requests.DiscountType == "money" ? requests.DiscountPrice : requests.DiscountPrice * requests.UnitPrice / 100)) : 0;
        ledger.Amount = requests.Quantity * ledger.UnitPrice;

        ledger = MapDebitFromAccountPay(ledger);
        if (_customer != null)
        {
            ledger.OrginalCompanyName = _customer.Name;
            ledger.OrginalAddress = _customer.Address;
        }
        else
        {
            ledger.OrginalAddress = "";
            ledger.OrginalCompanyName = "Khách hàng online";
        }

        ledger.AttachVoucher = string.IsNullOrEmpty(requests.DeliveryCode) ? _bill.BillNumber : requests.DeliveryCode;
        ledger.IsInternal = 3;
        if (_bill.Type == "BS")
        {
            ledger.IsInternal = 0;
        }

        if (_bill.IsPrintBill)
        {
            ledger.InvoiceCode = _invoiceDeclaration?.Name;
            ledger.InvoiceSerial = _invoiceDeclaration?.TemplateSymbol;
            ledger.InvoiceNumber = _invoiceNumber;

            ledger.InvoiceTaxCode = _customer_tax?.TaxCode;
            ledger.InvoiceName = _customer_tax?.CompanyName;
            ledger.InvoiceAddress = _customer_tax?.Address;
           // ledger.IsInternal = 1;
        }
        if (ledger.DebitCode != null && ledger.CreditCode != null)
        {
            await _ledgerService.Create(ledger, year);
        }

        Ledger xuatKho = null;

        if (_bill.Type == "BSN")
        {
            xuatKho = await LedgerXK(ledger, voucherMonth, maxOriginalVoucherXK, orderStringXK, good, year, _bill.Type);
        } if (_bill.Type == "BS")
        {
            xuatKho = await LedgerXK(ledger, voucherMonth, maxOriginalVoucherXK, orderStringXK, good, year, _bill.Type);
        }

        if (xuatKho != null)
            requests.Price = xuatKho.UnitPrice;
        return ledger;
    }

    private async Task AddLedgerFromImportProduceProduct(List<ProduceProductDetail> produceProductDetails, string voucherMonth, string orginalVoucherNumber,
        int maxOriginalVoucher, string typePayLedger, int year, string orginalCompanyName)
    {
        foreach (var produceProductDetail in produceProductDetails)
        {
            var good = await _context.Goods.FindAsync(produceProductDetail.GoodsId);

            var ledger = LedgerHelper.LedgerInit();
            ledger.CreditCode = "5111";
            ledger.InvoiceAdditionalDeclarationCode = "BT";

            ledger = MapLedgerFormGood(ledger, good, "154");

            ledger.DebitCode = good.Account;
            ledger.DebitCodeName = good.AccountName;
            ledger.DebitDetailCodeFirst = ledger.CreditDetailCodeFirst;
            ledger.DebitDetailCodeFirstName = ledger.CreditDetailCodeFirstName;
            ledger.DebitDetailCodeSecond = ledger.CreditDetailCodeSecond;
            ledger.DebitDetailCodeSecondName = ledger.CreditDetailCodeSecondName;

            ledger.Type = typePayLedger;
            ledger.VoucherNumber = voucherMonth + "/" + ledger.Type;
            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.Quantity = produceProductDetail.QuantityReal;
            ledger.UnitPrice = good.SalePrice;
            ledger.Amount = good.SalePrice * produceProductDetail.QuantityReal;
            ledger.IsInternal = 4;
            ledger.OrginalDescription = $"Nhập kho thành phẩm";
            ledger.OrginalCompanyName = orginalCompanyName;

            if (ledger.DebitCode != null && ledger.CreditCode != null)
            {
                await _ledgerService.Create(ledger, year);
            }
        }
    }

    private async Task AddLedgerFromExportProduceProduct(List<ProduceProductDetail> produceProductDetails, string voucherMonth, string orginalVoucherNumber,
        int maxOriginalVoucher, string typePayLedger, int year, string orginalCompanyName)
    {
        var goodIds = produceProductDetails.Select(x => x.GoodsId);
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var goodIdHasGoodDetails = goods.Where(x => x.GoodsType == nameof(GoodsTypeEnum.CB)
                || x.GoodsType == nameof(GoodsTypeEnum.COMBO)
                || x.GoodsType == nameof(GoodsTypeEnum.DM)).Select(x => x.Id);
        var goodDetails = await _context.GoodDetails.Where(x => goodIdHasGoodDetails.Contains(x.GoodID ?? 0)).ToListAsync();

        
        foreach (var good in goods)
        {
            var ledger = LedgerHelper.LedgerInit();
            double dUnitPriceAvg;
            ledger.InvoiceAdditionalDeclarationCode = "BT";
            ledger = MapLedgerFormGood(ledger, good, "154");

            ledger.DebitCode = "154";
            ledger.DebitCodeName = good.AccountName;
            ledger.DebitDetailCodeFirst = ledger.CreditDetailCodeFirst;
            ledger.DebitDetailCodeFirstName = ledger.CreditDetailCodeFirstName;
            ledger.DebitDetailCodeSecond = ledger.CreditDetailCodeSecond;
            ledger.DebitDetailCodeSecondName = ledger.CreditDetailCodeSecondName;


            ledger.Type = typePayLedger;
            ledger.VoucherNumber = voucherMonth + "/" + ledger.Type;
            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.CreditWarehouse = good.Warehouse;
            ledger.CreditWarehouseName = _wareHouses.Find(x => x.Code == ledger.CreditWarehouse)?.Name;
            ledger.IsInternal = 4;
            ledger.OrginalDescription = $"Xuất kho nguyên vật liệu";
            ledger.OrginalCompanyName = orginalCompanyName;


            if (good.GoodsType == nameof(GoodsTypeEnum.CB)
                    || good.GoodsType == nameof(GoodsTypeEnum.COMBO)
                    || good.GoodsType == nameof(GoodsTypeEnum.DM))//cb: CHE BIEN; COMBO
            {
                continue;
            }
            else
            {
                dUnitPriceAvg = await _ledgerService.TinhGiaXuatKho(good.Account, ledger.CreditDetailCodeFirst, ledger.CreditDetailCodeSecond, ledger.CreditWarehouse, ledger.OrginalBookDate ?? DateTime.Today, year);
                var quantity = produceProductDetails.Where(x => x.GoodsId == good.Id).Sum(x => x.QuantityReal);
                ledger.CreditCode = good.Account;
                ledger.CreditCodeName = good.AccountName;
                ledger.CreditDetailCodeFirst = good.Detail1;
                ledger.CreditDetailCodeFirstName = good.DetailName1;
                ledger.CreditDetailCodeSecond = good.Detail2;
                ledger.CreditDetailCodeSecondName = good.DetailName2;

                ledger.Quantity = quantity;
                ledger.UnitPrice = dUnitPriceAvg;
                ledger.Amount = dUnitPriceAvg * quantity;
                if (ledger.DebitCode != null && ledger.CreditCode != null)
                {
                    await _ledgerService.Create(ledger, year);
                }

            }
        }

        var goodDetailChecks = new List<GoodDetail>();
        foreach (var goodDetail in goodDetails)
        {
            var good = goods.Find(x => x.Id == goodDetail.GoodID);

            var goodDetailCheck = goodDetailChecks.FirstOrDefault(x => x.Account == goodDetail.Account && x.Detail1 == goodDetail.Detail1 
                        && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodDetail.Detail2)
                        && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == goodDetail.Warehouse));
            if (goodDetailCheck is null)
            {
                goodDetailChecks.Add(new GoodDetail
                {
                    Account = goodDetail.Account,
                    Detail1 = goodDetail.Detail1,
                    Detail2 = goodDetail.Detail2,
                    Warehouse = good.Warehouse,
                    Quantity = goodDetail.Quantity,
                });
            }
            else
            {
                goodDetailCheck.Quantity += goodDetail.Quantity;
            }
        }

        foreach (var goodDetail in goodDetailChecks)
        {
            var goodDetailFinds = goodDetails.Where(x => x.Account == goodDetail.Account && x.Detail1 == goodDetail.Detail1 
                                            && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodDetail.Detail2)
                                            && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == goodDetail.Warehouse));
            double quantity = 0;
            foreach(var goodDetailFind in goodDetailFinds)
            {
                var quantityReal = produceProductDetails.Where(x => x.GoodsId == goodDetailFind.GoodID).Sum(x => x.QuantityReal);
                quantity += (goodDetailFind.Quantity ?? 0) * quantityReal;
            }

            var ledger = LedgerHelper.LedgerInit();
            double dUnitPriceAvg;
            ledger.InvoiceAdditionalDeclarationCode = "BT";
            ledger = MapLedgerFormGoodDetail(ledger, goodDetail, "154");

            ledger.DebitCode = "154";
            ledger.DebitDetailCodeFirst = ledger.CreditDetailCodeFirst;
            ledger.DebitDetailCodeFirstName = ledger.CreditDetailCodeFirstName;
            ledger.DebitDetailCodeSecond = ledger.CreditDetailCodeSecond;
            ledger.DebitDetailCodeSecondName = ledger.CreditDetailCodeSecondName;


            ledger.Type = typePayLedger;
            ledger.VoucherNumber = voucherMonth + "/" + ledger.Type;
            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.CreditWarehouse = goodDetail.Warehouse;
            ledger.CreditWarehouseName = _wareHouses.Find(x => x.Code == ledger.CreditWarehouse)?.Name;
            ledger.IsInternal = 4;
            ledger.OrginalDescription = $"Xuất kho nguyên vật liệu";



            string ledgerString = JsonConvert.SerializeObject(ledger);
            Ledger item = JsonConvert.DeserializeObject<Ledger>(ledgerString);
            dUnitPriceAvg = await _ledgerService.TinhGiaXuatKho(goodDetail.Account, goodDetail.Detail1, goodDetail.Detail2, goodDetail.Warehouse, ledger.OrginalBookDate ?? DateTime.Today, year);
            item.Id = 0;
            item.CreditCode = goodDetail.Account;
            item.CreditCodeName = goodDetail.AccountName;
            item.CreditDetailCodeFirst = goodDetail.Detail1;
            item.CreditDetailCodeFirstName = goodDetail.DetailName1;
            item.CreditDetailCodeSecond = goodDetail.Detail2;
            item.CreditDetailCodeSecondName = goodDetail.DetailName2;
            item.Quantity = quantity;
            item.UnitPrice = dUnitPriceAvg;
            item.Amount = dUnitPriceAvg * quantity;
            if (item.DebitCode != null && item.CreditCode != null)
            {
                await _ledgerService.Create(item, year);
            }

        }
    }

    private Ledger MapLedgerFormGood(Ledger ledger, Goods good, string accountCodeLink)
    {

        var codeChartOfAccountGroups = _accountLinks.Where(x => x.CodeChartOfAccount == good.Account).Select(x => x.CodeChartOfAccountGroup);
        if (codeChartOfAccountGroups.Any())
        {
            int accountCodeLinkLength = accountCodeLink.Length;
            var accountGroupS = _accountLinks.Find(x => codeChartOfAccountGroups.Contains(x.CodeChartOfAccountGroup) && x.CodeChartOfAccount.Substring(0, accountCodeLinkLength) == accountCodeLink)?.CodeChartOfAccount;
            var creditacc = _chartOfAcounts.Find(x => x.Code == accountGroupS);
            if (creditacc != null)
                ledger.CreditCode = creditacc.Code;
        }
        ledger.CreditCodeName = _chartOfAcounts.Find(x => x.Code == ledger.CreditCode)?.Name;
        ledger.CreditWarehouse = good.Warehouse;
        ledger.CreditWarehouseName = _wareHouses.Find(x => x.Code == ledger.CreditWarehouse)?.Name;

        ledger.CreditDetailCodeFirst = good.Detail1;
        ledger.CreditDetailCodeFirstName = _chartOfAcounts.Find(x => x.Code == ledger.CreditDetailCodeFirst && x.ParentRef == ledger.CreditCode)?.Name;
        if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
        {
            ledger.CreditDetailCodeSecond = good.Detail2;
            ledger.CreditDetailCodeSecondName = _chartOfAcounts.Find(x => x.ParentRef.Contains(ledger.CreditDetailCodeFirst) && x.Code == good.Detail2)?.Name;
        }
        string maHang = GoodNameGetter.GetNameFromGood(good);
        ledger.OrginalDescription = "Doanh thu bán " + maHang;
        if (good.GoodsType == nameof(GoodsTypeEnum.COMBO))
        {
            ledger.OrginalDescription = "Xuất bán Combo " + maHang + " khuyến mãi";
        }
        return ledger;
    }

    private Ledger MapLedgerFormGoodDetail(Ledger ledger, GoodDetail goodDetail, string accountCodeLink)
    {

        var codeChartOfAccountGroups = _accountLinks.Where(x => x.CodeChartOfAccount == goodDetail.Account).Select(x => x.CodeChartOfAccountGroup);
        if (codeChartOfAccountGroups.Any())
        {
            int accountCodeLinkLength = accountCodeLink.Length;
            var accountGroupS = _accountLinks.Find(x => codeChartOfAccountGroups.Contains(x.CodeChartOfAccountGroup) && x.CodeChartOfAccount.Substring(0, accountCodeLinkLength) == accountCodeLink)?.CodeChartOfAccount;
            var creditacc = _chartOfAcounts.Find(x => x.Code == accountGroupS);
            if (creditacc != null)
                ledger.CreditCode = creditacc.Code;
        }
        ledger.CreditCodeName = _chartOfAcounts.Find(x => x.Code == ledger.CreditCode)?.Name;
        ledger.CreditWarehouse = goodDetail.Warehouse;
        ledger.CreditWarehouseName = _wareHouses.Find(x => x.Code == ledger.CreditWarehouse)?.Name;

        ledger.CreditDetailCodeFirst = goodDetail.Detail1;
        ledger.CreditDetailCodeFirstName = _chartOfAcounts.Find(x => x.Code == ledger.CreditDetailCodeFirst && x.ParentRef == ledger.CreditCode)?.Name;
        if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
        {
            ledger.CreditDetailCodeSecond = goodDetail.Detail2;
            ledger.CreditDetailCodeSecondName = _chartOfAcounts.Find(x => x.ParentRef.Contains(ledger.CreditDetailCodeFirst) && x.Code == goodDetail.Detail2)?.Name;
        }
        string maHang = GoodNameGetter.GetNameFromGoodDetail(goodDetail);
        ledger.OrginalDescription = "Doanh thu bán " + maHang;
        //if (goodDetail.GoodsType == nameof(GoodsTypeEnum.COMBO))
        //{
        //    ledger.OrginalDescription = "Xuất bán Combo " + maHang + " khuyến mãi";
        //}
        return ledger;
    }

    private Ledger MapDebitFromAccountPay(Ledger ledger)
    {
        ledger.DebitCode = "1111";

        var accountPay = _accountPays.Find(x => x.Code == "TM");
        var accountPayNH = _accountPays.Find(x => x.Code == "NH");

        if (accountPay != null && _bill.TypePay == "TM")
        {
            ledger.DebitCode = accountPay.Account;
            ledger.DebitCodeName = accountPay.AccountName;
            ledger.DebitDetailCodeFirst = accountPay.Detail1;
            ledger.DebitDetailCodeFirstName = accountPay.DetailName1;
            ledger.DebitDetailCodeSecond = accountPay.Detail2;
            ledger.DebitDetailCodeSecondName = accountPay.DetailName2;
        }
        else if (accountPayNH != null && _bill.TypePay == "NH")
        {
            ledger.DebitCode = accountPayNH.Account;
            ledger.DebitCodeName = accountPayNH.AccountName;
            ledger.DebitDetailCodeFirst = accountPayNH.Detail1;
            ledger.DebitDetailCodeFirstName = accountPayNH.DetailName1;
            ledger.DebitDetailCodeSecond = accountPayNH.Detail2;
            ledger.DebitDetailCodeSecondName = accountPayNH.DetailName2;
        }
        if (_bill.TypePay == "CN" && _customer != null)
        {
            ledger.DebitCode = _customer.DebitCode;
            ledger.DebitCodeName = _chartOfAcounts.Find(x => x.Code == ledger.DebitCode)?.Name;
            ledger.DebitDetailCodeFirst = _customer.DebitDetailCodeFirst;
            ledger.DebitDetailCodeFirstName = _chartOfAcounts.Find(x => x.Code == ledger.DebitDetailCodeFirst && x.ParentRef == ledger.DebitCode)?.Name;
            if (!string.IsNullOrEmpty(ledger.DebitCode))
            {
                ledger.DebitDetailCodeSecond = _customer.DebitDetailCodeSecond;
                ledger.DebitDetailCodeSecondName = _chartOfAcounts.Find(x => x.ParentRef.Contains(ledger.DebitDetailCodeFirst) && x.Code == ledger.DebitDetailCodeSecond)?.Name;
            }
        }
        return ledger;
    }
    private async Task<Ledger> LedgerXK(Ledger ledger, string voucherMonth, int maxOriginalVoucher, string orderString, Goods goods, int year, string billType = "BSN")
    {
        List<ChartOfAccount> listCoaDebitDefault = _context.GetChartOfAccount(year).AsNoTracking().Where(co => co.Type < 5).ToList();

        List<ChartOfAccountGroupLink> listDebitCodeGroupLink = (
                                    from gl in _context.GetChartOfAccountGroupLink(year)
                                    join co in _context.GetChartOfAccount(year) on gl.CodeChartOfAccount equals co.Code
                                    select gl).ToList();

        ChartOfAccountGroupLink debitCodeGroupLink = listDebitCodeGroupLink.Find(x => x.CodeChartOfAccount == ledger.CreditCode);
        List<string> crebitCodeGroupLink = listDebitCodeGroupLink.Where(x => x.CodeChartOfAccountGroup == debitCodeGroupLink?.CodeChartOfAccountGroup).Select(x => x.CodeChartOfAccount).ToList();
        string account6Code = crebitCodeGroupLink.Find(x => x.Substring(0, 1) == "6");
        //string account1Code = crebitCodeGroupLink.Find(x => x.Substring(0, 3) == "155");
        ChartOfAccount account6 = listCoaDebitDefault.Find(x => x.Code == account6Code);
        ChartOfAccount account1 = listCoaDebitDefault.Find(x => x.Code == goods.Account);

        if (goods.Account.StartsWith("156") &&
            (goods.GoodsType == nameof(GoodsTypeEnum.CB)
                || goods.GoodsType == nameof(GoodsTypeEnum.COMBO)
                || goods.GoodsType == nameof(GoodsTypeEnum.DM)))//cb: CHE BIEN; COMBO
        {
            if (account1 == null)
                account1 = new ChartOfAccount();
            var goodDetails = await _context.GoodDetails.Where(x => x.GoodID == goods.Id).ToListAsync();
            foreach (var goodDetail in goodDetails)
            {
                account1.Code = goodDetail.Account;
                account1.Name = goodDetail.AccountName;
                await AddLedgerXK(ledger, ledger.Month, voucherMonth, maxOriginalVoucher, orderString, account6, account1, goodDetail, year, 3, GoodsTypeEnum.DM);
            }
            return null;
        }
        else
        {
            if (billType == "BSN")
            {
                var item1 = await AddLedgerXK(ledger, ledger.Month, voucherMonth, maxOriginalVoucher, orderString, account6, account1, null, year, 3);
                return item1;
            }
           
           await AddLedgerXK(ledger, ledger.Month, voucherMonth, maxOriginalVoucher, orderString, account6, account1, null, year, 1);
            var item = await AddLedgerXK(ledger, ledger.Month, voucherMonth, maxOriginalVoucher, orderString, account6, account1, null, year, 3);
            return item;
          
        }
    }
    private async Task<Ledger> AddLedgerXK(Ledger ledger, int iMonth,
                string voucherMonth, int maxOriginalVoucher, string orderString,
                ChartOfAccount account6, ChartOfAccount account1, GoodDetail goodDetail, int year, int iMethodExportType = 3, GoodsTypeEnum goodType = GoodsTypeEnum.NONE)
    {
        double dUnitPriceAvg;
        if (goodDetail != null)
            dUnitPriceAvg = await _ledgerService.TinhGiaXuatKho(account1?.Code, goodDetail.Detail1, goodDetail.Detail2, ledger.CreditWarehouse, ledger.OrginalBookDate ?? DateTime.Today, year);
        else
        {
            dUnitPriceAvg = await _ledgerService.TinhGiaXuatKho(account1?.Code, ledger.CreditDetailCodeFirst, ledger.CreditDetailCodeSecond, ledger.CreditWarehouse, ledger.OrginalBookDate ?? DateTime.Today, year, iMethodExportType);
        }
        string ledgerString = JsonConvert.SerializeObject(ledger);
        Ledger item = JsonConvert.DeserializeObject<Ledger>(ledgerString);
        item.Id = 0;
        item.BillId = ledger.BillId;
        item.Type = "XK";
        item.Month = iMonth;
        item.OrginalBookDate = ledger.OrginalBookDate;
        item.OrginalDescription = string.IsNullOrEmpty(_bill.DescriptionForLedger) ? $"Xuất kho giá vốn theo Bill số {_bill.BillNumber} ngày {_bill.CreatedDate.ToString("dd/MM/yyyy")}" : _bill.DescriptionForLedger;
        item.DebitCode = account6?.Code;
        item.DebitCodeName = account6?.Name;
        item.DebitDetailCodeFirst = item.CreditDetailCodeFirst;
        item.DebitDetailCodeFirstName = item.CreditDetailCodeFirstName;
        item.DebitDetailCodeSecond = item.CreditDetailCodeSecond;
        item.DebitDetailCodeSecondName = item.CreditDetailCodeSecondName;

        item.CreditCode = account1?.Code;
        item.CreditCodeName = account1?.Name;

        item.VoucherNumber = voucherMonth + "/" + "XK";
        item.OrginalVoucherNumber = $"XK{voucherMonth}-{year.ToString().Substring(2, 2)}-{orderString}";
        item.Order = maxOriginalVoucher;
        item.UnitPrice = dUnitPriceAvg;
        item.Amount = dUnitPriceAvg * (ledger.Quantity);
        item.InvoiceCode = "";
        item.InvoiceSerial = "";
        item.InvoiceNumber = "";

        item.InvoiceTaxCode = "";
        item.InvoiceName = "";
        item.InvoiceAddress = "";
        if (goodDetail != null)
        {
            item.CreditDetailCodeFirst = goodDetail.Detail1;
            item.CreditDetailCodeFirstName = goodDetail.DetailName1;
            item.CreditDetailCodeSecond = goodDetail.Detail2;
            item.CreditDetailCodeSecondName = goodDetail.DetailName2;
            item.Quantity = (goodDetail.Quantity ?? 0) * (ledger.Quantity);
            item.Amount = dUnitPriceAvg * (goodDetail.Quantity ?? 0) * (ledger.Quantity);
        }
        if (iMethodExportType == 3)
        {
            item.IsInternal = 3;
        }
        else
        {
            item.IsInternal = 1;
        }
         
        if (item.DebitCode != null && item.CreditCode != null)
        {
            if (goodType != GoodsTypeEnum.NONE)
            {
                var current = item.IsInternal;
                item.IsInternal = 1;
                await _ledgerService.Create(item, year);
                item.IsInternal = current;
                item.Id = 0;
            }
            
            await _ledgerService.Create(item, year);
        }
        return item;
    }

    private async Task AddLedgerVat(Ledger ledger, int year)
    {
        var accountPayVAT = _accountPays.Find(x => x.Code == "VAT");

        string ledgerString = JsonConvert.SerializeObject(ledger);
        Ledger ledgerVat = JsonConvert.DeserializeObject<Ledger>(ledgerString);
        ledgerVat.Id = 0;
        ledgerVat.DebitCode = "1111";
        ledgerVat.CreditCode = accountPayVAT.Account;

        ledgerVat.CreditCodeName = _chartOfAcounts.Find(x => x.Code == ledger.CreditCode)?.Name;

        ledgerVat.CreditDetailCodeFirst = accountPayVAT.Detail1;
        ledgerVat.CreditDetailCodeFirstName = _chartOfAcounts.Find(x => x.Code == ledger.CreditDetailCodeFirst && x.ParentRef == ledger.CreditCode)?.Name;
        if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
        {
            ledgerVat.CreditDetailCodeSecond = accountPayVAT.Detail2;
            ledgerVat.CreditDetailCodeSecondName = _chartOfAcounts.Find(x => x.ParentRef.Contains(ledger.CreditDetailCodeFirst) && x.Code == ledger.CreditDetailCodeSecond)?.Name;
        }
        ledgerVat.Quantity = 0;
        ledgerVat.UnitPrice = 0;

        ledgerVat.Amount = _bill.Vat > 0 ? _bill.Vat : _billDetails.Sum(x => x.TaxVAT * x.Quantity);

        ledgerVat.OrginalDescription = "Thuế Giá trị gia tăng đầu ra";

        ledgerVat = MapDebitFromAccountPay(ledgerVat);
        if (_customer != null)
        {
            ledgerVat.OrginalCompanyName = _customer.Name;
            ledgerVat.OrginalAddress = _customer.Address;
        }
        else
        {
            ledgerVat.OrginalAddress = "";
            ledgerVat.OrginalCompanyName = "Khách hàng online";
        }

        if (ledgerVat.DebitCode != null && ledgerVat.CreditCode != null)
        {
            await _ledgerService.Create(ledgerVat, year);
        }
    }
}
