using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services.ProduceProductServices;
public class ProduceProductLedgerService: IProduceProductLedgerService
{
    private readonly ApplicationDbContext _context;
    private readonly ILedgerService _ledgerServices;
    private readonly IBillPromotionService _billPromotionService;
    private int _customerId;
    private List<OrderProduceProductDetail> _orderProduceProductDetails;

    public ProduceProductLedgerService(ApplicationDbContext context, ILedgerService ledgerServices, IBillPromotionService billPromotionService)
    {
        _context = context;
        _ledgerServices = ledgerServices;
        _billPromotionService = billPromotionService;
    }

    public async Task<bool> AddLedgerDebitFromOrderProduct(int orderProduceProductId, int year)
    {
        var orderProduceProduct = await _context.OrderProduceProducts.FindAsync(orderProduceProductId);
        _orderProduceProductDetails = await _context.OrderProduceProductDetails.Where(x => x.OrderProduceProductId == orderProduceProductId).ToListAsync();
        _customerId = orderProduceProduct.CustomerId;
        return await AddLedgerDebit(orderProduceProductId, nameof(OrderProduceProduct), year);
    }
    public async Task AddLedgerDebitFromPlanningProduct(int planningProduceProductId, int carId, string carName, int year)
    {
        var planningDetails = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == planningProduceProductId 
                                            && x.CarId == carId && x.CarName == carName).ToListAsync();

