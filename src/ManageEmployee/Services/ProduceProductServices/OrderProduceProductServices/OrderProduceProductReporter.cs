using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Services.ProduceProductServices.OrderProduceProductServices;

public class OrderProduceProductReporter : IOrderProduceProductReporter
{
    private readonly ApplicationDbContext _context;
    private readonly ICompanyService _companyService;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;
    private readonly IConverter _converterPDF;

    public OrderProduceProductReporter(ApplicationDbContext context,
        ICompanyService companyService,
        IOptions<AppSettings> appSettings,
        IProcedureExportHelper procedureExportHelper,
        IConverter converterPDF)
    {
        _context = context;
        _companyService = companyService;
        _appSettings = appSettings.Value;
        _procedureExportHelper = procedureExportHelper;
        _converterPDF = converterPDF;
    }

    public async Task<string> ExportPdf(int id)
    {
        try
        {
            var orderProduceProduct = await _context.OrderProduceProducts.FindAsync(id);
            if (orderProduceProduct is null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }

            var orderProduceProductDetails = await _context.OrderProduceProductDetails.Where(x => x.OrderProduceProductId == id).ToListAsync();
            var goodIds = orderProduceProductDetails.Select(x => x.GoodsId).ToList();
            var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
            var billPromotions = await _context.BillPromotions.Where(x => orderProduceProduct.Id == x.TableId).ToListAsync();
            var customer = await _context.Customers.FindAsync(orderProduceProduct.CustomerId);
            var p = await _companyService.GetCompany();

            string _template = "OrderProduceProductTemplate.html",
                _folderPath = @"Uploads/Html/ProduceProduct",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = File.ReadAllText(path), resultHTML = string.Empty;
            string path_logo = _appSettings.UrlHost;
            path_logo += p.FileLogo;
            var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == orderProduceProduct.CustomerId);

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Name },
                { "DiaChi", p.Address },
                { "SoDienThoai", p.Phone },
                { "WebsiteName", p.WebsiteName },
                { "Email", p.Email },
                { "NoteOfCEO", p.NoteOfCEO},
                { "NameOfCEO", p.NameOfCEO},
                { "ProduceCode", orderProduceProduct.Code},

