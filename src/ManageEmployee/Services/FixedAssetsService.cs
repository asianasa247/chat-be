using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Handlers;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

public class FixedAssetService : IFixedAssetsService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly IMapper _mapper;
    private readonly ILedgerService _ledgerServices;
    private readonly ILedgerFixedAssetService _ledgerFixedAssetService;
    private readonly IFixedAssets242Service _fixedAssets242Service;

    public FixedAssetService(ApplicationDbContext context, IChartOfAccountService chartOfAccountService, IMapper mapper, ILedgerService ledgerServices, ILedgerFixedAssetService ledgerFixedAssetService, IFixedAssets242Service fixedAssets242Service)
    {
        _context = context;
        _chartOfAccountService = chartOfAccountService;
        _mapper = mapper;
        _ledgerServices = ledgerServices;
        _ledgerFixedAssetService = ledgerFixedAssetService;
        _fixedAssets242Service = fixedAssets242Service;
    }

    public async Task<FixedAsset> UpdateFromLedger(Ledger ledger, int year)
    {
        string strDebitCode = ledger.DebitCode ?? string.Empty;
        string strDebitDetailCode = ledger.DebitDetailCodeFirst;
        string strDebitParentCode = strDebitCode;
        if (!string.IsNullOrEmpty(ledger.DebitDetailCodeSecond))
        {
            strDebitDetailCode = ledger.DebitDetailCodeSecond;
            strDebitParentCode = strDebitCode + ":" + ledger.DebitDetailCodeFirst;
        }
        ChartOfAccount acctDebit = await _chartOfAccountService.GetAccountByCode(strDebitCode, year);
        ChartOfAccount acctDebitDetail = await _chartOfAccountService.GetAccountByCode(strDebitDetailCode, year, strDebitParentCode);
        if (acctDebitDetail is null)
            acctDebitDetail = acctDebit;

        if ((acctDebitDetail.Classification == (int)AssetsType.PB || acctDebitDetail.Classification == (int)AssetsType.KH) && ledger.DepreciaMonth > 0)
        {
            FixedAssetsModel assetModel = new FixedAssetsModel().FromLedger(ledger);
            assetModel.Name = (!string.IsNullOrEmpty(assetModel.CreditDetailCodeSecondName) ? assetModel.CreditDetailCodeSecondName : (!string.IsNullOrEmpty(assetModel.CreditDetailCodeFirstName) ? assetModel.CreditDetailCodeFirstName : assetModel.CreditCodeName));
            assetModel.CreditCode = ledger?.DebitCode;
            assetModel.CreditCodeName = ledger?.DebitCodeName;

            assetModel.CreditDetailCodeFirst = ledger?.DebitDetailCodeFirst;
            assetModel.CreditDetailCodeFirstName = ledger?.DebitDetailCodeFirstName;

            assetModel.CreditDetailCodeSecond = ledger?.DebitDetailCodeSecond;
            assetModel.CreditDetailCodeSecondName = ledger?.DebitDetailCodeSecondName;

            assetModel.Use = 1;
            assetModel.CarryingAmount = assetModel.HistoricalCost - (assetModel.DepreciationOfThisPeriod ?? 0);

            assetModel.Type = Enum.Parse(typeof(AssetsType), acctDebitDetail.Classification.ToString()).ToString();
            FixedAsset asset = _mapper.Map<FixedAsset>(assetModel);

            _context.FixedAssets.Add(asset);
            await _context.SaveChangesAsync();

            // add LedgerFixedAsset
            LedgerFixedAsset ledgerFixedAsset = new LedgerFixedAsset()
            {
                LedgerId = ledger.Id,
                FixedAssetId = asset.Id,
                IsInternal = ledger.IsInternal
            };
            await _ledgerFixedAssetService.CreateAsync(ledgerFixedAsset);
            return GetById(asset.Id).Result;
        }
        else
        {
            await _fixedAssets242Service.UpdateFromLedger(ledger, year);

        }
        return null;
    }

    public async Task<List<FixedAssetsModelEdit>> GetListEdit(AssetsType eType, int iMonth, int iYear, int isInternal)
    {
        DateTime dtPeriodDate = new DateTime(iYear, iMonth, 1);
        var lst = await _context.FixedAssets
                        .GroupJoin(_context.LedgerFixedAssets,
                                    f => f.Id,
                                    l => l.FixedAssetId,
                                    (f, l) => new { fixedAsset = f, ledger = l})
                        .SelectMany(x => x.ledger.DefaultIfEmpty(), (f, l) => new { fixedAsset = f.fixedAsset, isInternal = l.IsInternal })
                        .Where(x => (isInternal == 3 ? x.isInternal == 3 : x.isInternal == 2) || x.isInternal == 1)
                            .Where(t => t.fixedAsset.Use == 1)
                               .Where(t => t.fixedAsset.Type == eType.ToString() && (t.fixedAsset.TotalMonth ?? 0) > 0
                                           && t.fixedAsset.BuyDate.Value < dtPeriodDate.AddMonths(1)
                                )
                               .Select(t => _mapper.Map<FixedAssetsModelEdit>(t.fixedAsset))
                               .Distinct()
                               .ToListAsync();
        return lst;
    }

    public async Task<FixedAsset> GetById(int id)
    {
        return await _context.FixedAssets.FindAsync(id);
    }

    public async Task<FixedAssetsModelEdit> UpdateEdit(FixedAssetsModelEdit entity, int year)
    {
        FixedAsset asset = new FixedAsset();

        if (entity.Id > 0)
        {
            asset = await _context.FixedAssets.FindAsync(entity.Id);
        }
        if (asset != null)
        {
            asset.CreditCode = entity.CreditCode;
            asset.CreditDetailCodeFirst = entity.CreditDetailCodeFirst;
            asset.CreditDetailCodeSecond = entity.CreditDetailCodeSecond;
            asset.BuyDate = entity.BuyDate;
            asset.Quantity = entity.Quantity;
            asset.HistoricalCost = entity.HistoricalCost;
            asset.UnitPrice = entity.UnitPrice;
            asset.TotalMonth = entity.TotalMonth;
            asset.Use = entity.Use;
            await UpdateChartOfAccountName(asset, year);
            asset.Name = !string.IsNullOrEmpty(asset.CreditDetailCodeSecondName) ? asset.CreditDetailCodeSecondName :
                (!string.IsNullOrEmpty(asset.CreditDetailCodeFirstName) ? asset.CreditDetailCodeFirstName : asset.CreditCodeName);

            if (entity.Id > 0)
                _context.FixedAssets.Update(asset);
            else
                _context.FixedAssets.Add(asset);
            await _context.SaveChangesAsync();
        }

        return null;
    }

    public async Task AddFixedAsset242FromFixedAsset(List<FixedAssetsModelEdit> entities, int year)
    {
        var fixedAsset242s = new List<FixedAsset242>();
        foreach (var entity in entities)
        {
            var account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.CreditCode);

            string strDebitCode = entity.CreditCode ?? string.Empty;
            string strDebitDetailCode = entity.CreditDetailCodeFirst;
            string strDebitParentCode = strDebitCode;
            if (!string.IsNullOrEmpty(entity.CreditDetailCodeSecond))
            {
                strDebitDetailCode = entity.DebitDetailCodeSecond;
                strDebitParentCode = strDebitCode + ":" + entity.DebitDetailCodeFirst;
            }
            ChartOfAccount acctDebit = await _chartOfAccountService.GetAccountByCode(strDebitCode, year);
            ChartOfAccount acctDebitDetail = await _chartOfAccountService.GetAccountByCode(strDebitDetailCode, year, strDebitParentCode);
            if (acctDebitDetail is null)
                acctDebitDetail = acctDebit;
            if (acctDebitDetail.Classification == (int)AssetsType.CCDCSD || acctDebitDetail.Classification == (int)AssetsType.KH || acctDebitDetail.Classification == (int)AssetsType.PB)
            {
                FixedAsset242 asset = new FixedAsset242();

                asset.Id = 0;
                asset.TotalMonth = entity.TotalMonth;
                asset.HistoricalCost = entity.HistoricalCost;
                asset.TotalDayDepreciationOfThisPeriod = entity.TotalDayDepreciationOfThisPeriod;
                asset.DepreciationOfOneDay = entity.DepreciationOfOneDay;
                asset.DepreciationOfThisPeriod = entity.DepreciationOfThisPeriod;
                asset.CarryingAmount = entity.CarryingAmount;
                asset.DebitCode = "";
                asset.DebitDetailCodeFirst = "";
                asset.DebitDetailCodeSecond = "";
                asset.CreditCode = entity.DebitCode;
                asset.CreditDetailCodeFirst = entity.DebitDetailCodeFirst;
                asset.CreditDetailCodeSecond = entity.DebitDetailCodeSecond;

                asset.DepartmentId = entity.DepartmentId;
                asset.UserId = entity.UserId;
                asset.UsedDate = entity.UsedDate;
                asset.Use = entity.Use;
                asset.UnitPrice = entity.UnitPrice;
                asset.Quantity = entity.UsedQuantity;
                asset.Type = acctDebitDetail?.Classification == 4 ? "PB" : (acctDebitDetail?.Classification == 5 ? "KH" : "");

                fixedAsset242s.Add(asset);
                await UpdateChartOfAccountName242(asset, year);
                _context.FixedAsset242.Add(asset);

                string userCodeString = _context.FixedAssetUser.OrderByDescending(x => x.UsedCode).FirstOrDefault()?.UsedCode ?? "0";
                int userCode = int.Parse(userCodeString);
                for (int i = 0; i < asset.Quantity; i++)
                {
                    userCode++;
                    var assetUser = _mapper.Map<FixedAssetUser>(asset);
                    assetUser.Quantity = 1;
                    assetUser.UsedCode = userCode.ToString();
                    _context.FixedAssetUser.Add(assetUser);
                }
                await _context.SaveChangesAsync();

                var fixedAsset = _context.FixedAssets.Find(entity.Id);
                if (fixedAsset != null)
                {
                    double quantity = (entity.Quantity ?? 0) - (entity.UsedQuantity ?? 0);
                    if (quantity > 0)
                    {
                        fixedAsset.Quantity = quantity;
                        _context.FixedAssets.Update(fixedAsset);
                    }
                    else
                    {
                        _context.FixedAssets.Remove(fixedAsset);
                        var ledgerFixedAsset = await _context.LedgerFixedAssets.FirstOrDefaultAsync(x => x.FixedAssetId == fixedAsset.Id);
                        if (ledgerFixedAsset != null)
                        {
                            ledgerFixedAsset.FixedAsset242Id = asset.Id;
                            _context.LedgerFixedAssets.Update(ledgerFixedAsset);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Không phải tài khoản ccdc");
            }
        }
        await _context.SaveChangesAsync();

        int j = 0;
        var ledgers = entities.ConvertAll(x =>
        {
            var y = _mapper.Map<FixedAssetsModelEdit>(x);
            y.PeriodDate = x.UsedDate ?? DateTime.UtcNow;
            y.Id = fixedAsset242s[j].Id;
            j++;
            return y;
        });
        foreach (var ledger in ledgers)
        {
            await _ledgerServices.CreateFromFixedAsset242(ledger, (AssetsType)Enum.Parse(typeof(AssetsType), ledger.Type.ToString()), year);
        }
    }

    private async Task UpdateChartOfAccountName(FixedAsset cEntity, int year)
    {
        var coaDebit = await _chartOfAccountService.GetAccountByCode(cEntity.DebitCode, year);
        var coaDebitDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.DebitDetailCodeFirst, year);
        var coaDebitDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.DebitDetailCodeSecond, year);

        var coaCredit = await _chartOfAccountService.GetAccountByCode(cEntity.CreditCode, year);
        var coaCreditDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeFirst, year);
        var coaCreditDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeSecond, year);

        cEntity.DebitCodeName = cEntity.DebitCodeName ?? coaDebit?.Name;
        cEntity.DebitDetailCodeFirstName = cEntity.DebitDetailCodeFirstName ?? coaDebitDetail1?.Name;
        cEntity.DebitDetailCodeSecondName = cEntity.DebitDetailCodeSecondName ?? coaDebitDetail2?.Name;
        cEntity.CreditCodeName = cEntity.CreditCodeName ?? coaCredit?.Name;
        cEntity.CreditDetailCodeFirstName = cEntity.CreditDetailCodeFirstName ?? coaCreditDetail1?.Name;
        cEntity.CreditDetailCodeSecondName = cEntity.CreditDetailCodeSecondName ?? coaCreditDetail2?.Name;
        if (coaCredit.Type == 4)
            cEntity.Type = "PB";
        else if (coaCredit.Type == 5)
            cEntity.Type = "KH";
        cEntity.DebitCode = cEntity.DebitCode ?? string.Empty;
        cEntity.DebitDetailCodeFirst = cEntity.DebitDetailCodeFirst ?? string.Empty;
        cEntity.DebitDetailCodeSecond = cEntity.DebitDetailCodeSecond ?? string.Empty;
        cEntity.CreditCode = cEntity.CreditCode ?? string.Empty;
        cEntity.CreditDetailCodeFirst = cEntity.CreditDetailCodeFirst ?? string.Empty;
        cEntity.CreditDetailCodeSecond = cEntity.CreditDetailCodeSecond ?? string.Empty;

        cEntity.DebitCodeName = cEntity.DebitCodeName ?? string.Empty;
        cEntity.DebitDetailCodeFirstName = cEntity.DebitDetailCodeFirstName ?? string.Empty;
        cEntity.DebitDetailCodeSecondName = cEntity.DebitDetailCodeSecondName ?? string.Empty;
        cEntity.CreditCodeName = cEntity.CreditCodeName ?? string.Empty;
        cEntity.CreditDetailCodeFirstName = cEntity.CreditDetailCodeFirstName ?? string.Empty;
        cEntity.CreditDetailCodeSecondName = cEntity.CreditDetailCodeSecondName ?? string.Empty;

        cEntity.DebitWarehouse = coaDebitDetail2?.WarehouseCode ?? coaDebitDetail1?.WarehouseCode ?? coaDebit?.WarehouseCode ?? string.Empty;
        cEntity.CreditWarehouse = coaCreditDetail2?.WarehouseCode ?? coaCreditDetail1?.WarehouseCode ?? coaCredit?.WarehouseCode ?? string.Empty;
    }

    public string Delete(IEnumerable<int> ids)
    {
        string message = "";

        if (!_context.FixedAssets.Any(t => ids.Contains(t.Id)))
        {
            return ErrorMessages.DataNotFound;
        }

        try
        {
            _context.Database.BeginTransaction();

            foreach (var id in ids)
            {
                var entity = _context.FixedAssets.Find(id);
                _context.FixedAssets.Remove(entity);
            }

            _context.SaveChanges();
            _context.Database.CommitTransaction();
        }
        catch
        {
            message = ErrorMessages.NotDeletedAll;
            _context.Database.RollbackTransaction();
        }

        return message;
    }

    public async Task<PagingResult<FixedAssetViewModel>> SearchFixedAsset(PagingRequestModel searchRequest)
    {
        try
        {
            var fixedAssets = GetFixedAssetPaging(searchRequest);
            var datas = fixedAssets.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize).ToList();
            foreach (var item in datas)
            {
                if (item.DepartmentId != null)
                {
                    var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == item.DepartmentId);
                    item.DepartmentManagerName = department?.Name;
                }
                if (item.UserId != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == item.UserId);
                    item.UserManagerName = user?.FullName;
                }
            }
            return new PagingResult<FixedAssetViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = fixedAssets.Count(),
                Data = datas
            };
        }
        catch
        {
            return new PagingResult<FixedAssetViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = 0,
                Data = new List<FixedAssetViewModel>()
            };
        }
    }

    private IQueryable<FixedAssetViewModel> GetFixedAssetPaging(PagingRequestModel searchRequest)
    {
        return (from f in _context.FixedAssets
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
                    HistoricalCost = f.HistoricalCost,
                    Use = f.Use,
                    Quantity = f.Quantity,
                    UnitPrice = f.UnitPrice
                });
    }

    public string ExportExcel(PagingRequestModel searchRequest)
    {
        var datas = GetFixedAssetPaging(searchRequest).ToList();

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
                int nCol = 13;
                int rowIdx = nRowBegin;
                if (datas.Count > 0)
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
                        sheet.Cells[rowIdx, 10].Value = item.UnitPrice;
                        sheet.Cells[rowIdx, 11].Value = item.HistoricalCost;
                        sheet.Cells[rowIdx, 12].Value = item.TotalMonth;
                        sheet.Cells[rowIdx, 13].Value = item.Use == 1 ? "Có" : "Không";
                        rowIdx++;
                    }
                    var loaiUse = sheet.Cells[nRowBegin, 13, rowIdx, 13].DataValidation.AddListDataValidation();
                    loaiUse.Formula.Values.Add("Có");
                    loaiUse.Formula.Values.Add("Không");
                }

                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 9, rowIdx, 11].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

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
            FixedAsset fixAsset = _mapper.Map<FixedAsset>(data);
            fixAsset.Type = AssetsType.PB.ToString();
            fixAsset.BuyDate = fixAsset.UsedDate;
            fixAsset.Name = (!string.IsNullOrEmpty(fixAsset.CreditDetailCodeSecondName) ? fixAsset.CreditDetailCodeSecondName : (!string.IsNullOrEmpty(fixAsset.CreditDetailCodeFirstName) ? fixAsset.CreditDetailCodeFirstName : fixAsset.CreditCodeName)); ;
            _context.FixedAssets.Add(fixAsset);
        }

        _context.SaveChanges();
        return string.Empty;
    }

    public async Task<CustomActionResult<List<FixedAssetsModelEdit>>> UpdateEditAccount(List<FixedAssetsModelEdit> entities, bool IsAutoAddDetail, int year)
    {
        var result = new CustomActionResult<List<FixedAssetsModelEdit>>
        {
            IsSuccess = true,
            SuccessData = new List<FixedAssetsModelEdit>()
        };
        if (entities.Count < 1)
        {
            throw new Exception("Không có tài khoản dược chọn");
        }

        ChartOfAccount account = await _context.GetChartOfAccount(year).AsNoTracking().FirstOrDefaultAsync(x => x.Code == entities[0].DebitCode);

        foreach (var entity in entities)
        {
            var item = await _context.FixedAssets.FindAsync(entity.Id);
            if (item != null)
            {
                item.DebitCode = entity.DebitCode;
                item.CreditCode = entity.CreditCode;
                item.CreditDetailCodeFirst = entity.CreditDetailCodeFirst;
                item.CreditDetailCodeSecond = entity.CreditDetailCodeSecond;

                if (IsAutoAddDetail)
                {
                    await UpdateNameFixedAsset(item, year);

                    item.DebitDetailCodeFirst = entity.CreditDetailCodeFirst;
                    item.DebitDetailCodeFirstName = item.CreditDetailCodeFirstName;
                    item.DebitDetailCodeSecond = entity.CreditDetailCodeSecond;
                    item.DebitDetailCodeSecondName = item.CreditDetailCodeSecondName;
                    var detail1 = new ChartOfAccount();
                    if (!string.IsNullOrEmpty(entity.CreditDetailCodeFirst))
                    {
                        detail1 = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.CreditDetailCodeFirst && x.ParentRef == entity.DebitCode);
                        if (detail1 is null)
                        {
                            detail1 = new ChartOfAccount();
                            detail1.Id = 0;
                            detail1.Code = entity.CreditDetailCodeFirst;
                            detail1.Name = item.CreditDetailCodeFirstName;
                            detail1.ParentRef = entity.DebitCode;
                            detail1.Type = 5;
                            detail1.Duration = account.Duration;
                            detail1.AccGroup = account.AccGroup;
                            detail1.Classification = account.Classification;
                            detail1.StockUnit = account.StockUnit;
                            detail1.Year = year;

                            _context.ChartOfAccounts.Add(detail1);
                            if (!account.HasDetails)
                            {
                                account.HasDetails = true;
                                _context.ChartOfAccounts.Update(account);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(entity.CreditDetailCodeSecond))
                    {
                        string parentRef = $"{entity.DebitCode}:{entity.CreditDetailCodeFirst}";
                        var detail = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.CreditDetailCodeSecond && x.ParentRef == parentRef);
                        if (detail is null)
                        {
                            detail = new ChartOfAccount();
                            detail.Id = 0;
                            detail.Code = entity.CreditDetailCodeFirst;
                            detail.Name = item.CreditDetailCodeFirstName;
                            detail.ParentRef = entity.DebitCode;
                            detail.Type = 5;
                            detail.Duration = account.Duration;
                            detail.AccGroup = account.AccGroup;
                            detail.Classification = account.Classification;
                            detail.StockUnit = account.StockUnit;
                            detail.Year = year;

                            _context.ChartOfAccounts.Add(detail);
                            if (!detail1.HasDetails)
                            {
                                detail1.HasDetails = true;
                                _context.ChartOfAccounts.Update(detail1);
                            }
                        }
                    }
                }
                else
                {
                    item.DebitDetailCodeFirst = entity.DebitDetailCodeFirst;
                    item.DebitDetailCodeSecond = entity.DebitDetailCodeSecond;
                    await UpdateChartOfAccountName(item, year);
                }
                _context.FixedAssets.Update(item);
            }
        }
        await _context.SaveChangesAsync();
        result.SuccessData = entities;
        return result;
    }

    private async Task UpdateChartOfAccountName242(FixedAsset242 cEntity, int year)
    {
        var coaDebit = await _chartOfAccountService.GetAccountByCode(cEntity.DebitCode, year);
        var coaDebitDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.DebitDetailCodeFirst, year);
        var coaDebitDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.DebitDetailCodeSecond, year);

        var coaCredit = await _chartOfAccountService.GetAccountByCode(cEntity.CreditCode, year);
        var coaCreditDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeFirst, year);
        var coaCreditDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeSecond, year);

        cEntity.DebitDetailCodeFirstName = cEntity.DebitDetailCodeFirstName ?? coaDebitDetail1?.Name;
        cEntity.DebitDetailCodeSecondName = cEntity.DebitDetailCodeSecondName ?? coaDebitDetail2?.Name;
        cEntity.CreditCodeName = cEntity.CreditCodeName ?? coaCredit?.Name;
        cEntity.CreditDetailCodeFirstName = cEntity.CreditDetailCodeFirstName ?? coaCreditDetail1?.Name;
        cEntity.CreditDetailCodeSecondName = cEntity.CreditDetailCodeSecondName ?? coaCreditDetail2?.Name;
        cEntity.DebitCode = cEntity.DebitCode ?? string.Empty;
        cEntity.DebitDetailCodeFirst = cEntity.DebitDetailCodeFirst ?? string.Empty;
        cEntity.DebitDetailCodeSecond = cEntity.DebitDetailCodeSecond ?? string.Empty;
        cEntity.CreditCode = cEntity.CreditCode ?? string.Empty;
        cEntity.CreditDetailCodeFirst = cEntity.CreditDetailCodeFirst ?? string.Empty;
        cEntity.CreditDetailCodeSecond = cEntity.CreditDetailCodeSecond ?? string.Empty;
        cEntity.DebitCodeName = cEntity.DebitCodeName ?? string.Empty;

        cEntity.Name = !string.IsNullOrEmpty(cEntity.CreditDetailCodeSecondName) ? cEntity.CreditDetailCodeSecondName :
                        (!string.IsNullOrEmpty(cEntity.CreditDetailCodeFirstName) ? cEntity.CreditDetailCodeFirstName : cEntity.CreditCodeName);

        cEntity.DebitWarehouse = coaDebitDetail2?.WarehouseCode ?? coaDebitDetail1?.WarehouseCode ?? coaDebit?.WarehouseCode ?? string.Empty;
        cEntity.CreditWarehouse = coaCreditDetail2?.WarehouseCode ?? coaCreditDetail1?.WarehouseCode ?? coaCredit?.WarehouseCode ?? string.Empty;
    }

    private async Task UpdateNameFixedAsset(FixedAsset cEntity, int year)
    {
        var coaDebit = await _chartOfAccountService.GetAccountByCode(cEntity.DebitCode, year);

        var coaCredit = await _chartOfAccountService.GetAccountByCode(cEntity.CreditCode, year);
        var coaCreditDetail1 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeFirst, year);
        var coaCreditDetail2 = await _chartOfAccountService.GetAccountByCode(cEntity.CreditDetailCodeSecond, year);

        cEntity.DebitCodeName = cEntity.DebitCodeName ?? coaDebit?.Name;
        cEntity.CreditCodeName = cEntity.CreditCodeName ?? coaCredit?.Name;
        cEntity.CreditDetailCodeFirstName = cEntity.CreditDetailCodeFirstName ?? coaCreditDetail1?.Name;
        cEntity.CreditDetailCodeSecondName = cEntity.CreditDetailCodeSecondName ?? coaCreditDetail2?.Name;

        cEntity.DebitCode = cEntity.DebitCode ?? string.Empty;
        cEntity.CreditCode = cEntity.CreditCode ?? string.Empty;
        cEntity.CreditDetailCodeFirst = cEntity.CreditDetailCodeFirst ?? string.Empty;
        cEntity.CreditDetailCodeSecond = cEntity.CreditDetailCodeSecond ?? string.Empty;

        cEntity.Name = !string.IsNullOrEmpty(cEntity.CreditDetailCodeSecondName) ? cEntity.CreditDetailCodeSecondName :
                        (!string.IsNullOrEmpty(cEntity.CreditDetailCodeFirstName) ? cEntity.CreditDetailCodeFirstName : cEntity.CreditCodeName);

        cEntity.CreditWarehouse = coaCreditDetail2?.WarehouseCode ?? coaCreditDetail1?.WarehouseCode ?? coaCredit?.WarehouseCode ?? string.Empty;
    }
}