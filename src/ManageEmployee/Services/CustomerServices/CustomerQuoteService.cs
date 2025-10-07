using AutoMapper;
using Common.Constants;
using Common.Helpers;
using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Helpers;
using ManageEmployee.Hubs;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;

namespace ManageEmployee.Services.CustomerServices;
public class CustomerQuoteService: ICustomerQuoteService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
    private readonly IConverter _converterPDF;
    private readonly AppSettings _appSettings;
    private readonly ICompanyService _companyService;
    public CustomerQuoteService(IMapper mapper,
        ApplicationDbContext context,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IConverter converterPDF,
        IOptions<AppSettings> appSettings, ICompanyService companyService)
    {
        _mapper = mapper;
        _context = context;
        _hubContext = hubContext;
        _converterPDF = converterPDF;
        _appSettings = appSettings.Value;
        _companyService = companyService;
    }

    private async Task<List<CustomerQuote_DetailModel>> CreateCustomerQuote(List<BillDetailModel> requests, int customerId, string note, int userId, int year)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            List<CustomerQuote_DetailModel> listItem = new List<CustomerQuote_DetailModel>();
            var goodsIds = requests.Select(x => x.GoodsId);
            List<Goods> goods = await _context.Goods.Where(x => !x.IsDeleted && goodsIds.Contains(x.Id)).ToListAsync();
            List<ChartOfAccount> accountList = await _context.GetChartOfAccount(year).Where(x => x.ParentRef.Contains("1561") && !x.HasDetails && !x.HasChild).ToListAsync();

            CustomerQuote item = new CustomerQuote();
            item.CustomerId = customerId;
            item.CreateDate = DateTime.Now;
            item.Note = note;
            await _context.CustomerQuote.AddAsync(item);
            await _context.SaveChangesAsync();

            foreach (var request in requests)
            {
                CustomerQuote_DetailModel item_detail = new();
                item_detail.IdCustomerQuote = item.Id;
                item_detail.GoodsId = request.GoodsId;
                item_detail.UnitPrice = request.UnitPrice ?? 0;
                item_detail.DiscountPrice = request.DiscountPrice ?? 0;
                item_detail.CreateDate = DateTime.Now;
                item_detail.TaxVAT = request.TaxVAT ?? 0;
                item_detail.Quantity = request.Quantity;
                item_detail.Note = "";
                var good = goods.Find(x => x.Id == request.GoodsId);
                if (good != null)
                {
                    item_detail.GoodsName = GoodNameGetter.GetNameFromGood(good);
                    string accountCode = GoodNameGetter.GetCodeFromGood(good);
                    var account = accountList.Find(x => x.Code == accountCode);
                    if (account != null)
                        item_detail.StockUnit = account.StockUnit;
                }

                listItem.Add(item_detail);
            }
            var customerQuoteDetails = _mapper.Map<List<CustomerQuote_Detail>>(listItem);
            await _context.CustomerQuote_Detail.AddRangeAsync(customerQuoteDetails);

            // add task
            var customer = await _context.Customers.FindAsync(customerId);
            var userTask = new UserTask
            {
                Name = "Gửi báo giá khách hàng " + customer?.Name,
                UserCreated = userId,
                CustomerId = customerId,
                CreatedDate = DateTime.Now,
                DueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 17, 0, 0),
                Status = 0,
                IsDeleted = false
            };
            await _context.UserTasks.AddAsync(userTask);
            await _context.SaveChangesAsync();

            var taskRole = new UserTaskRoleDetails
            {
                UserId = userId,
                UserTaskId = userTask.Id,
                UserTaskRoleId = 1
            };
            await _context.UserTaskRoleDetails.AddAsync(taskRole);
            var user = await _context.Users.FindAsync(userId);
            BillTracking billTracking = new()
            {
                BillId = 0,
                CustomerName = customer.Name,
                UserCode = userId.ToString(),
                UserIdReceived = userId,
                Note = "<strong>" + userTask.Name + "</strong><br/>" + user.FullName + "<br/>" + DateTime.Now.ToString("dd/MM/yyyy : HH:mm"),
                TranType = TranTypeConst.SendToStaff,
                Status = "Success",
                IsRead = false,
                DisplayOrder = 0,
            };
            _context.BillTrackings.Add(billTracking);
            await _context.SaveChangesAsync();

            _context.Database.CommitTransaction();
            await _hubContext.Clients.All.BroadcastMessage();
            return listItem;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task<PagingResult<CustomerQuoteModel>> GetListCustomerQuoteHistory(CustomerQuoteSearchModel search)
    {
        try
        {
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            var query = _context.CustomerQuote.Where(x => x.CustomerId == search.CustomerId);
            var customer = _context.Customers.Find(search.CustomerId);
            if (search.FromDate > 0)
            {
                fromDate = DateHelpers.UnixTimeStampToDateTime(search.FromDate ?? 0).Date;
                query = query.Where(x => x.CreateDate >= fromDate);
            }
            if (search.ToDate > 0)
            {
                toDate = DateHelpers.UnixTimeStampToDateTime(search.ToDate ?? 0).Date;
                query = query.Where(x => x.CreateDate <= toDate);
            }
            var data_return = await query.Skip((search.Page - 1) * search.PageSize).Take(search.PageSize).ToListAsync();
            var listQuoteId = data_return.Select(x => x.Id).ToList();
            var listQuoteHistory = await _context.CustomerQuote_Detail.Where(x => listQuoteId.Contains(x.IdCustomerQuote)).ToListAsync();

            var itemOuts = new List<CustomerQuoteModel>();
            foreach (var data in data_return)
            {
                var quoteHistorys = listQuoteHistory.Where(x => x.IdCustomerQuote == data.Id).ToList();
                var itemOut = _mapper.Map<CustomerQuoteModel>(data);
                itemOut.Quantity = quoteHistorys.Sum(x => x.Quantity);
                itemOut.TotalPrice = quoteHistorys.Sum(x => x.Quantity * x.UnitPrice);
                itemOut.CustomerName = customer.Name;
                itemOuts.Add(itemOut);
            }
            return new PagingResult<CustomerQuoteModel>()
            {
                CurrentPage = search.Page,
                PageSize = search.PageSize,
                TotalItems = await query.CountAsync(),
                Data = itemOuts
            };
        }
        catch
        {
            return new PagingResult<CustomerQuoteModel>()
            {
                CurrentPage = search.Page,
                PageSize = search.PageSize,
                TotalItems = 0,
                Data = new List<CustomerQuoteModel>()
            };
        }
    }

    public async Task<List<CustomerQuote_DetailModel>> GetListCustomerQuoteDetail(long CustomerQuoteId)
    {
        var listData = await _context.CustomerQuote_Detail.Where(X => X.IdCustomerQuote == CustomerQuoteId).ToListAsync();
        var listGoodId = listData.Select(x => x.GoodsId).ToList();
        List<Goods> goods = await _context.Goods.Where(x => listGoodId.Contains(x.Id)).ToListAsync();

        var customerQuoteDetails = _mapper.Map<List<CustomerQuote_DetailModel>>(listData);
        foreach (var data in customerQuoteDetails)
        {
            var good = goods.Find(x => x.Id == data.GoodsId);
            if (good != null)
            {
                data.GoodsName = GoodNameGetter.GetNameFromGood(good);
            }
        }
        return customerQuoteDetails;
    }

    public async Task<string> ConvertToHTML_BaoGia(long CustomerQuoteId, string type, int customerId)
    {
        try
        {
            var customerQuote = await _context.CustomerQuote.FindAsync(CustomerQuoteId);
            List<CustomerQuote_Detail> customerQuoteDetails = await _context.CustomerQuote_Detail.Where(x => x.IdCustomerQuote == CustomerQuoteId).ToListAsync();
            List<Goods> goods = await _context.Goods.Where(x => !x.IsDeleted).ToListAsync();

            var listQuote = _mapper.Map<List<CustomerQuote_DetailModel>>(customerQuoteDetails);
            foreach (var quote in listQuote)
            {
                var good = goods.Find(x => x.Id == quote.GoodsId);
                if (good != null)
                {
                    quote.GoodsName = (!string.IsNullOrEmpty(good.DetailName2) ? good.DetailName2 : (!string.IsNullOrEmpty(good.DetailName1) ? good.DetailName1 : good.AccountName));
                    quote.Image = good.Image1;
                    quote.StockUnit = good.StockUnit;
                }
            }
            Company p = _context.Companies.FirstOrDefault();

            string _template = "BaoGiaTemplate.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;
            string noteAll = customerQuote.Note;

            if (!string.IsNullOrEmpty(customerQuote.Note) && customerQuote.Note.Contains("\n"))
            {
                string[] notes = customerQuote.Note.Split('\n');
                noteAll = "";
                foreach (string note in notes)
                    noteAll += note + "<br/>";
            }

            string path_logo = _appSettings.UrlHost;
            path_logo += p.FileLogo;

            var customer = await _context.Customers.FindAsync(customerId);
            var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == customerId);
            string NgayBaoGia = "Ngày " + listQuote[0].CreateDate.Day + " Tháng " + listQuote[0].CreateDate.Month + " Năm " + listQuote[0].CreateDate.Year;
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Name },
                { "DiaChi", p.Address },
                { "SoDienThoai", p.Phone },
                { "WebsiteName", p.WebsiteName },
                { "Email", p.Email },
                { "NoteOfCEO", p.NoteOfCEO},
                { "NameOfCEO", p.NameOfCEO},
                { "listNote", noteAll},
                { "NgayBaoGia", NgayBaoGia},

                { "CustomerName", customer.Name + (string.IsNullOrEmpty(customerTax?.CompanyName) ? "" : (" - " + customerTax.CompanyName)) },
                { "CustomerAddress", customer.Address},
                { "CustomerPhone", customer.Phone},
                { "CustomerTax", customerTax?.TaxCode},
                { "LoGoCongTy", path_logo},
                { "CompanyTax", p.MST},
                { "BaoGiaNote", p.Note},
            };
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            int i = 0;
            listQuote.ForEach(x =>
            {
                i++;
                string _txt = @"<tr>
                                            <td class='txt-center'>{{{STT}}}</td>
                                            <td class='txt-center'><img src='{{{HINHANH}}}' height='60' alt='HinhAnh'/></td>
                                            <td colspan='2' class='txt-left'>{{{TENHANG}}}</td>
                                            <td class='txt-center'>{{{DVT}}}</td>
                                            <td class='txt-center'>{{{SOLUONG}}}</td>
                                            <td class='txt-right'>{{{DONGIA}}}</td>
                                            <td class='txt-right'>{{{THANHTIEN}}}</td>
                                        </tr>";

                _txt = _txt.Replace("{{{STT}}}", i.ToString())
                                    .Replace("{{{HINHANH}}}", _appSettings.UrlHost + x.Image)
                                    .Replace("{{{TENHANG}}}", x.GoodsName)
                                    .Replace("{{{DVT}}}", x.StockUnit)
                                    .Replace("{{{SOLUONG}}}", String.Format("{0:N0}", x.Quantity))
                                    .Replace("{{{DONGIA}}}", String.Format("{0:N0}", x.UnitPrice))
                                    .Replace("{{{THANHTIEN}}}", String.Format("{0:N0}", x.Quantity * x.UnitPrice))
                                    ;

                resultHTML += _txt;
            });
            string _tr_Sum = @"<tr class='font - b'>
                              <td colspan = '7' class='txt-left font-b'>TỔNG CỘNG </td>
                                  <td class='txt-right font-b'>{{{TONG_TIEN}}}</td>
                            </tr>    ";
            _tr_Sum = _tr_Sum.Replace("{{{TONG_TIEN}}}", String.Format("{0:N0}", listQuote.Sum(x => x.Quantity * x.UnitPrice)));

            resultHTML += _tr_Sum;

            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);

            if (type == "html")
            {
                return _allText;
            }

            return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "BaoGia");
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<(object customerQuote, List<CustomerQuote_DetailModel> customerQuoteDetails)> GetDataBaoGia(long CustomerQuoteId)
    {
        var customerQuote = await _context.CustomerQuote.FindAsync(CustomerQuoteId);
        List<CustomerQuote_Detail> listQuote = await _context.CustomerQuote_Detail.Where(x => x.IdCustomerQuote == CustomerQuoteId).ToListAsync();
        List<Goods> goods = await _context.Goods.Where(x => !x.IsDeleted).ToListAsync();

        var customerQuoteDetails = _mapper.Map<List<CustomerQuote_DetailModel>>(listQuote);
        foreach (var quote in customerQuoteDetails)
        {
            var good = goods.Find(x => x.Id == quote.GoodsId);
            if (good != null)
                quote.GoodsName = (!string.IsNullOrEmpty(good.DetailName2) ? good.DetailName2 : (!string.IsNullOrEmpty(good.DetailName1) ? good.DetailName1 : good.AccountName));
        }

        string noteAll = customerQuote.Note;

        if (!string.IsNullOrEmpty(customerQuote.Note) && customerQuote.Note.Contains("\n"))
        {
            string[] notes = customerQuote.Note.Split('\n');
            noteAll = "";
            foreach (string note in notes)
                noteAll += note + "<br/>";
        }
        var company = await _context.Companies.FirstOrDefaultAsync();
        var info = new
        {
            Note = noteAll,
            NameOfCEO = company.NameOfCEO
        };

        return (info, customerQuoteDetails);
    }

    public async Task<object> ExportCustomerQuote(List<BillDetailModel> model, int customerId, int userId, int year)
    {
        string _fileMapServer = $"DanhSachKhachHang_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
             folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\EXCEL"),
                _pathSave = Path.Combine(folder, _fileMapServer);
        var company = await _companyService.GetCompany();

        List<CustomerQuote_DetailModel> result = await CreateCustomerQuote(model, customerId, company.Note, userId, year);

        if (result.Any())
        {

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\MAU-BAO-GIA.xlsx");

            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets[0];
                    int nRowBegin = 16, rowIdx = 16;

                    var pic = sheet.Drawings.AddPicture("MyPhoto", new FileInfo(company.FileLogo));
                    pic.SetPosition(2, 0, 2, 0);
                    sheet.Cells["F2"].Value = company.Name.ToUpper();
                    sheet.Cells["F3"].Value = company.Address;
                    sheet.Cells["F4"].Value = company.Phone;
                    sheet.Cells["F5"].Value = company.WebsiteName;
                    sheet.Cells["F6"].Value = company.Email;

                    var customer = _context.Customers.Find(customerId);
                    if (customer != null)
                    {
                        var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == customerId);

                        sheet.Cells["B10"].Value = "Khách hàng: " + customer.Name + (string.IsNullOrEmpty(customerTax?.CompanyName) ? "" : (" - " + customerTax?.CompanyName));
                        sheet.Cells["B11"].Value = "Địa chỉ: " + customer.Address;
                        sheet.Cells["B12"].Value = "SĐT: " + customer.Phone;
                        sheet.Cells["B13"].Value = "MST: " + customerTax?.TaxCode;
                    }
                    foreach (CustomerQuote_DetailModel lo in result)
                    {
                        sheet.Cells[rowIdx, 2].Value = rowIdx - 15;
                        sheet.Cells[rowIdx, 3].Value = lo.GoodsName;
                        sheet.Cells[rowIdx, 3, rowIdx, 6].Merge = true;
                        sheet.Cells[rowIdx, 7].Value = lo.StockUnit;
                        sheet.Cells[rowIdx, 8].Value = lo.Quantity;
                        sheet.Cells[rowIdx, 9].Value = lo.UnitPrice;
                        sheet.Cells[rowIdx, 10].Value = lo.UnitPrice * lo.Quantity;
                        sheet.Cells[rowIdx, 11].Value = lo.Note;
                        sheet.Cells[rowIdx, 11, rowIdx, 12].Merge = true;
                        rowIdx++;
                    }
                    sheet.Cells[rowIdx, 2].Value = "TỔNG CỘNG:";
                    sheet.Cells[rowIdx, 2, rowIdx, 9].Merge = true;
                    sheet.Cells[rowIdx, 2].Style.Font.Bold = true;
                    sheet.Cells[rowIdx, 10].Value = result.Sum(x => x.Quantity * x.UnitPrice);
                    sheet.Cells[rowIdx, 10].Style.Font.Bold = true;
                    sheet.Cells[rowIdx, 11].Value = "";
                    sheet.Cells[rowIdx, 11, rowIdx, 12].Merge = true;

                    if (rowIdx >= nRowBegin)
                    {
                        sheet.Cells[nRowBegin, 8, rowIdx, 10].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 2, rowIdx, 12].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    rowIdx = rowIdx + 2;
                    DateTime dt = DateTime.Today;
                    sheet.Cells[rowIdx, 7].Value = "Ngày " + dt.Day + " Tháng " + dt.Month + " Năm " + dt.Year;
                    sheet.Cells[rowIdx, 7, rowIdx, 12].Merge = true;
                    sheet.Cells[rowIdx, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    rowIdx++;
                    sheet.Cells[rowIdx, 7].Value = company.NoteOfCEO;
                    sheet.Cells[rowIdx, 7].Style.Font.Bold = true;
                    sheet.Cells[rowIdx, 7, rowIdx, 12].Merge = true;
                    sheet.Cells[rowIdx, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    rowIdx = rowIdx + 5;

                    sheet.Cells[rowIdx, 7].Value = company.NameOfCEO;
                    sheet.Cells[rowIdx, 7].Style.Font.Bold = true;
                    sheet.Cells[rowIdx, 7, rowIdx, 12].Merge = true;
                    sheet.Cells[rowIdx, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    rowIdx = rowIdx + 2;
                    sheet.Cells[rowIdx, 2].Value = "LƯU Ý:";
                    sheet.Cells[rowIdx, 2].Style.Font.Bold = true;
                    string[] listNote = company.Note.Split('\n');
                    if (listNote.Any())
                    {
                        foreach (string note in listNote)
                        {
                            sheet.Cells[rowIdx + 1, 3].Value = note;
                            rowIdx++;
                        }
                    }

                    using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                    {
                        package.SaveAs(fs);
                    }
                }

            }
        }
        return new
        {
            data = _fileMapServer,
            id = result.First().IdCustomerQuote
        };
    }
}