        var orderProduceProductIds = planningDetails.Select(x => x.OrderProduceProductId).Distinct();
        foreach(var orderProduceProductId in orderProduceProductIds)
        {
            if (orderProduceProductId is null)
            {
                continue;
            }

            var planningDetailFinds = planningDetails.Where(x => x.OrderProduceProductId .Equals(orderProduceProductId)).ToList();
            var planningDetailFindIds = planningDetailFinds.Select(x => x.OrderProduceProductDetailId);
            _orderProduceProductDetails = await _context.OrderProduceProductDetails.Where(x => planningDetailFindIds.Contains(x.Id)).ToListAsync();
            _customerId = planningDetailFinds.FirstOrDefault().CustomerId;
           
            await AddLedgerDebit(orderProduceProductId ?? 0, nameof(PlanningProduceProduct), year);
        }
    }
    private async Task<bool> AddLedgerDebit(int tableId, string tableName, int year)
    {
        var listChartOfAcc = await _context.GetChartOfAccount(year).ToListAsync();
        var listWareHouse = await _context.Warehouses.ToListAsync();
        string typePayLedger = "XK";
        int maxOriginalVoucher = 0;
        int monthLed = DateTime.Today.Month;
        string orginalVoucherNumber = "";
        string voucherMonth = (monthLed < 10 ? "0" + monthLed : monthLed.ToString());

        var ledgerExist = await _context.GetLedger(year, 1).AsNoTracking().Where(x => !x.IsDelete && x.Type == typePayLedger
                                                            && x.OrginalBookDate.Value.Year == DateTime.Today.Year && x.OrginalBookDate.Value.Month == DateTime.Today.Month).ToListAsync();
        if (ledgerExist != null && ledgerExist.Count > 0)
        {
            maxOriginalVoucher = ledgerExist.Max(x => Int32.Parse(x.OrginalVoucherNumber.Split('-').Last()));
        }
        maxOriginalVoucher++;

        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);

        orginalVoucherNumber = $"{typePayLedger}{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";
        var accountLinks = await _context.GetChartOfAccountGroupLink(year).ToListAsync();

        var accountPays = await _context.AccountPays.ToListAsync();
        var khachHang = await _context.Customers.FindAsync(_customerId);
        var khachHang_tax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == _customerId);

        Ledger ledger = null;
        foreach (var requests in _orderProduceProductDetails)
        {
            var good = await _context.Goods.FindAsync(requests.GoodsId);

            ledger = new Ledger();
            ledger.Type = typePayLedger;
            ledger.Month = monthLed;
            ledger.BookDate = DateTime.Today;
            ledger.VoucherNumber = voucherMonth + "/" + ledger.Type;
            ledger.IsVoucher = false;

            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.Order = maxOriginalVoucher;

            ledger.OrginalBookDate = DateTime.Today;
            ledger.ReferenceBookDate = DateTime.Today;
            ledger.InvoiceDate = DateTime.Today;
            ledger.CreditCode = "5111";

            var accountGroup = accountLinks.Find(x => x.CodeChartOfAccount == good.Account);
            if (accountGroup != null)
            {
                var accountGroupS = accountLinks.Find(x => x.CodeChartOfAccountGroup == accountGroup.CodeChartOfAccountGroup && x.CodeChartOfAccount.Substring(0, 1) == "5")?.CodeChartOfAccount;
                var creditacc = listChartOfAcc.Find(x => x.Code == accountGroupS);
                if (creditacc != null)
                    ledger.CreditCode = creditacc.Code;
            }

            ledger.CreditCodeName = listChartOfAcc.Find(x => x.Code == ledger.CreditCode)?.Name;
            ledger.CreditWarehouse = good.Warehouse;
            ledger.CreditWarehouseName = listWareHouse.Find(x => x.Code == ledger.CreditWarehouse)?.Name;

            ledger.CreditDetailCodeFirst = good.Detail1;
            ledger.CreditDetailCodeFirstName = listChartOfAcc.Find(x => x.Code == ledger.CreditDetailCodeFirst && x.ParentRef == ledger.CreditCode)?.Name;
            if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
            {
                ledger.CreditDetailCodeSecond = good.Detail2;
                ledger.CreditDetailCodeSecondName = listChartOfAcc.Find(x => x.ParentRef.Contains(ledger.CreditDetailCodeFirst) && x.Code == good.Detail2)?.Name;
            }
            ledger.Quantity = requests.QuantityReal;
            ledger.UnitPrice = requests.UnitPrice;
            ledger.Amount = requests.QuantityReal * ledger.UnitPrice;
            ledger.CreateAt = DateTime.Now;
            ledger.OrginalCode = "";
            ledger.OrginalFullName = "";
            string maHang = "";
            if (!string.IsNullOrEmpty(ledger.CreditDetailCodeSecondName))
                maHang = ledger.CreditDetailCodeSecondName;
            else if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirstName))
                maHang = ledger.CreditDetailCodeFirstName;
            else
                maHang = ledger.CreditCodeName;

            ledger.OrginalDescription = "Doanh thu bán " + maHang;
            if (good.GoodsType == nameof(GoodsTypeEnum.COMBO))
            {
                ledger.OrginalDescription = "Xuất bán Combo " + maHang + " khuyến mãi";
            }

            ledger.OrginalDescriptionEN = "";

            ledger.AttachVoucher = "";

            ledger.DebitCode = khachHang.DebitCode;
            ledger.DebitDetailCodeFirst = khachHang.DebitDetailCodeFirst;
            ledger.DebitDetailCodeFirstName = listChartOfAcc.Find(x => x.Code == ledger.DebitDetailCodeFirst && x.ParentRef == ledger.DebitCode)?.Name;
            if (!string.IsNullOrEmpty(ledger.DebitCode))
            {
                ledger.DebitDetailCodeSecond = khachHang.DebitDetailCodeSecond;
                ledger.DebitDetailCodeSecondName = listChartOfAcc.Find(x => x.ParentRef.Contains(ledger.DebitDetailCodeFirst) && x.Code == ledger.DebitDetailCodeSecond)?.Name;
            }

            ledger.OrginalCompanyName = khachHang.Name;
            ledger.OrginalAddress = khachHang.Address;

            ledger.DebitCodeName = listChartOfAcc.Find(x => x.Code == ledger.DebitCode)?.Name;

            ledger.ReferenceVoucherNumber = "";
            ledger.ReferenceFullName = "";
            ledger.ReferenceAddress = "";
            ledger.InvoiceAdditionalDeclarationCode = "BT";
            ledger.InvoiceProductItem = "";
            ledger.ProjectCode = "";
            ledger.IsInternal = 1;

            await _ledgerServices.Create(ledger, year);
        }


        var billPromotions = await _billPromotionService.Get(tableId, tableName);

        if (billPromotions.Any())
        {
            var accountPayVAT = accountPays.Find(x => x.Code == "VAT");

            string ledgerString = JsonConvert.SerializeObject(ledger);
            Ledger ledgerVat = JsonConvert.DeserializeObject<Ledger>(ledgerString);
            ledgerVat.Id = 0;
            ledgerVat.CreditCode = accountPayVAT.Account;
            ledgerVat.CreditCodeName = listChartOfAcc.Find(x => x.Code == ledger.CreditCode)?.Name;
            ledgerVat.CreditDetailCodeFirst = accountPayVAT.Detail1;
            ledgerVat.CreditDetailCodeFirstName = listChartOfAcc.Find(x => x.Code == ledger.CreditDetailCodeFirst && x.ParentRef == ledger.CreditCode)?.Name;
            if (!string.IsNullOrEmpty(ledger.CreditDetailCodeFirst))
            {
                ledgerVat.CreditDetailCodeSecond = accountPayVAT.Detail2;
                ledgerVat.CreditDetailCodeSecondName = listChartOfAcc.Find(x => x.ParentRef.Contains(ledger.CreditDetailCodeFirst) && x.Code == ledger.CreditDetailCodeSecond)?.Name;
            }
            ledgerVat.Quantity = 0;
            ledgerVat.UnitPrice = 0;

            ledgerVat.Amount = billPromotions.Sum(x => x.Amount);

            ledgerVat.OrginalDescription = "Thuế Giá trị gia tăng đầu ra";

            ledgerVat.DebitCode = khachHang.DebitCode;
            ledgerVat.DebitDetailCodeFirst = khachHang.DebitDetailCodeFirst;
            ledgerVat.DebitDetailCodeFirstName = listChartOfAcc.Find(x => x.Code == ledger.DebitDetailCodeFirst && x.ParentRef == ledger.DebitCode)?.Name;
            if (!string.IsNullOrEmpty(ledger.DebitCode))
            {
                ledgerVat.DebitDetailCodeSecond = khachHang.DebitDetailCodeSecond;
                ledgerVat.DebitDetailCodeSecondName = listChartOfAcc.Find(x => x.ParentRef.Contains(ledger.DebitDetailCodeFirst) && x.Code == ledger.DebitDetailCodeSecond)?.Name;
            }

            ledgerVat.OrginalCompanyName = khachHang.Name;
            ledgerVat.OrginalAddress = khachHang.Address;

            ledgerVat.DebitCodeName = listChartOfAcc.Find(x => x.Code == ledger.DebitCode)?.Name;

            await _ledgerServices.Create(ledgerVat, year);
        }
        return true;
    }

}