                { "CustomerName", customer.Name + (string.IsNullOrEmpty(customerTax?.CompanyName) ? "" : " - " + customerTax.CompanyName) },
                { "CustomerAddress", customer.Address},
                { "CustomerPhone", customer.Phone},
                { "CustomerTax", customerTax?.TaxCode},
                { "LoGoCongTy", path_logo},
                { "CompanyTax", p.MST},
                { "BaoGiaNote", p.Note},
                { "DateProduce", $"Ngày {orderProduceProduct.CreatedAt.Day} tháng {orderProduceProduct.CreatedAt.Month} năm {orderProduceProduct.CreatedAt.Year}"},
                { "Note", orderProduceProduct.Note},
            };
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            int i = 0;
            foreach (var good in goods)
            {
                i++;
                var orderProduceProductDetail = orderProduceProductDetails.FirstOrDefault(x => x.GoodsId == good.Id);
                string _txt = @"<tr>
                                            <td class='txt-center'>{{{STT}}}</td>
                                            <td colspan='2' class='txt-left'>{{{TENHANG}}}</td>
                                            <td class='txt-center'>{{{DVT}}}</td>
                                            <td class='txt-center'>{{{SOLUONG}}}</td>
                                            <td class='txt-right'>{{{DONGIA}}}</td>
                                            <td class='txt-right'>{{{THANHTIEN}}}</td>
                                        </tr>";

                _txt = _txt.Replace("{{{STT}}}", i.ToString())
                                    .Replace("{{{TENHANG}}}", GoodNameGetter.GetNameFromGood(good))
                                    .Replace("{{{DVT}}}", good.StockUnit)
                                    .Replace("{{{SOLUONG}}}", string.Format("{0:N0}", orderProduceProductDetail.QuantityRequired))
                                    .Replace("{{{DONGIA}}}", string.Format("{0:N0}", orderProduceProductDetail.UnitPrice))
                                    .Replace("{{{THANHTIEN}}}", string.Format("{0:N0}", orderProduceProductDetail.QuantityRequired * orderProduceProductDetail.UnitPrice))
                                    ;
                resultHTML += _txt;
            }
            foreach (var bill in billPromotions)
            {
                string _txt = @"<tr>
                                            <td class='txt-center'></td>
                                            <td colspan='2' class='txt-left'>{{{TENHANG}}}</td>
                                            <td class='txt-center'>{{{DONVITINH}}}</td>
                                            <td class='txt-center'>{{{SOLUONG}}}</td>
                                            <td class='txt-right'>{{{DONGIA}}}</td>
                                            <td class='txt-right'>{{{THANHTIEN}}}</td>
                                        </tr>";

                _txt = _txt.Replace("{{{TENHANG}}}", bill.Note)
                    .Replace("{{{DONVITINH}}}", bill.Unit)
                                    .Replace("{{{SOLUONG}}}", string.Format("{0:N0}", bill.Qty))
                                    .Replace("{{{DONGIA}}}", string.Format("{0:N0}", -bill.Discount))
                                    .Replace("{{{THANHTIEN}}}", string.Format("{0:N0}", -bill.Amount))
                                    ;
                resultHTML += _txt;
            }

            string _tr_Sum = @"<tr class='font - b'>
                              <td colspan = '6' class='txt-left font-b'>TỔNG CỘNG </td>
                                  <td class='txt-right font-b'>{{{TONG_TIEN}}}</td>
                            </tr>    ";
            var sumAmount = orderProduceProductDetails.Sum(x => x.QuantityReal * x.UnitPrice) - billPromotions.Sum(x => x.Amount);
            _tr_Sum = _tr_Sum.Replace("{{{TONG_TIEN}}}", string.Format("{0:N0}", sumAmount));

            resultHTML += _tr_Sum;

            var signature = await _procedureExportHelper.SignPlaceOrder(id, nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT), true);
            var signatureCEO = await _procedureExportHelper.SignPlaceLastestOrder(id, nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));

            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML)
                .Replace("{{{AmountString}}}", NumberToWords.ConvertAmount(sumAmount))
                .Replace("##REPLACE_PLACE_SIGNATURE##", signature)
                .Replace("##REPLACE_PLACE_SIGNATURE_CEO##", signatureCEO);
            return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "DonDatHang");
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            return string.Empty;
        }
    }

    public async Task<IEnumerable<OrderProduceProductReport>> ReportAsync(OrderProduceProductReportRequestModel param)
    {
        var orders = await _context.OrderProduceProducts.Where(x => (param.FromAt == null || x.Date >= param.FromAt.Value)
                                                && (param.ToAt == null || x.Date <= param.ToAt.Value) && x.IsFinished).ToListAsync();
        var orderIds = orders.Select(x => x.Id).ToList();
        var details = await _context.OrderProduceProductDetails.Where(x => orderIds.Contains(x.OrderProduceProductId)).ToListAsync();
        var goodIds = details.Select(x => x.GoodsId).Distinct();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        var customerIds = orders.Select(x => x.CustomerId).ToList();
        var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();

        if (param.Type == 0)
        {
            return ReportForGoods(goods, details, customers, orders);
        }
        else
        {
            return ReportForCustomer(goods, details, customers, orders);
        }
    }

    private IEnumerable<OrderProduceProductReport> ReportForGoods(List<Goods> goods, List<OrderProduceProductDetail> details,
        List<Customer> customers, List<OrderProduceProduct> orders)
    {
        var listOut = new List<OrderProduceProductReport>();

        foreach (var good in goods)
        {
            var detailChecks = details.Where(x => x.GoodsId == good.Id);
            var orderCheckIds = details.Select(x => x.OrderProduceProductId).Distinct();
            var orderChecks = orders.Where(x => orderCheckIds.Contains(x.Id));
            var customerCheckIds = orderChecks.Select(x => x.CustomerId);
            var customerChecks = customers.Where(x => customerCheckIds.Contains(x.Id));

            var itemOut = new OrderProduceProductReport
            {
                GoodName = GoodNameGetter.GetCodeFromGood(good),
                Quantity = detailChecks.Sum(x => x.QuantityRequired),
                QuantityDelivered = detailChecks.Sum(x => x.QuantityDelivered),
                QuantityInProgress = detailChecks.Sum(x => x.QuantityInProgress),
                Items = new List<OrderProduceProductReportDetail>()
            };

            foreach (var customer in customerChecks)
            {
                var orderIdForCustomer = orderChecks.Where(x => x.CustomerId == customer.Id).Select(x => x.Id);

                var detailOut = new OrderProduceProductReportDetail
                {
                    CustomerName = customer.Name,
                    OrderProduceProductCode = String.Join("; ", orderChecks.Where(x => orderIdForCustomer.Contains(x.Id)).Select(x => x.Code)),
                    Quantity = detailChecks.Where(x => orderIdForCustomer.Contains(x.OrderProduceProductId)).Sum(x => x.QuantityRequired),
                    QuantityDelivered = detailChecks.Where(x => orderIdForCustomer.Contains(x.OrderProduceProductId)).Sum(x => x.QuantityDelivered),
                    QuantityInProgress = detailChecks.Where(x => orderIdForCustomer.Contains(x.OrderProduceProductId)).Sum(x => x.QuantityInProgress),
                };
                itemOut.Items.Add(detailOut);
            }
            listOut.Add(itemOut);
        }
        return listOut;
    }

    private IEnumerable<OrderProduceProductReport> ReportForCustomer(List<Goods> goods, List<OrderProduceProductDetail> details,
        List<Customer> customers, List<OrderProduceProduct> orders)
    {
        var listOut = new List<OrderProduceProductReport>();

        foreach (var customer in customers)
        {
            var orderChecks = orders.Where(x => x.CustomerId == customer.Id);
            var orderCheckIds = orderChecks.Select(x => x.Id);
            var detailChecks = details.Where(x => orderCheckIds.Contains(x.OrderProduceProductId));
            var goodIds = detailChecks.Select(x => x.GoodsId);
            var goodChecks = goods.Where(x => goodIds.Contains(x.Id));

            var itemOut = new OrderProduceProductReport
            {
                CustomerName = customer.Name,
                Quantity = detailChecks.Sum(x => x.QuantityRequired),
                QuantityDelivered = detailChecks.Sum(x => x.QuantityDelivered),
                QuantityInProgress = detailChecks.Sum(x => x.QuantityInProgress),
                Items = new List<OrderProduceProductReportDetail>()
            };
            foreach (var good in goodChecks)
            {
                var orderIdForGood = detailChecks.Where(x => x.GoodsId == good.Id).Select(x => x.OrderProduceProductId);

                var detailOut = new OrderProduceProductReportDetail
                {
                    GoodName = GoodNameGetter.GetCodeFromGood(good),
                    OrderProduceProductCode = String.Join("; ", orderChecks.Where(x => orderIdForGood.Contains(x.Id)).Select(x => x.Code)),
                    Quantity = detailChecks.Where(x => x.GoodsId == good.Id).Sum(x => x.QuantityRequired),
                    QuantityDelivered = detailChecks.Where(x => x.GoodsId == good.Id).Sum(x => x.QuantityDelivered),
                    QuantityInProgress = detailChecks.Where(x => x.GoodsId == good.Id).Sum(x => x.QuantityInProgress),
                };
                itemOut.Items.Add(detailOut);
            }
            listOut.Add(itemOut);
        }
        return listOut;
    }

    public async Task<string> ExportReportAsync(OrderProduceProductReportRequestModel param)
    {
        var datas = await ReportAsync(param);
        var p = await _companyService.GetCompany();

        string _template = "OrderProduceProductReportTemplate.html",
            _folderPath = @"Uploads/Html/ProduceProduct",
            path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
            _allText = File.ReadAllText(path), resultHTML = string.Empty;
        string path_logo = _appSettings.UrlHost;
        path_logo += p.FileLogo;

        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Name },
                { "DiaChi", p.Address },
                { "SoDienThoai", p.Phone },
                { "WebsiteName", p.WebsiteName },
                { "Email", p.Email },
                { "NoteOfCEO", p.NoteOfCEO},
                { "NameOfCEO", p.NameOfCEO},
                { "LoGoCongTy", path_logo},
                { "CompanyTax", p.MST},
                { "BaoGiaNote", p.Note},
            };
        v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

        int i = 0;
        foreach (var data in datas)
        {
            i++;
            string _txt = @"<tr>
                                            <td class='txt-center'>{{{STT}}}</td>
                                            <td colspan='2' class='txt-left'>{{{TENHANG}}}</td>
                                            <td class='txt-center'>{{{SOLUONG}}}</td>
                                            <td class='txt-right'>{{{DANGLAM}}}</td>
                                            <td class='txt-right'>{{{BANGIAO}}}</td>
                                        </tr>";

            _txt = _txt.Replace("{{{STT}}}", i.ToString())
                                .Replace("{{{TENHANG}}}", param.Type == 0 ? data.GoodName : data.CustomerName)
                                .Replace("{{{SOLUONG}}}", string.Format("{0:N0}", data.Quantity))
                                .Replace("{{{DANGLAM}}}", string.Format("{0:N0}", data.QuantityInProgress))
                                .Replace("{{{BANGIAO}}}", string.Format("{0:N0}", data.QuantityDelivered))
                                ;
            resultHTML += _txt;

            int j = 0;
            foreach (var item in data.Items)
            {
                j++;
                string _txtDetail = @"<tr>
                                            <td class='txt-right'> + {{{STT}}} </span></td>
                                            <td colspan='2' class='txt-left'>{{{TENHANG}}}</td>
                                            <td class='txt-right'>{{{SOLUONG}}}</td>
                                            <td class='txt-right'>{{{DANGLAM}}}</td>
                                            <td class='txt-right'>{{{BANGIAO}}}</td>
                                        </tr>";

                _txtDetail = _txtDetail.Replace("{{{STT}}}", j.ToString())
                                    .Replace("{{{TENHANG}}}", param.Type == 0 ? item.CustomerName : item.GoodName)
                                    .Replace("{{{SOLUONG}}}", string.Format("{0:N0}", item.Quantity))
                                    .Replace("{{{DANGLAM}}}", string.Format("{0:N0}", item.QuantityInProgress))
                                    .Replace("{{{BANGIAO}}}", string.Format("{0:N0}", item.QuantityDelivered))
                                    ;
                resultHTML += _txtDetail;
            }
        }

        _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);
        return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "DonDatHangBaoCao");
    }

}