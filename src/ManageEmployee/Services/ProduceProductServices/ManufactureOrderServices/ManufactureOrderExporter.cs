using Common.Extensions;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Generators;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

namespace ManageEmployee.Services.ProduceProductServices.ManufactureOrderServices
{
    public class ManufactureOrderExporter: IManufactureOrderExporter
    {
        private readonly IProcedureExportHelper _procedureExportHelper;
        private readonly IConverter _converterPDF;
        private readonly ICompanyService _companyService;
        private readonly AppSettings _appSettings;
        private readonly ApplicationDbContext _context;
        private readonly IPdfGeneratorService _pdfGeneratorService;

        public ManufactureOrderExporter(IProcedureExportHelper procedureExportHelper,
            IConverter converterPDF,
            ICompanyService companyService,
            IOptions<AppSettings> appSettings,
            ApplicationDbContext context,
            IPdfGeneratorService pdfGeneratorService)
        {
            _procedureExportHelper = procedureExportHelper;
            _converterPDF = converterPDF;
            _companyService = companyService;
            _appSettings = appSettings.Value;
            _context = context;
            _pdfGeneratorService = pdfGeneratorService;
        }
        public async Task<string> ExportPdf(int id)
        {
            try
            {
                var manufactureOrder = await _context.ManufactureOrders.FindAsync(id);
                if (manufactureOrder is null)
                {
                    throw new ErrorException(ErrorMessages.DataNotFound);
                }

                var manufactureOrderDetails = await _context.ManufactureOrderDetails.Where(x => x.ManufactureOrderId == id)
                    .OrderByDescending(x => x.CustomerId).ToListAsync();
                var goodIds = manufactureOrderDetails.Select(x => x.GoodsId).ToList();
                var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
                var p = await _companyService.GetCompany();
                var customerIds = manufactureOrderDetails.Select(x => x.CustomerId).Distinct();
                var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();

                string _template = "ManufactureOrderTemplate.html",
                    _folderPath = @"Uploads\Html\ProduceProduct",
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
                    { "NgayBaoGia", $"Ngày {manufactureOrder.CreatedAt.Day} tháng {manufactureOrder.CreatedAt.Month} năm {manufactureOrder.CreatedAt.Year}"},
                    { "ProduceCode", manufactureOrder.Code},
                    { "ProduceNote", manufactureOrder.Note},

                };
                v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

                int i = 0;
                double quantityNet = 0;
                foreach (var manufactureOrderDetail in manufactureOrderDetails)
                {
                    if (manufactureOrderDetail.GoodsId == 0)
                    {
                        continue;
                    }
                    i++;
                    var good = goods.FirstOrDefault(x => x.Id == manufactureOrderDetail.GoodsId);

                    var customer = customers.FirstOrDefault(x => x.Id == manufactureOrderDetail.CustomerId);

                    string _txt = @"<tr>
                                            <td class='txt-center'>{{{STT}}}</td>
                                            <td class='txt-left'>{{{TENHANG}}}</td>
                                            <td class='txt-left'>{{{DAILY}}}</td>
                                            <td class='txt-center'>{{{DVT}}}</td>
                                            <td class='txt-center'>{{{SOLUONGTAN}}}</td>
                                            <td class='txt-center'>{{{SOLUONGBAO}}}</td>
                                            <td class='txt-center'>{{{GHICHU}}}</td>
                                        </tr>";

                    _txt = _txt.Replace("{{{STT}}}", i.ToString())
                                        .Replace("{{{TENHANG}}}", GoodNameGetter.GetNameFromGood(good))
                                        .Replace("{{{DAILY}}}", customer?.Name)
                                        .Replace("{{{DVT}}}", good.StockUnit)
                                        .Replace("{{{SOLUONGTAN}}}", string.Format("{0:N0}", manufactureOrderDetail.QuantityReal))
                                        .Replace("{{{SOLUONGBAO}}}", string.Format("{0:N0}", manufactureOrderDetail.QuantityReal * (good.Net ?? 0)))
                                        .Replace("{{{GHICHU}}}", manufactureOrderDetail.Note)
                                        ;
                    resultHTML += _txt;
                    quantityNet += manufactureOrderDetail.QuantityReal * (good.Net ?? 0);
                }
                string _tr_Sum = @"<tr class='font - b'>
                                    <td colspan = '4' class='txt-left font-b'>TỔNG CỘNG </td>
                                    <td class='txt-right font-b'>{{{TONG_SOLUONGTAN}}}</td>
                                    <td class='txt-right font-b'>{{{TONG_SOLUONGBAO}}}</td>
                                    <td class='txt-right font-b'></td>
                                 </tr>    ";

                _tr_Sum = _tr_Sum.Replace("{{{TONG_SOLUONGTAN}}}", string.Format("{0:N0}", manufactureOrderDetails.Sum(x => x.QuantityReal)))
                    .Replace("{{{TONG_SOLUONGBAO}}}", string.Format("{0:N0}", quantityNet))
                    ;

                resultHTML += _tr_Sum;


                var signature = await _procedureExportHelper.SignPlace(id, nameof(ProcedureEnum.MANUFACTURE_ORDER));
                _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML)
                    .Replace("##REPLACE_PLACE_SIGNATURE##", signature);
                return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "LenhSanXuat");
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> ExportGoodDetail(int id)
        {
            var manufactureOrder = await _context.ManufactureOrders.FindAsync(id);
            if (manufactureOrder is null)
            {
                throw new ErrorException(ErrorMessages.DataNotFound);
            }

            var manufactureOrderDetails = await _context.ManufactureOrderDetails.Where(x => x.ManufactureOrderId == id)
                .OrderByDescending(x => x.CustomerId).ToListAsync();
            var goodIds = manufactureOrderDetails.Select(x => x.GoodsId).ToList();
            var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
            var goodIdHasDetails = goods.Where(x => x.GoodsType == nameof(GoodsTypeEnum.CB)
                                                    || x.GoodsType == nameof(GoodsTypeEnum.COMBO)
                                                    || x.GoodsType == nameof(GoodsTypeEnum.DM)).Select(x => x.Id);
            var goodDetails = await _context.GoodDetails.Where(x => goodIdHasDetails.Contains(x.GoodID ?? 0)).ToListAsync();

            string htmlContent = await FileExtension.ReadContent(
                    Directory.GetCurrentDirectory(),
                    @"Uploads\Html\ProduceProduct",
                    "ManufactureOrderGoodDetailTemplate.html"
                );

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(htmlContent);
            // Get template section
            var rowTemplateElement = htmlDoc.GetElementbyId("table-row-template");

            if (rowTemplateElement == null)
            {
                throw new Exception("Table row template section not found. Cannot generate report");
            }

            // Remove template after get content
            rowTemplateElement.Remove();

            var mainSb = new StringBuilder(htmlDoc.DocumentNode.OuterHtml);
            var mappingVals = new Dictionary<string, string>
            {
                { nameof(manufactureOrder.ProcedureNumber), manufactureOrder.Code },
            };
            foreach (var val in mappingVals)
            {
                mainSb.Replace($"[{val.Key}]", val.Value);
            }


            // Mapping company information
            await _pdfGeneratorService.MappingCompany(mainSb);

            // Mapping table detail section
            var tblDetailSb = new StringBuilder();

            var rIndex = 1;
            foreach (var good in goods)
            {
                var quantityRequired = manufactureOrderDetails.Where(x => x.GoodsId == good.Id).Sum(x => x.QuantityRequired);
                var quantityReal = manufactureOrderDetails.Where(x => x.GoodsId == good.Id).Sum(x => x.QuantityReal);
                var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                    .Replace("[Index]", rIndex.ToString())
                    .Replace("[Code]", GoodNameGetter.GetCodeFromGood(good))
                    .Replace("[Name]", GoodNameGetter.GetNameFromGood(good))
                    .Replace("[StockUnit]", good.StockUnit)
                    .Replace("[QuantityRequired]", quantityRequired.ToString("n2"))
                    .Replace("[QuantityReal]", quantityReal.ToString("n2"))
                    .Replace("[UnitPrice]", "-")
                    .Replace("[Amount]", "-");
                tblDetailSb.Append(rowSb);
                rIndex++;
            }
            var goodDetailChecks = new List<GoodDetail>();
            foreach (var goodDetail in goodDetails)
            {
                var goodDetailCheck = goodDetailChecks.FirstOrDefault(x => x.Account == goodDetail.Account && x.Detail1 == goodDetail.Detail1
                        && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodDetail.Detail2));
                if (goodDetailCheck != null)
                {
                    continue;
                }
                var goodDetailFinds = goodDetails.Where(x => x.Account == goodDetail.Account && x.Detail1 == goodDetail.Detail1
                        && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodDetail.Detail2));
                double quantityRequired = 0;
                double quantityReal = 0;

