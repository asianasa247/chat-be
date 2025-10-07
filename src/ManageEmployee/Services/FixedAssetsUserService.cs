using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.DataTransferObject.FixedAssetsModels;

namespace ManageEmployee.Services;


public class FixedAssetsUserService : IFixedAssetsUserService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    public FixedAssetsUserService(ApplicationDbContext context, IMapper mapper, IChartOfAccountV2Service chartOfAccountV2Service)
    {
        _context = context;
        _mapper = mapper;
        _chartOfAccountV2Service = chartOfAccountV2Service;
    }

    public IQueryable<FixedAssetViewModel> GetListEdit(PagingRequestModel searchRequest)
    {
        return (from f in _context.FixedAssetUser
                join _d in _context.Departments on f.DepartmentId equals _d.Id into _d
                from ds in _d.DefaultIfEmpty()
                join _u in _context.Users on f.UserId equals _u.Id into _u
                from us in _u.DefaultIfEmpty()
                where (string.IsNullOrEmpty(searchRequest.SearchText)
                        || (!string.IsNullOrEmpty(f.Name) && f.Name.ToLower().Contains(searchRequest.SearchText.ToLower())))
                select new FixedAssetViewModel
                {
                    Id = f.Id,
                    DepartmentManagerName = ds.Name,
                    UserManagerName = us.FullName,
                    CreditCode = f.CreditCode,
                    CreditCodeName = f.CreditCodeName,
                    CreditDetailCodeFirst = f.CreditDetailCodeFirst,
                    CreditDetailCodeSecond = f.CreditDetailCodeSecond,
                    CreditDetailCodeFirstName = f.CreditDetailCodeFirstName,
                    CreditDetailCodeSecondName = f.CreditDetailCodeSecondName,
                    UsedDate = f.UsedDate,
                    BuyDate = f.BuyDate,
                    CarryingAmount = f.CarryingAmount,
                    TotalMonth = f.TotalMonth,
                    Use = f.Use,
                    HistoricalCost = f.HistoricalCost,
                });
    }
    public async Task<FixedAssetUser> GetById(int id)
    {
        return await _context.FixedAssetUser.FindAsync(id);
    }
    public async Task<FixedAssetUserGetterModel> GetByIdV2(int id, int year)
    {
        var item = await _context.FixedAssetUser.FindAsync(id);
        var itemOut = _mapper.Map<FixedAssetUserGetterModel>(item);
        itemOut.Debit = await _chartOfAccountV2Service.FindAccount(item.DebitCode, string.Empty, year);
        itemOut.DebitDetailFirst = await _chartOfAccountV2Service.FindAccount(item.DebitDetailCodeFirst, item.DebitCode, year);
        itemOut.DebitDetailSecond = await _chartOfAccountV2Service.FindAccount(item.DebitDetailCodeSecond, item.DebitCode + ":" + item.DebitDetailCodeFirst, year);


        itemOut.Credit = await _chartOfAccountV2Service.FindAccount(item.CreditCode, string.Empty, year);
        itemOut.CreditDetailFirst = await _chartOfAccountV2Service.FindAccount(item.CreditDetailCodeFirst, item.CreditCode, year);
        itemOut.CreditDetailSecond = await _chartOfAccountV2Service.FindAccount(item.DebitDetailCodeSecond, item.CreditCode + ":" + item.CreditDetailCodeFirst, year);

        return itemOut;
    }

    public async Task<FixedAssetUser> UpdateEdit(FixedAssetUser entity)
    {
        var asset = await _context.FixedAssetUser.FindAsync(entity.Id);
        if (asset is null)
            throw new Exception("");
        asset.Name = entity.Name;
        asset.TotalMonth = entity.TotalMonth;
        asset.HistoricalCost = entity.HistoricalCost;
        asset.TotalDayDepreciationOfThisPeriod = entity.TotalDayDepreciationOfThisPeriod;
        asset.DepreciationOfOneDay = entity.DepreciationOfOneDay;
        asset.DepreciationOfThisPeriod = entity.DepreciationOfThisPeriod;
        asset.CarryingAmount = entity.CarryingAmount;
        asset.DebitCode = entity.DebitCode;
        asset.DebitDetailCodeFirst = entity.DebitDetailCodeFirst;
        asset.DebitDetailCodeSecond = entity.DebitDetailCodeSecond;
        asset.CreditCode = entity.CreditCode;
        asset.CreditDetailCodeFirst = entity.CreditDetailCodeFirst;
        asset.CreditDetailCodeSecond = entity.CreditDetailCodeSecond;
        asset.DepartmentId = entity.DepartmentId;
        asset.UserId = entity.UserId;
        asset.BuyDate = entity.BuyDate;
        asset.Use = entity.Use;
        asset.Note = entity.Note;
        asset.LiquidationDate = entity.LiquidationDate;
        _context.Update(asset);
        await _context.SaveChangesAsync();

        return asset;
    }

    public string Delete(int id)
    {
        string message = "";
        var asset = _context.FixedAssetUser.Find(id);
        if (asset == null)
        {
            return ErrorMessages.DataNotFound;
        }

        try
        {
            _context.FixedAssetUser.Remove(asset);

            _context.SaveChanges();
        }
        catch
        {
            message = ErrorMessages.NotDeletedAll;
        }

        return message;
    }

    public async Task<string> ExportExcel(PagingRequestModel searchRequest)
    {
        var fixedAssets = GetListEdit(searchRequest);
        var datas = await fixedAssets.ToListAsync();

        string _fileMapServer = $"CongCuDungCu_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                   folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                   _pathSave = Path.Combine(folder, _fileMapServer);

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\FixAsset.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 6;
                int nCol = 12;
                int rowIdx = nRowBegin;
                if (datas.Any())
                {
                    int i = 0;
                    foreach (var item in datas)
                    {
                        i++;
                        sheet.Cells[rowIdx, 1].Value = i.ToString();
                        sheet.Cells[rowIdx, 2].Value = item.CreditCode;
                        sheet.Cells[rowIdx, 3].Value = item.CreditCodeName;
                        sheet.Cells[rowIdx, 4].Value = item.CreditDetailCodeFirst;
                        sheet.Cells[rowIdx, 5].Value = item.CreditDetailCodeFirstName;
                        sheet.Cells[rowIdx, 6].Value = item.CreditDetailCodeSecond;
                        sheet.Cells[rowIdx, 7].Value = item.CreditDetailCodeSecondName;
                        sheet.Cells[rowIdx, 8].Value = (item.UsedDate ?? DateTime.Now).ToString("dd/MM/yyyy");
                        sheet.Cells[rowIdx, 9].Value = item.Quantity;
                        sheet.Cells[rowIdx, 10].Value = item.CarryingAmount;
                        sheet.Cells[rowIdx, 11].Value = item.TotalMonth;
                        sheet.Cells[rowIdx, 12].Value = item.Use == 1 ? "Có" : "Không";
                        rowIdx++;
                    }
                    var loaiUse = sheet.Cells[nRowBegin, 12, rowIdx, 12].DataValidation.AddListDataValidation();
                    loaiUse.Formula.Values.Add("Có");
                    loaiUse.Formula.Values.Add("Không");
                }

                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 9, rowIdx, 10].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }

            }
        }
        return _fileMapServer;
    }

    public string ImportExcel(List<FixedAssetViewModel> datas)
    {
        foreach (var data in datas)
        {
            FixedAssetUser fixAsset = _mapper.Map<FixedAssetUser>(data);
            _context.FixedAssetUser.Add(fixAsset);
        }

        _context.SaveChanges();
        return string.Empty;
    }

    public async Task<FixedAssetUser> Create(FixedAssetUser entity, int year)
    {
        var account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.CreditCode);
        FixedAssetUser asset = new();
        asset.Name = entity.Name;
        asset.TotalMonth = entity.TotalMonth;
        asset.HistoricalCost = entity.HistoricalCost;
        asset.TotalDayDepreciationOfThisPeriod = entity.TotalDayDepreciationOfThisPeriod;
        asset.DepreciationOfOneDay = entity.DepreciationOfOneDay;
        asset.DepreciationOfThisPeriod = entity.DepreciationOfThisPeriod;
        asset.CarryingAmount = entity.CarryingAmount;
        asset.DebitCode = entity.DebitCode;
        asset.DebitDetailCodeFirst = entity.DebitDetailCodeFirst;
        asset.DebitDetailCodeSecond = entity.DebitDetailCodeSecond;
        asset.CreditCode = entity.CreditCode;
        asset.CreditDetailCodeFirst = entity.CreditDetailCodeFirst;
        asset.CreditDetailCodeSecond = entity.CreditDetailCodeSecond;
        asset.DepartmentId = entity.DepartmentId;
        asset.UserId = entity.UserId;
        asset.UsedDate = entity.UsedDate;
        asset.Use = entity.Use;
        asset.Type = account?.Classification == 4 ? "PB" : (account?.Classification == 5 ? "KH" : "");
        string userCodeString = _context.FixedAssetUser.OrderByDescending(x => x.UsedCode).FirstOrDefault()?.UsedCode ?? "0";
        int userCode = int.Parse(userCodeString);
        asset.UsedCode = userCode.ToString();

        _context.Add(asset);
        await _context.SaveChangesAsync();

        return asset;
    }

}