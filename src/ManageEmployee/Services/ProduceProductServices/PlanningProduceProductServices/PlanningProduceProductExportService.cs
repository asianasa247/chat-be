using DinkToPdf.Contracts;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.CompanyEntities;

namespace ManageEmployee.Services.ProduceProductServices.PlanningProduceProductServices;

public class PlanningProduceProductExportService : IPlanningProduceProductExportService
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _converterPDF;
    private readonly ICompanyService _companyService;
    private readonly AppSettings _appSettings;
    private readonly IProcedureExportHelper _procedureExportHelper;
    private readonly ICarDeliveryService _carDeliveryService;
    private readonly IPaymentProposalService _paymentProposalService;
    private readonly IHtmlToPdfConverter _htmlToPdfConverter;
    private readonly IBillPromotionService _billPromotionService;

    private PlanningProduceProduct _planning;
    private List<PlanningProduceProductDetail> _planningDetails;
    private Company _company;
    private string _soThapPhan;
    private IEnumerable<Goods> _goods;
    private IEnumerable<Customer> _customers;

    public PlanningProduceProductExportService(ApplicationDbContext context,
        IConverter converterPDF,
        ICompanyService companyService,
        IOptions<AppSettings> appSettings,
        IProcedureExportHelper procedureExportHelper,
        ICarDeliveryService carDeliveryService,
        IPaymentProposalService paymentProposalService,
        IHtmlToPdfConverter htmlToPdfConverter,
        IBillPromotionService billPromotionService)
    {
        _context = context;
        _converterPDF = converterPDF;
        _companyService = companyService;
        _appSettings = appSettings.Value;
        _procedureExportHelper = procedureExportHelper;
        _carDeliveryService = carDeliveryService;
        _paymentProposalService = paymentProposalService;
        _htmlToPdfConverter = htmlToPdfConverter;
        _billPromotionService = billPromotionService;
    }

    public async Task<string> ExportFull(int id)
    {
        try
        {
            await InitData(id);
            string _template = "PlanningProduceProductTemplate.html",
                     _folderPath = @"Uploads\Html\ProduceProduct",
                     path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                     _allText = File.ReadAllText(path), resultHTML = string.Empty, resultSignHTML = string.Empty;

            var carNames = _planningDetails.Select(x => x.CarName).Distinct().ToList();
            foreach (var carName in carNames)
            {
                var customerIds = _planningDetails.Where(x => x.CarName == carName).Select(x => x.CustomerId).Distinct().ToList();
                var j = 0;
                double totalQuantity = 0;
                var rowSpan = _planningDetails.Where(x => x.CarName == carName).Count() + 1;
                var _txtTotal = @"<tr>
                                            <td class='txt-center'>{{{Index}}}</td>
                                            <td>{{{GoodName}}}</td>
                                            <td>{{{CustomerName}}}</td>
                                            <td>{{{Unit}}}</td>
                                            <td class='txt-right'>{{{Quantity}}}</td>
                                            <td class='txt-right'>{{{RealQuantity}}}</td>
                                            <td rowspan='{{{RowSpan}}}'>{{{CarName}}}</td>
                                        </tr>";
                foreach (var customerId in customerIds)
                {
                    var detailGoods = _planningDetails.Where(x => x.CustomerId == customerId && x.CarName == carName).ToList();
                    int i = 0;
                    j++;
                    var customer = _customers.FirstOrDefault(x => x.Id == customerId);

                    foreach (var detailGood in detailGoods)
                    {
                        i++;
                        var _txt = @"<tr>
                                            <td class='txt-center'>{{{Index}}}</td>
                                            <td>{{{GoodName}}}</td>
                                            <td>{{{CustomerName}}}</td>
                                            <td>{{{Unit}}}</td>
                                            <td class='txt-right'>{{{Quantity}}}</td>
                                            <td class='txt-right'>{{{RealQuantity}}}</td>
                                        </tr>";

                        if (j == 1 && i == 1)
                        {
                            _txt = _txtTotal;
                        }

                        var good = _goods.FirstOrDefault(x => x.Id == detailGood.GoodsId);

                        _txt = _txt
                            .Replace("{{{Index}}}", i.ToString())
                            .Replace("{{{GoodName}}}", GoodNameGetter.GetNameFromGood(good))
                            .Replace("{{{CustomerName}}}", customer?.Name)
                                            .Replace("{{{Unit}}}", good.StockUnit)
                                            .Replace("{{{Quantity}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.Quantity))
                                            .Replace("{{{RealQuantity}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.Quantity * (good.Net ?? 0)))
                                            .Replace("{{{CarName}}}", carName)
                                            .Replace("{{{RowSpan}}}", rowSpan.ToString())
                                            ;
                        totalQuantity += detailGood.Quantity * (good.Net ?? 0);
                        resultHTML += _txt;
                    }

                }
                _txtTotal = @"<tr>
                                            <td colspan='4'>Tổng cộng</td>
                                            <td class='txt-right'>{{{Quantity}}}</td>
                                            <td class='txt-right'>{{{RealQuantity}}}</td>
                                        </tr>";
                _txtTotal = _txtTotal
                                           .Replace("{{{Quantity}}}", string.Format("{0:" + _soThapPhan + "}", _planningDetails.Where(x => x.CarName == carName).Sum(x => x.Quantity)))
                                           .Replace("{{{RealQuantity}}}", string.Format("{0:" + _soThapPhan + "}", totalQuantity))
                                           ;

                resultHTML += _txtTotal;
            }

            resultSignHTML = await _procedureExportHelper.SignPlace(id, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "CompanyName", _company.Name },
                { "CompanyAddress", _company.Address },
                { "CompanyTax", _company.MST },
                { "CompanyPhone", _company.Phone },
                { "Day", _planning.Date.Day.ToString() },
                { "Month", _planning.Date.Month.ToString() },
                { "Year", _planning.Date.Year.ToString() },
                { "MST", _company?.MST},
                { "Phone", _company?.Phone},
                { "CompanyImage", $"{_appSettings.UrlHost}{_company?.FileLogo}"},
                { "ProduceCode", _planning.Code},
                { "ProduceNote", _planning.Note},
                { "WebsiteName", _company.WebsiteName},
                { "Email", _company.Email},

            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);
            _allText = _allText.Replace("##SIGN_REPLACE_PLACE##", resultSignHTML);

            var fileName = ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "KeHoachNhapXuat");

            return fileName;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message, ex);
        }
    }

    public async Task<List<string>> ExportForCar(int? carId, string carCode, int id)
    {
        try
        {
            await InitData(id, carId, carCode, true);

            var fileNames = new List<string>();
            foreach (var customer in _customers)
            {
                var bill = await DeliveryBill(customer, id, carId, carCode);
                var billSale = await DeliveryBillWithSale(customer, id, carId, carCode);
                string pageBreak = "<div style='page-break-before: always;'></div>";
                var allText = bill + pageBreak + billSale;
                fileNames.Add(ExcelHelpers.ConvertUseDink(allText, _converterPDF, Directory.GetCurrentDirectory(), "PhieuXuatKho"));
            }
            return fileNames;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message, ex);
        }
    }

    private async Task<string> DeliveryBill(Customer customer, int id, int? carId, string carCode)
    {
        string _template = "PhieuXuatKho.html",
                    _folderPath = @"Uploads\Html\ProduceProduct",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = File.ReadAllText(path), resultHTML = string.Empty, resultSignHTML = string.Empty;

        int i = 0;
        double totalAmount = 0;
        var detailGoods = _planningDetails.Where(x => x.CustomerId == customer.Id).ToList();
        bool isCanceled = detailGoods.Any(x => x.IsCanceled);

        foreach (var detailGood in detailGoods)
        {
            i++;
            var good = _goods.FirstOrDefault(x => x.Id == detailGood.GoodsId);
            string _txt = @"<tr>
                                    <td class='txt-center'>{{{Index}}}</td>
                                    <td>{{{GoodName}}}</td>
                                    <td>{{{GoodCode}}}</td>
                                    <td>{{{Unit}}}</td>
                                    <td class='txt-right'>{{{RequiredQuantity}}}</td>
                                    <td class='txt-right'>{{{RealQuantity}}}</td>
                                    <td class='txt-right'>{{{UnitPrice}}}</td>
                                    <td class='txt-right'>{{{Amount}}}</td>
                                </tr>";
            _txt = _txt
                .Replace("{{{Index}}}", i.ToString())
                .Replace("{{{GoodName}}}", GoodNameGetter.GetNameFromGood(good))
                .Replace("{{{GoodCode}}}", GoodNameGetter.GetCodeFromGood(good))
                                .Replace("{{{Unit}}}", good.StockUnit)
                                .Replace("{{{RequiredQuantity}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.Quantity))
                                .Replace("{{{RealQuantity}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.Quantity))
                                .Replace("{{{UnitPrice}}}", string.Empty)
                                .Replace("{{{Amount}}}", string.Empty);

            resultHTML += _txt;
            totalAmount += detailGood.Quantity * good.SalePrice;
        }
        var billPromotions = await _billPromotionService.Get(id, nameof(PlanningProduceProduct), carId, carCode, customerId: customer.Id);

        foreach (var bill in billPromotions)
        {
            if (bill.Discount == 0)
            {
                string _txt = @"<tr>
                        <td class='txt-center'></td>
                        <td></td>
                        <td>{{{TENHANG}}}</td>
                        <td class='txt-right'>{{{DONVITINH}}}</td>
                        <td></td>
                        <td class='txt-right'>{{{SOLUONG}}}</td>
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
        }
        var _signtxtItem = @"<td class='td-no-border'>
                                        <div class='signature-item'>
                                            <p class='signature-item-signer'>Người nhận</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <p class='signature-item-signer'></p><br/>
                                        </div>
                                    </td>";
        resultSignHTML = _signtxtItem + await _procedureExportHelper.SignPlace(id, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));

        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "CompanyName", _company.Name },
                { "CompanyAddress", _company.Address },
                { "CompanyTax", _company.MST },
                { "CompanyPhone", _company.Phone },
                { "Day", _planning.Date.Day.ToString() },
                { "Month", _planning.Date.Month.ToString() },
                { "Year", _planning.Date.Year.ToString() },
                { "CustomerName", customer.Name },
                { "CustomerAddress", customer.Address },
                { "TotalRequiredQuantity",  string.Format("{0:" + _soThapPhan + "}", detailGoods.Sum(x => x.Quantity) )},
                { "TotalRealQuantity",  string.Format("{0:" + _soThapPhan + "}", detailGoods.Sum(x => x.Quantity)) },
                { "TotalAmount",  string.Empty},
                { "Reason", string.Empty },
                { "WarehouseName", string.Empty },
                { "WarehouseAddress", string.Empty },
                { "Debit", string.Empty },
                { "Credit", string.Empty },
                { "BillNumber", string.Empty },
                { "TotalAmountText",  string.Empty},
                { "MST", _company?.MST},
                { "Phone", _company?.Phone},
                { "Status", isCanceled ? "Hủy" : string.Empty},
                { "CompanyImage", $"{_appSettings.UrlHost}{_company?.FileLogo}"},
                { "ProduceCode",(string.IsNullOrEmpty(_planning.Code) ? _planning.ProcedureNumber : _planning.Code) + " - " + detailGoods.FirstOrDefault()?.DeliveryCode},

            };

        v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
        _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);
        _allText = _allText.Replace("##SIGN_REPLACE_PLACE##", resultSignHTML);
        return _allText;
        //return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "PhieuXuatKho");
    }

    private async Task<string> DeliveryBillWithSale(Customer customer, int id, int? carId, string carCode)
    {
        string _template = "PhieuXuatKhoKiemBanHang.html",
                    _folderPath = @"Uploads\Html\ProduceProduct",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = File.ReadAllText(path), resultHTML = string.Empty, resultSignHTML = string.Empty;

        int i = 0;
        double totalAmount = 0;
        var detailGoods = _planningDetails.Where(x => x.CustomerId == customer.Id).ToList();
        bool isCanceled = detailGoods.Any(x => x.IsCanceled);

        foreach (var detailGood in detailGoods)
        {
            i++;
            var good = _goods.FirstOrDefault(x => x.Id == detailGood.GoodsId);

            string _txt = @"<tr>
                                    <td class='txt-center'>{{{Index}}}</td>
                                    <td>{{{GoodCode}}}</td>
                                    <td>{{{GoodName}}}</td>
                                    <td>{{{Unit}}}</td>
                                    <td class='txt-right'>{{{Quantity}}}</td>
                                    <td class='txt-right'>{{{UnitPrice}}}</td>
                                    <td class='txt-right'>{{{Amount}}}</td>
                                </tr>";
            _txt = _txt
                .Replace("{{{Index}}}", i.ToString())
                .Replace("{{{GoodName}}}", GoodNameGetter.GetNameFromGood(good))
                .Replace("{{{GoodCode}}}", GoodNameGetter.GetCodeFromGood(good))
                                .Replace("{{{Unit}}}", good.StockUnit)
                                .Replace("{{{Quantity}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.Quantity))
                                .Replace("{{{UnitPrice}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.UnitPrice))
                                .Replace("{{{Amount}}}", string.Format("{0:" + _soThapPhan + "}", detailGood.Quantity * detailGood.UnitPrice));

            resultHTML += _txt;
            totalAmount += detailGood.Quantity * detailGood.UnitPrice;
        }
        var billPromotions = await _billPromotionService.Get(id, nameof(PlanningProduceProduct), carId, carCode, customerId: customer.Id);

        foreach (var bill in billPromotions)
        {
            string _txt = @"<tr>
                                            <td class='txt-center'></td>
                                            <td class='txt-center'></td>
                                            <td class='txt-left'>{{{TENHANG}}}</td>
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
        var sumAmount = totalAmount - billPromotions.Sum(x => x.Amount);
        _tr_Sum = _tr_Sum.Replace("{{{TONG_TIEN}}}", string.Format("{0:N0}", sumAmount));

        resultHTML += _tr_Sum;

        var _signtxtItem = @"<td class='td-no-border'>
                                        <div class='signature-item'>
                                            <p class='signature-item-signer'>Người nhận</p>
                                            <p class='signature-item-note'>(Ký, họ tên)</p><br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <br/>
                                            <p class='signature-item-signer'></p><br/>
                                        </div>
                                    </td>";
        resultSignHTML = _signtxtItem + await _procedureExportHelper.SignPlace(id, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));

        var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == customer.Id);
        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "CompanyName", _company.Name },
                { "CompanyAddress", _company.Address },
                { "CompanyTax", _company.MST },
                { "CompanyPhone", _company.Phone },
                { "Day", _planning.Date.Day.ToString() },
                { "Month", _planning.Date.Month.ToString() },
                { "Year", _planning.Date.Year.ToString() },
                { "CustomerName", customer.Name },
                { "CustomerAddress", customer.Address },
                { "CustomerPhone", customer.Phone },
                { "CustomerTax", customerTax?.TaxCode },
                { "TotalAmount",  string.Format("{0:" + _soThapPhan + "}", totalAmount)},
                { "Description", string.Empty },
                { "SalerName", string.Empty },
                { "TotalAmountText", NumberToWords.ConvertAmount(sumAmount) },
                { "DocumentNumber", string.Empty },
                { "Debit", string.Empty },
                { "Credit", string.Empty },
                { "BillNumber", string.Empty },
                { "MST", _company.MST},
                { "Phone", _company.Phone},
                { "Status", isCanceled ? "Hủy" : string.Empty},
                { "CompanyImage", $"{_appSettings.UrlHost}{_company?.FileLogo}"},
                { "ProduceCode", (string.IsNullOrEmpty(_planning.Code) ? _planning.ProcedureNumber : _planning.Code)  + " - " + detailGoods.FirstOrDefault()?.DeliveryCode},

            };

        v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
        _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);
        _allText = _allText.Replace("##SIGN_REPLACE_PLACE##", resultSignHTML);
        return _allText;
    }

    public async Task<string> ExportGatePass(int? carId, string carCode, int id)
    {
        await InitData(id, carId, carCode, true);

        //if(!_planningDetails.Any(x => !x.ShouldExport))
        //{
        //    throw new ErrorException(ErrorMessages.AccessDenined);
        //}
        var soThapPhan = "N" + _company.MethodCalcExportPrice;
        string _template = "GiayRaCong.html",
                    _folderPath = @"Uploads\Html\ProduceProduct",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = File.ReadAllText(path), resultHTML = string.Empty, resultSignHTML = string.Empty;
        var planningDetail = _planningDetails.FirstOrDefault();
        var planningDetailIds = _planningDetails.Where(x => x.CarId == carId && x.CarName == carCode).Select(x => x.Id).ToList();
        var carDeliveryId = await _context.CarDeliveries.Where(X => X.TableName == nameof(PlanningProduceProductDetail) && planningDetailIds.Contains(X.TableId)).FirstOrDefaultAsync();
        var carDelivery = await _carDeliveryService.Get(carDeliveryId?.Id);

        resultSignHTML = await _procedureExportHelper.SignPlaceLastest(id, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));


        var note = "";
        foreach (var customer in _customers)
        {
            var details = _planningDetails.Where(x => x.CustomerId == customer.Id).Sum(x => x.Quantity);
            note += $"Giao hàng cho: {customer.Name}; Số lượng: {details} tấn;\n";
        }

        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "CompanyName", _company.Name },
                { "CompanyAddress", _company.Address },
                { "CompanyTax", _company.MST },
                { "CompanyPhone", _company.Phone },
                { "Local", carDelivery?.Driver },
                { "Hour", _planning.Date.Hour.ToString() },
                { "Day", _planning.Date.Day.ToString() },
                { "Month", _planning.Date.Month.ToString() },
                { "Year", _planning.Date.Year.ToString() },
                { "CarName", carDelivery?.LicensePlates ?? planningDetail?.CarName },
                { "Note", note },
                { "MST", _company.MST},
                { "Phone", _company.Phone},
                { "CompanyImage", $"{_appSettings.UrlHost}{_company?.FileLogo}"},
                { "UserSign", "" },
                { "UserName", "" },
                { "UserPosition", "BAN GIÁM ĐỐC" },
                { "ProduceCode", string.IsNullOrEmpty(_planning.Code) ? _planning.ProcedureNumber : _planning.Code},

            };

        v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
        _allText = _allText.Replace("##SIGN_REPLACE_PLACE##", resultSignHTML);

        //await _htmlToPdfConverter.ExportPdf(_allText, "GiayRaCong");
        return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "GiayRaCong");
        //return string.Empty;
    }

    public async Task<string> ExportPaymentProposal(int? carId, string carName, int id)
    {
        var planningDetailIds = await _context.PlanningProduceProductDetails
                            .Where(x => x.PlanningProduceProductId == id && x.CarId == carId && x.CarName == carName)
                            .Select(x => x.Id).ToListAsync();
        var paymentProposalId = await _context.PaymentProposals.FirstOrDefaultAsync(x => planningDetailIds.Contains(x.TableId ?? 0) && x.TableName == nameof(PlanningProduceProductDetail));
        if (paymentProposalId is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        return await _paymentProposalService.Export(paymentProposalId.Id, true);
    }

    private async Task InitData(int id, int? carId = null, string carName = null, bool shouldFinish = false)
    {
        _planning = await _context.PlanningProduceProducts.FindAsync(id);
        if (!_planning.IsFinished && shouldFinish)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }
        _planningDetails = await _context.PlanningProduceProductDetails
                                            .Where(x => x.PlanningProduceProductId == id
                                            && (carId == null || x.CarId == carId)
                                            && (string.IsNullOrEmpty(carName) || x.CarName == carName)).ToListAsync();
        var goodIds = _planningDetails.Select(x => x.GoodsId).ToList();
        _goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        _company = await _companyService.GetCompany();
        _soThapPhan = "N" + _company.MethodCalcExportPrice;
        var customerIds = _planningDetails.Select(x => x.CustomerId);
        _customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();

    }
}