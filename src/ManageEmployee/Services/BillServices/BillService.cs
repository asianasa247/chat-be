using AutoMapper;
using Common.Constants;
using Common.Errors;
using Emgu.CV;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Dal.Enums;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.InvoiceModel.WinInvoice;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Helpers;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OfficeOpenXml;
using System.Net.Http.Headers;
using System.Text;

namespace ManageEmployee.Services.BillServices;


public class BillService : IBillService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IBillTrackingService _billTrackingService;
    private readonly IBillDetailService _billDetailService;
    private readonly IBillPromotionService _billPromotionService;
    private readonly WinInvoiceConfig _winInvoiceConfig;

    public BillService(
        ApplicationDbContext context,
        IMapper mapper,
        IBillTrackingService billTrackingService,
        IBillDetailService billDetailService,
        IBillPromotionService billPromotionService,
        IOptions<WinInvoiceConfig> winInvoiceConfig
    )
    {
        _context = context;
        _mapper = mapper;
        _billTrackingService = billTrackingService;
        _billDetailService = billDetailService;
        _billPromotionService = billPromotionService;
        _winInvoiceConfig = winInvoiceConfig.Value;
    }

    public async Task<PagingResult<BillPaging>> GetAll(BillRequestModel param, bool isExportDetail = false)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;
            List<int> billIds = new List<int>();
            if (param.GoodId > 0)
            {
                billIds = _context.BillDetails.Where(X => X.GoodsId == param.GoodId).Select(x => x.BillId)
                    .Distinct().ToList();
            }

            if (param.StartDate != null)
                param.StartDate = param.StartDate.Value.Date;
            if (param.EndDate != null)
                param.EndDate = param.EndDate.Value.AddDays(1).Date;
            List<BillPaging> categories = (from p in _context.Bills
                                           join cus in _context.Customers on p.CustomerId equals cus.Id into _cus
                                           from customer in _cus.DefaultIfEmpty()

                                           join user in _context.Users on p.UserCode equals user.Username into _user
                                           from user in _user.DefaultIfEmpty()
                                           where !p.Status.Contains(TranTypeConst.Cancel) && !p.IsDeleted &&
                    (string.IsNullOrEmpty(param.SearchText) || p.UserCode.Trim().Contains(param.SearchText))
                                     && (param.StartDate == null || p.CreatedDate >= param.StartDate)
                                     && (param.EndDate == null || p.CreatedDate < param.EndDate)
                                     && (param.CustomerId == null || p.CustomerId == param.CustomerId)
                                     && (billIds.Count == 0 || billIds.Contains(p.Id))
                                     && (string.IsNullOrEmpty(param.TypePay) || p.TypePay == param.TypePay)
                                           select new BillPaging
                                           {
                                               Id = p.Id,
                                               CustomerId = p.CustomerId,
                                               CustomerName = customer.Name,
                                               Status = p.Status.Contains(TranTypeConst.Cancel) ? "Đơn hàng bị hủy" :
                                                           p.Status.Contains(TranTypeConst.Paid) ? "Lưu thành công" : "Chưa lưu",
                                               UserCode = p.UserCode,
                                               DeskId = p.DeskId,
                                               FloorId = p.FloorId,
                                               QuantityCustomer = p.QuantityCustomer,
                                               TotalAmount = p.TotalAmount,
                                               AmountReceivedByCus = p.AmountReceivedByCus,
                                               AmountSendToCus = p.AmountSendToCus,
                                               DiscountPrice = p.DiscountPrice,
                                               DiscountType = p.DiscountType,
                                               Note = p.Note,
                                               CreatedDate = p.CreatedDate,
                                               TotalAmountCN = p.TypePay == "CN" ? p.TotalAmount : 0,
                                               DisplayOrder = $"Bill {p.DisplayOrder} - {p.BillNumber}",
                                               InvoiceNumber = string.IsNullOrEmpty(p.InvoiceNumber) ? "Chưa xuất hóa đơn" : p.InvoiceNumber,
                                               TypePay = p.TypePay,
                                               UserName = user.FullName
                                           }).OrderByDescending(x => x.CreatedDate).ToList();
            if (isExportDetail)
            {
                return new PagingResult<BillPaging>()
                {
                    CurrentPage = param.Page,
                    PageSize = param.PageSize,
                    TotalItems = categories.Count,
                    Data = categories
                };
            }

            var listOut = new List<BillPaging>();

            int quantityCustomer = categories.Sum(x => x.QuantityCustomer);
            double totalAmount = categories.Sum(x => x.TotalAmount);
            BillPaging itemOut = new BillPaging();
            itemOut.QuantityCustomer = quantityCustomer;
            itemOut.TotalAmount = totalAmount;
            itemOut.TotalCustomer = categories.Select(x => x.CustomerId).Distinct().Count();
            listOut.Add(itemOut);
            if (param.Page > 0)
                listOut.AddRange(categories.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToList());
            else
                listOut.AddRange(categories);

            foreach (var item in listOut)
            {
                var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == item.CustomerId);
                item.CustomerAddress = customerTax?.Address;
                item.CustomerTaxCode = customerTax?.TaxCode;
                var isExistLedger = await _context.Ledgers.AnyAsync(x => x.BillId == item.Id);
                if (!isExistLedger)
                {
                    item.DisplayOrder = $"{item.DisplayOrder} (Xóa)";
                }
            }
            return new PagingResult<BillPaging>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = categories.Count + 1,
                Data = listOut
            };
        }
        catch
        {
            return new PagingResult<BillPaging>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<BillPaging>()
            };
        }
    }

    public async Task<BillModel> GetById(int id)
    {
        var bill = await _context.Bills.FindAsync(id);
        var billModel = _mapper.Map<BillModel>(bill);
        billModel.BillPromotions = await _billPromotionService.Get(id, nameof(Bill));
        return billModel;
    }


    public async Task<(Bill bill, List<BillDetailViewPaging> billDetails)>
        GetBillPdfByIdAsync(int billId)
    {
        var bill = await _context.Bills.AsNoTracking().FirstOrDefaultAsync(x => x.Id == billId);
        var billDetails = await _billDetailService.GetListByBillId(billId);
        return (bill, billDetails);
    }
    private static string RemoveAllLetters(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "[^0-9]", "");
    }
    public async Task<Bill> Create(BillModel requests, int? orderProduceProductId)
    {
        if (string.IsNullOrEmpty(requests.BillNumber))
        {
            var billNumber = await GetBillTrackingOrder("BSN");
            requests.Type = "BSN";
            requests.BillNumber = billNumber.Item2;
            requests.DisplayOrder = Convert.ToInt32(billNumber.Item1);
        }
        if (requests.IsPayment)
        {
            requests.Status = TranTypeConst.Paid;
        }
        var billCheck = await _context.Bills.FirstOrDefaultAsync(x => x.DisplayOrder == requests.DisplayOrder && x.BillNumber == requests.BillNumber );
        if (billCheck != null)
        {
            var prefix = new string(requests.BillNumber.TakeWhile(c => !char.IsDigit(c)).ToArray());
            var numberPart = requests.BillNumber.Replace(prefix, "");
            string newBillNumber = $"{prefix}{numberPart + 1}";
            // Gán lại bill number mới
            requests.BillNumber = newBillNumber;
        }
        await _context.Database.BeginTransactionAsync();

        try
        {

            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == requests.CustomerId);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == requests.UserCode);
            Bill billModel = new()
            {
                DeskId = requests.DeskId,
                FloorId = requests.FloorId,
                UserCode = requests.UserCode,
                CustomerId = requests.CustomerId,
                CustomerName = customer?.Name,
                QuantityCustomer = requests.QuantityCustomer,
                TotalAmount = requests.TotalAmount,
                DiscountPrice = requests.DiscountPrice,
                DiscountType = requests.DiscountType,
                AmountReceivedByCus = requests.AmountReceivedByCus,
                AmountSendToCus = requests.AmountSendToCus,
                Note = requests.Note,
                Status = string.IsNullOrEmpty(requests.Status) ? TranTypeConst.Waiting : requests.Status,
                TypePay = requests.TypePay,
                DisplayOrder = requests.DisplayOrder,
                IsPrintBill = requests.IsPrintBill,
                IsPriority = requests.IsPriority,
                UserCreated = requests.UserCreated,
                BillNumber = requests.BillNumber,
                Type = requests.Type,
                Vat = requests.Vat,
                VatRate = requests.VatRate,
                VatCode = requests.VatCode,
                DescriptionForLedger = requests.DescriptionForLedger,
                Date = requests.Date ?? DateTime.Today,
                OrderProduceProductId = orderProduceProductId,
                ProjectCode = requests.ProjectCode,
                BranchId = user?.BranchId
            };

            _context.Bills.Add(billModel);
            await _context.SaveChangesAsync();

            if (requests.TypePay == "TM" && requests.IsPayment)
            {
                var till = await _context.TillManagers.FirstOrDefaultAsync(x =>
                    x.UserId == requests.UserCreated && !x.IsFinish);
                if (till is not null)
                {
                    till.ToAmountAuto = till.ToAmountAuto + requests.TotalAmount - 
                                        (requests.DiscountType == "percent" ? (requests.DiscountPrice * requests.TotalAmount/100) : requests.DiscountPrice);
                    _context.TillManagers.Update(till);
                }
            }
            // add promotions
            await _billPromotionService.Create(requests.BillPromotions, billModel.Id, nameof(Bill));
            await _context.SaveChangesAsync();

            // Create Bill Tracking after create Bill success
            await _billTrackingService.Create(requests, billModel);
            await _context.Database.CommitTransactionAsync();

            return billModel;
        }
        catch (Exception ex)
        {
            await _context.Database.RollbackTransactionAsync();

            throw new ErrorException(ex.Message);
        }
    }

    public async Task<Bill> Update(BillModel requests)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var bill = await _context.Bills.SingleOrDefaultAsync(x => x.Id == requests.Id && !x.IsDeleted);
            if (bill == null)
            {
                throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
            }
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == requests.UserCode);

            bill.CustomerId = requests.CustomerId;
            bill.CustomerName = requests.CustomerName;
            bill.QuantityCustomer = requests.QuantityCustomer;
            bill.TotalAmount = requests.TotalAmount;
            bill.DiscountPrice = requests.DiscountPrice;
            bill.DiscountType = requests.DiscountType;
            bill.UserCode = requests.UserCode;
            bill.DeskId = requests.DeskId;
            bill.FloorId = requests.FloorId;
            bill.AmountReceivedByCus = requests.AmountReceivedByCus;
            bill.AmountSendToCus = requests.AmountSendToCus;
            bill.Note = requests.Note;
            bill.UpdatedDate = DateTime.Now;
            bill.TypePay = requests.TypePay;
            bill.IsPriority = requests.IsPriority;
            bill.Vat = requests.Vat;
            bill.VatCode = requests.VatCode;
            bill.VatRate = requests.VatRate;
            bill.BillNumber = requests.BillNumber;
            bill.ProjectCode = requests.ProjectCode;
            bill.BranchId = user?.BranchId;

            string status = "";
            if (!bill.Status.Contains(TranTypeConst.SendToCashier))
            {
                status = TranTypeConst.SendToCashier;
            }
            bill.Status = bill.Status.Replace(TranTypeConst.Paid, status);

            _context.Bills.Update(bill);

            // update billTracking
            var billTrackings = await _context.BillTrackings.Where(X => X.BillId == requests.Id).ToListAsync();
            billTrackings = billTrackings.ConvertAll(x =>
            {
                x.TranType = x.TranType.Replace(TranTypeConst.Paid, status);
                x.UpdateAt = DateTime.Now;
                return x;
            });
            _context.BillTrackings.UpdateRange(billTrackings);

            // add promotions
            await _billPromotionService.Create(requests.BillPromotions, requests.Id, nameof(Bill));


            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return bill;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public void Delete(int id)
    {
        var category = _context.Bills.Find(id);
        if (category != null)
        {
            category.IsDeleted = true;
            _context.Bills.Update(category);
            _context.SaveChanges();
        }
    }

    public async Task<Bill> CreateBillEmployee(BillModel requests)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            Bill billModel = new Bill
            {
                DeskId = requests.DeskId,
                FloorId = requests.FloorId,
                UserCode = requests.UserCode,
                CustomerId = requests.CustomerId,
                CustomerName = requests.CustomerName,
                QuantityCustomer = requests.QuantityCustomer,
                TotalAmount = requests.TotalAmount,
                DiscountPrice = requests.DiscountPrice,
                DiscountType = requests.DiscountType,
                AmountReceivedByCus = requests.AmountReceivedByCus,
                AmountSendToCus = requests.AmountSendToCus,
                Note = requests.Note,
                Status = requests.IsPayment ? "Paid" : "Waiting",
                TypePay = requests.TypePay,
            };
            _context.Bills.Add(billModel);
            _context.SaveChanges();

            if (requests.Products != null)
            {
                foreach (var product in requests.Products)
                {
                    BillDetail billDetailModel = new BillDetail
                    {
                        BillId = billModel.Id,
                        GoodsId = product.GoodsId,
                        Quantity = product.Quantity,
                        UnitPrice = product.UnitPrice ?? 0,
                        DiscountPrice = product.DiscountPrice ?? 0,
                        DiscountType = requests.DiscountType,
                        TaxVAT = product.TaxVAT ?? 0,
                        Note = product.Note,
                    };
                    _context.BillDetails.Add(billDetailModel);
                }
            }

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            return billModel;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public long GetBillTrackingOrder()
    {
        var bill = _context.Bills.Where(x => x.CreatedDate.Month == DateTime.Today.Month && x.IsDeleted == false)
            .OrderByDescending(x => x.Id).FirstOrDefault();
        if (bill == null)
        {
            return 1;
        }

        return (bill.DisplayOrder ?? 0) + 1;
    }


    public async Task<(long, string)> GetBillTrackingOrder(string billType, List<int> excludeBillIds = null)
    {
        if (excludeBillIds == null)
        {
            excludeBillIds = new List<int>();
        }

        var billOrder = (await _context.Bills.Where(x =>
                x.CreatedDate.Year == DateTime.Today.Year
                && !string.IsNullOrEmpty(billType)
                && x.Type == billType
                && !string.IsNullOrEmpty(x.BillNumber)
                && !excludeBillIds.Contains(x.Id)
                && !x.IsDeleted)
            .Select(x => x.DisplayOrder)
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync()).GetValueOrDefault() + 1;
        // Example HD2307000001
        return (billOrder, $"{billType}{DateTime.Now:yyMM}{billOrder.ToString().PadLeft(6, '0')}");
    }

    public async Task<GoodForCustomeViewModel> GetGoodForCustomer(BillReportRequestModel param)
    {
        var datas = await (from p in _context.BillDetails
                           join g in _context.Goods on p.GoodsId equals g.Id
                           join b in _context.Bills on p.BillId equals b.Id
                           join c in _context.Customers on b.CustomerId equals c.Id into _cus
                           from c in _cus.DefaultIfEmpty()
                           join u in _context.Users on b.UserCode equals u.Username into _user
                           from u in _user.DefaultIfEmpty()
                           where (param.StartDate != null ? b.CreatedDate >= param.StartDate : true)
                            && (param.EndDate != null ? b.CreatedDate < param.EndDate.Value.AddDays(1) : true)
                            && (param.CustomerId != null ? b.CustomerId == param.CustomerId : true)
                            && (string.IsNullOrEmpty(param.Detail1) || g.Detail1 == param.Detail1)
                            && (param.UserId > 0 ? u.Id == param.UserId : true)
                           select new GoodForCustomeViewModel
                           {
                               CustomerId = param.Type == 0 ? b.CustomerId : 0,
                               CustomerCode = param.Type == 0 ? c.Code : "",
                               CustomerName = param.Type == 0 ? b.CustomerName : "",
                               UserCode = param.Type == 2 ? u.Username : "",
                               UserName = param.Type == 2 ? u.FullName : "",

                               Quantity = p.Quantity,
                               Amount = p.Quantity * p.UnitPrice,
                               Detail1 = g.Detail1,
                               Detail1Name = g.DetailName1,
                               GoodCode = !string.IsNullOrEmpty(g.Detail2) ? g.Detail2 : g.Detail1,
                               GoodName = !string.IsNullOrEmpty(g.DetailName2) ? g.DetailName2 : g.DetailName1,
                               Account = g.Account,
                               AccountName = g.AccountName,
                               StockUnit = g.StockUnit,

                               AmountBack = 0,
                               QuantityBack = 0,
                               AmountProfit = p.Quantity * p.UnitPrice,
                           }).GroupBy(x => x.Detail1)
                                       .Select(x => new GoodForCustomeViewModel
                                       {
                                           Detail1 = x.Key,
                                           Detail1Name = x.First().Detail1Name,
                                           CustomerCode = param.Type == 0 ? x.First().CustomerCode : "",
                                           CustomerName = param.Type == 0 ? x.First().CustomerName : "",
                                           UserCode = param.Type == 2 ? x.First().UserCode : "",
                                           UserName = param.Type == 2 ? x.First().UserName : "",

                                           Items = x.ToList(),
                                           Amount = x.Sum(x => x.Amount),
                                           Quantity = x.Sum(x => x.Quantity),
                                       }).ToListAsync();
        foreach (var data in datas)
        {
            var detail2s = data.Items.Select(x => x.GoodCode).Distinct().ToList();
            var items = new List<GoodForCustomeViewModel>();
            foreach (var detail2 in detail2s)
            {
                var amount = data.Items.Where(x => x.GoodCode == detail2).Sum(x => x.Amount);
                var quantity = data.Items.Where(x => x.GoodCode == detail2).Sum(x => x.Quantity);
                items.Add(data.Items.Where(x => x.GoodCode == detail2)
                    .Select(x => { x.Amount = amount; x.Quantity = quantity; return x; }).FirstOrDefault());
            }
            data.Items = items;
        }
        if (datas.Count > 0)
        {
            GoodForCustomeViewModel itemOut = new GoodForCustomeViewModel
            {
                CustomerId = 0,
                CustomerCode = param.Type == 0 ? datas.First().CustomerCode : "",
                CustomerName = param.Type == 0 ? datas.First().CustomerName : "",
                UserCode = param.Type == 2 ? datas.First().UserCode : "",
                UserName = param.Type == 2 ? datas.First().UserName : "",

                Quantity = datas.Sum(x => x.Quantity),
                Amount = datas.Sum(x => x.Amount),
                Note = "",
                Detail1 = "",
                Detail1Name = "",
                GoodCode = "",
                GoodName = "",
                Account = "",
                AccountName = "",
                StockUnit = "",
                AmountBack = 0,
                QuantityBack = 0,
                AmountProfit = datas.Sum(x => x.AmountProfit),
                Items = datas
            };
            return itemOut;
        }
        return new GoodForCustomeViewModel();
    }


    public BillForCustomerInvoice GetBillForCustomerInvoice(int id)
    {
        var bill = _context.Bills.Find(id);
        var billDetails = _context.BillDetails.Where(x => x.BillId == id);
        var customer = _context.CustomerTaxInformations.Find(bill?.CustomerId);
        return new BillForCustomerInvoice
        {
            Bill = bill,
            BillDetails = billDetails,
            Customer = customer ?? new CustomerTaxInformation()
        };
    }
    public async Task UpdateCustomerInvoice(CustomerTaxInformation customer, int statusPrintInvoice, int billId, int year)
    {
        if (statusPrintInvoice == 3)//da xuat hoa don
        {
            var bill = await _context.Bills.FindAsync(billId);

            var invoiceNumber = _context.GetLedger(year, 2).AsNoTracking().OrderByDescending(x => x.InvoiceNumber).FirstOrDefault()?.InvoiceNumber ?? "";
            int invoiceNumberMax = 1;

            if (!string.IsNullOrEmpty(invoiceNumber))
                invoiceNumberMax = int.Parse(invoiceNumber) + 1;
            invoiceNumber = invoiceNumberMax.ToString();
            while (true)
            {
                if (invoiceNumber.Length >= 7)
                    break;
                invoiceNumber = "0" + invoiceNumber;
            }
            bill.InvoiceNumber = invoiceNumber;
            _context.Bills.Update(bill);
            var invoiceDeclaration = await _context.InvoiceDeclarations.FirstOrDefaultAsync(x => x.Name == "R01" && x.Month == DateTime.Now.Month);
            var khachHang_tax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == bill.CustomerId);

            var ledgers = _context.GetLedger(year, 2).Where(x => x.BillId == billId).ToList()
                        .Select(x =>
                        {
                            x.InvoiceCode = invoiceDeclaration.Name;
                            x.InvoiceSerial = invoiceDeclaration.TemplateSymbol;
                            x.InvoiceNumber = invoiceNumber;
                            x.InvoiceTaxCode = customer?.TaxCode;
                            x.InvoiceName = customer?.CompanyName;
                            x.InvoiceAddress = customer?.Address;
                            return x;
                        });
            _context.Ledgers.UpdateRange(ledgers);

        }
        var customerFind = _context.CustomerTaxInformations.FirstOrDefault(x => x.TaxCode == customer.TaxCode);
        if (customerFind == null)
        {
            customerFind = customer;
            customerFind.Id = 0;
            _context.CustomerTaxInformations.Add(customer);
        }
        await _context.SaveChangesAsync();
    }
    public async Task<BillForCustomerInvoice> GetInvoiceForBill(int billId, int year)
    {
        var bill = await _context.Bills.FindAsync(billId);
        if (bill is null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
        }
        if (string.IsNullOrEmpty(bill.InvoiceNumber))
        {
            var invoiceNumber = _context.GetLedger(year, 2).AsNoTracking().OrderByDescending(x => x.InvoiceNumber).FirstOrDefault()?.InvoiceNumber ?? "";
            int invoiceNumberMax = 1;

            if (!string.IsNullOrEmpty(invoiceNumber))
                invoiceNumberMax = int.Parse(invoiceNumber) + 1;
            invoiceNumber = invoiceNumberMax.ToString();
            while (true)
            {
                if (invoiceNumber.Length >= 7)
                    break;
                invoiceNumber = "0" + invoiceNumber;
            }
            bill.InvoiceNumber = invoiceNumber;
            _context.Bills.Update(bill);
            var invoiceDeclaration = await _context.InvoiceDeclarations.FirstOrDefaultAsync(x => x.Name == "R01" && x.Month == DateTime.Now.Month);

            var ledgers = _context.GetLedger(year, 2).Where(x => x.BillId == billId).ToList()
                        .Select(x =>
                        {
                            x.InvoiceCode = invoiceDeclaration.Name;
                            x.InvoiceSerial = invoiceDeclaration.TemplateSymbol;
                            x.InvoiceNumber = invoiceNumber;
                            return x;
                        });
            _context.Ledgers.UpdateRange(ledgers);

        }
        BillForCustomerInvoice itemOut = new BillForCustomerInvoice();
        itemOut.BillDetails = await _context.BillDetails.Where(x => x.BillId == billId).ToListAsync();
        itemOut.Bill = bill;
        itemOut.Customer = new CustomerTaxInformation();
        return itemOut;
    }
    public async Task CopyBill(int billId, int userId)
    {
        try
        {
            var bill = await _context.Bills.FindAsync(billId);
            var billAdd = _mapper.Map<Bill>(bill);
            billAdd.Id = 0;
            bill.UserCreated = userId;
            bill.DisplayOrder = await _context.Bills.Where(x => x.Type == bill.Type).OrderByDescending(x => x.DisplayOrder).Select(x => x.DisplayOrder).FirstOrDefaultAsync() + 1;
            bill.IsPrintBill = false;
            bill.Status = TranTypeConst.SendToCashier;
            bill.CreatedDate = DateTime.Now;
            bill.UpdatedDate = DateTime.Now;
            bill.InvoiceNumber = string.Empty;

            if (!string.IsNullOrEmpty(bill.Type))
            {
                var billNumberFind = await GetBillTrackingOrder(bill.Type);
                bill.BillNumber = billNumberFind.Item2;
            }


            await _context.Bills.AddAsync(billAdd);
            await _context.SaveChangesAsync();
            BillModel billModel = new BillModel();
            billModel.IsPayment = false;
            billModel.UserType = "";
            await _billTrackingService.Create(billModel, billAdd);

            var billDetails = await _context.BillDetails.Where(x => x.BillId == billId).ToListAsync();
            foreach (var billDetail in billDetails)
            {
                var billDetailAdd = _mapper.Map<BillDetail>(billDetail);
                billDetailAdd.Id = 0;
                billDetailAdd.BillId = billAdd.Id;
                await _context.BillDetails.AddAsync(billDetailAdd);
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }

    public async Task UpdateSurCharge(int billId, double surcharge)
    {
        var bill = await _context.Bills.FindAsync(billId);
        if (bill is null)
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
        bill.Surcharge = surcharge;
        _context.Bills.Update(bill);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<SelectListModel>> GetCustomerForReportBill(BillReportRequestModel param)
    {
        if (param.EndDate != null)
            param.EndDate = param.EndDate.Value.AddDays(1);
        var customerIds = await _context.Bills.Where(x => x.CreatedDate >= param.StartDate && x.CreatedDate < param.EndDate && x.CustomerId > 0).Select(x => x.CustomerId).Distinct().ToListAsync();
        return await _context.Customers.Where(x => customerIds.Contains(x.Id)).Select(p => new SelectListModel
        {
            Id = p.Id,
            Code = p.Code,
            Name = p.Code + " - " + p.Name,
        }).ToListAsync();
    }

    public async Task<IEnumerable<SelectListModel>> GetUserForReportBill(BillReportRequestModel param)
    {
        if (param.EndDate != null)
            param.EndDate = param.EndDate.Value.AddDays(1);
        var userCodes = await _context.Bills.Where(x => x.CreatedDate >= param.StartDate && x.CreatedDate < param.EndDate).Select(x => x.UserCode).Distinct().ToListAsync();
        return await _context.Users.Where(x => userCodes.Contains(x.Username)).Select(p => new SelectListModel
        {
            Id = p.Id,
            Name = p.FullName,
        }).ToListAsync();
    }
    public async Task<IEnumerable<SelectListModel>> GetChartOfAccountForReportBill(BillReportRequestModel param)
    {
        if (param.EndDate != null)
            param.EndDate = param.EndDate.Value.AddDays(1);
        var data = await _context.Bills.Where(x => x.CreatedDate >= param.StartDate && x.CreatedDate < param.EndDate)
            .Join(_context.BillDetails,
                    b => b.Id,
                    d => d.BillId,
                    (b, d) => new { goodsId = d.GoodsId })
            .Join(_context.Goods,
                    b => b.goodsId,
                    g => g.Id,
                    (b, g) => new { good = g })
            .Where(x => string.IsNullOrEmpty(param.AccountCode) || x.good.Account == param.AccountCode)
            .Select(x => new SelectListModel
            {
                Id = x.good.Id,
                Code = x.good.Detail1,
                Name = x.good.DetailName1,
            })
            .ToListAsync();

        return data.DistinctBy(x => x.Code);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="type">0: ma ct1- ma ct2, 1: ma ct1- ten ct2, 2: ten ct1- ten ct2, 3: ma hh, 4: ten hh/param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<List<BillDetailImportModel>> ImportBill(string fileName, int type)
    {
        string url = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        StringBuilder message = new StringBuilder();
        List<BillDetailImportModel> billDetails = new List<BillDetailImportModel>();

        List<TaxRate> taxRates = await _context.TaxRates.AsNoTracking().ToListAsync();
        using (FileStream templateDocumentStream = File.OpenRead(url))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int i = 3;

                while (sheet.Cells[i + 1, 1].Value != null)
                {
                    i++;
                    var account = sheet.Cells[i, 2].Value?.ToString();
                    string detail1 = "";
                    string detail1Name = "";
                    string detail2 = "";
                    string detail2Name = "";
                    if (type == 0)
                    {
                        detail1 = sheet.Cells[i, 3].Value?.ToString();
                        detail2 = sheet.Cells[i, 4].Value?.ToString();
                    }
                    else if (type == 1)
                    {
                        detail1 = sheet.Cells[i, 3].Value?.ToString();
                        detail2Name = sheet.Cells[i, 4].Value?.ToString();
                    }
                    else if (type == 2)
                    {
                        detail1Name = sheet.Cells[i, 3].Value?.ToString();
                        detail2Name = sheet.Cells[i, 4].Value?.ToString();
                    }
                    else if (type == 3)
                    {
                        detail2 = sheet.Cells[i, 4].Value?.ToString();
                    }
                    else if (type == 4)
                    {
                        detail2Name = sheet.Cells[i, 4].Value?.ToString();
                    }
                    var good = await _context.Goods.FirstOrDefaultAsync(x => (string.IsNullOrEmpty(account) || x.Account.ToLower().Contains(account.ToLower()))
                                         && (string.IsNullOrEmpty(detail1) || x.Detail1.ToLower().Contains(detail1))
                                         && (string.IsNullOrEmpty(detail1Name) || x.DetailName1.ToLower().Contains(detail1Name))
                                         && (string.IsNullOrEmpty(detail2) || x.Detail2.ToLower().Contains(detail2))
                                         && (string.IsNullOrEmpty(detail2Name) || x.DetailName2.ToLower().Contains(detail2Name)
                                         && x.PriceList == "BGC")
                                         );
                    if (good is null)
                    {
                        message.Append("Hàng hóa dòng " + i.ToString() + " không tồn tại");
                        continue;
                    }
                    var taxRate = taxRates.Find(x => x.Id == good.TaxRateId);
                    BillDetailImportModel billDetail = new BillDetailImportModel()
                    {
                        GoodsId = good.Id,
                        GoodsName = GoodNameGetter.GetNameFromGood(good),
                        GoodsCode = GoodNameGetter.GetCodeFromGood(good),
                        WareHouseName = good.WarehouseName,
                        SalePrice = good.SalePrice,
                        BillQuantity = int.Parse(sheet.Cells[i, 5]?.Value.ToString()),
                        TaxVat = good.SalePrice * (taxRate?.Percent ?? 0) / 100,
                        DiscountPrice = good.DiscountPrice,
                        Image1 = good.Image1,
                        //DiscountType = good.GoodsType
                    };
                    billDetails.Add(billDetail);
                }

            }
        }
        if (!string.IsNullOrEmpty(message.ToString()))
        {
            throw new ErrorException(message.ToString());
        }
        return billDetails;
    }


    public async Task<List<Ledger>> GetLedgerFromBillId(int billId)
    {
        return await _context.Ledgers.Where(x => x.BillId == billId && x.IsInternal == 3 && x.Type == "XK").ToListAsync();
    }

    public async Task UpdateUserSale(int id, int userId)
    {
        var bill = await _context.Bills.FindAsync(id);
        if (bill == null)
        {
            throw new ErrorException(ErrorMessage.BILL_IS_NOT_FOUND);
        }
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        bill.UserCode = user?.Username;
        _context.Bills.Update(bill);
        await _context.SaveChangesAsync();
    }

    public async Task<(bool success, string msg)> CreateInvoice(int Id)
    {
        var rawData = await (
            from b in _context.Bills
            join c in _context.Customers on b.CustomerId equals c.Id
            join bd in _context.BillDetails on b.Id equals bd.BillId
            join g in _context.Goods on bd.GoodsId equals g.Id
            join i in _context.CustomerTaxInformations on b.CustomerId equals i.CustomerId
            where b.Id == Id
            select new
            {
                Bill = b,
                Customer = c,
                Detail = bd,
                Goods = g,
                CustomerTaxInformations = i
            }
        ).ToListAsync();

        if (!rawData.Any())
        {
            return (false, "Không tìm thấy hóa đơn.");
        }

        var first = rawData.First();

        var company = await _context.Companies.FirstOrDefaultAsync();

        var invoice = new CreateInvoice
        {
            invName = company.InvName,
            invSerial = company.InvSerial,
            invRef = first.Bill.Id.ToString(),
            buyerName = first.Customer.Name,
            buyCode = first.Customer.Code,
            buyerAddress = first.CustomerTaxInformations.Address,
            buyerEmail = first.Customer.Email,
            buyerPhone = first.Customer.Phone,
            buyerCompany= first.CustomerTaxInformations.CompanyName,
            buyerTax =first.CustomerTaxInformations.TaxCode,
            invSubTotal = first.Bill.TotalAmount - first.Bill.Vat,
            invVatRate = first.Bill.VatRate,
            invVatAmount = first.Bill.Vat,
            invDscnAmnt = 0,
            invTotalAmount = first.Bill.TotalAmount,
            invPayment = first.Bill.TypePay,
            note = first.Bill.Note,
            invAutoSign = 0,
            invDate = first.Bill.CreatedDate.ToString("yyyy/MM/dd"),
            invRefDate = DateTime.Now.ToString("yyyy/MM/dd"),
            items = rawData.Select((x, index) => new InvoiceItem
            {
                itemNo = index + 1,
                itemCode = !string.IsNullOrEmpty(x.Goods.Detail2) ? x.Goods.Detail2
                         : !string.IsNullOrEmpty(x.Goods.Detail1) ? x.Goods.Detail1
                         : x.Goods.Account,
                itemName = !string.IsNullOrEmpty(x.Goods.DetailName2) ? x.Goods.DetailName2
                         : !string.IsNullOrEmpty(x.Goods.DetailName1) ? x.Goods.DetailName1
                         : x.Goods.AccountName,
                itemUnit = x.Goods.StockUnit,
                itemQuantity = x.Detail.Quantity,
                itemPrice = x.Detail.UnitPrice,
                itemAmountNoVat = x.Detail.UnitPrice * x.Detail.Quantity,
                itemVatRate = first.Bill.VatRate,
                itemVatAmnt = x.Detail.UnitPrice * x.Detail.Quantity * first.Bill.VatRate / 100,
            }).ToList()
        };


        var httpClient = new HttpClient();
        var apiUrl = $"{_winInvoiceConfig.APIBaseUrl}{_winInvoiceConfig.CreateInvoice}";

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{company.ClientID}:{company.ClientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var dtoJson = JsonConvert.SerializeObject(invoice);

        var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");
        request.Content = content;
        Console.WriteLine("Request content:");
        Console.WriteLine(request);

        var response = await httpClient.SendAsync(request);
        var resContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseGetLinkInvoice>(resContent);
        Console.WriteLine("Response content:");
        Console.WriteLine(resContent);
        if (result.IsSuccess)
        {
            return (true, $"Tạo hóa đơn thành công");
        }
        else
        {
            return (false, $"Lỗi khi gửi hóa đơn: {result.ErrorMessage}");
        }
    }

    public async Task<(bool success, string msg, string link)> GetLinkInvoice(int Id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync();

        var httpClient = new HttpClient();
        var apiUrl = $"{_winInvoiceConfig.APIBaseUrl}{_winInvoiceConfig.GetLinkInvoice}";

        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{company.ClientID}:{company.ClientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var param = new GetLinkInvoiceRequest()
        {
            invRef = Id.ToString(),
            invSample = company.InvName,
            invSign = company.InvSerial,
        };
        var dtoJson = JsonConvert.SerializeObject(param);

        var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");
        request.Content = content;

        Console.WriteLine("Request content:");
        Console.WriteLine(request);

        var response = await httpClient.SendAsync(request);

        var resContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseGetLinkInvoice>(resContent);
        Console.WriteLine("Response content:");
        Console.WriteLine(resContent);
        if (result.IsSuccess)
        {
            return (true, result.ErrorMessage, result.Data.Link);
        }
        else
        {
            return (false, result.ErrorMessage, "");
        }
    }

    public async Task<(bool success, string msg)> DigitallySignInvoice(int Id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync();

        var httpClient = new HttpClient();
        var apiUrl = $"{_winInvoiceConfig.APIBaseUrl}{_winInvoiceConfig.DigitallySignInvoice}";
        var bill = await _context.Bills.FirstOrDefaultAsync(x => x.Id == Id);
        if(bill == null)
        {
            return (false, "Không tìm thấy hóa đơn");

        }
        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{company.ClientID}:{company.ClientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var param = new DigitallySignInvoiceRequest()
        {
            invRef = Id.ToString(),
            invName = company.InvName,
            invSerial = company.InvSerial,
        };
        var dtoJson = JsonConvert.SerializeObject(param);

        var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await httpClient.SendAsync(request);
        var resContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseDigitallySignInvoice>(resContent);

        return (result.IsSuccess, result.ErrorMessage);
    }

    public async Task<(bool success, string msg)> DeleteSignInvoice(int Id)
    {
        var company = await _context.Companies.FirstOrDefaultAsync();

        var httpClient = new HttpClient();
        var apiUrl = $"{_winInvoiceConfig.APIBaseUrl}/api/invoice/delete_raw_inv";
        var bill = await _context.Bills.FirstOrDefaultAsync(x => x.Id == Id);
        if (bill == null)
        {
            return (false, "Không tìm thấy hóa đơn");

        }
        var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{company.ClientID}:{company.ClientSecret}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var param = new DeleteInvoiceRequest()
        {
            invRef = Id.ToString(),
            invcSign = company.InvSerial,
            invcCode = '0'.ToString(),
        };
        var dtoJson = JsonConvert.SerializeObject(param);

        var content = new StringContent(dtoJson, Encoding.UTF8, "application/json");
        request.Content = content;

        var response = await httpClient.SendAsync(request);
        var resContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ResponseDigitallySignInvoice>(resContent);

        return (result.IsSuccess, result.ErrorMessage);
    }




    }