                foreach(var goodDetailFind in goodDetailFinds)
                {
                    quantityRequired += manufactureOrderDetails.Where(x => x.GoodsId == goodDetailFind.GoodID).Sum(x => x.QuantityRequired * (goodDetailFind.Quantity ?? 0));
                    quantityReal += manufactureOrderDetails.Where(x => x.GoodsId == goodDetailFind.GoodID).Sum(x => x.QuantityReal * (goodDetailFind.Quantity ?? 0));
                }
                var accountCode = goodDetail.Detail1;
                var accountParent = goodDetail.Account;

                if (!string.IsNullOrEmpty(goodDetail.Detail2))
                {
                    accountCode = goodDetail.Detail2;
                    accountParent = $"{goodDetail.Account}:{goodDetail.Detail1}";
                }
                var stockUnit = await _context.ChartOfAccounts.Where(x => x.Code == accountCode && x.ParentRef == accountParent).Select(x => x.StockUnit).FirstOrDefaultAsync();

                var rowSb = new StringBuilder(rowTemplateElement.InnerHtml)
                    .Replace("[Index]", rIndex.ToString())
                    .Replace("[Code]", GoodNameGetter.GetCodeFromGoodDetail(goodDetail))
                    .Replace("[Name]", GoodNameGetter.GetNameFromGoodDetail(goodDetail))
                    .Replace("[StockUnit]", stockUnit)
                    .Replace("[QuantityRequired]", quantityRequired.ToString("n2"))
                    .Replace("[QuantityReal]", quantityReal.ToString("n2"))
                    .Replace("[UnitPrice]", "-")
                    .Replace("[Amount]", "-");
                tblDetailSb.Append(rowSb);
                rIndex++;
                goodDetailChecks.Add(goodDetail);
            }

            // Remove template section after mapping
            rowTemplateElement.Remove();

            mainSb.Replace("[TABLE_BODY_SECTION]", tblDetailSb.ToString());

            var signingHtml = await _procedureExportHelper.SignPlace(id, nameof(ProcedureEnum.MANUFACTURE_ORDER));

            // Mapping Signing section
            mainSb.Replace("[SIGN_REPLACE_PLACE]", signingHtml);

            var html = mainSb.ToString();
            var generatedFileName = ExcelHelpers.ConvertUseDink(html, _converterPDF, Directory.GetCurrentDirectory(),
                "ExportProcessProduce");
            return generatedFileName;
        }
    }
}
