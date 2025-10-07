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


public class FixedAssets242Service : IFixedAssets242Service
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly IMapper _mapper;
    private readonly ILedgerService _ledgerServices;
    private readonly ILedgerFixedAssetService _ledgerFixedAssetService;

    public FixedAssets242Service(ApplicationDbContext context, IChartOfAccountService chartOfAccountService, IMapper mapper, ILedgerService ledgerServices, ILedgerFixedAssetService ledgerFixedAssetService)
    {
        _context = context;
        _chartOfAccountService = chartOfAccountService;
        _mapper = mapper;
        _ledgerServices = ledgerServices;
        _ledgerFixedAssetService = ledgerFixedAssetService;
    }
    public async Task<FixedAsset242> UpdateFromLedger(Ledger ledger, int year)
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
        if ((acctDebitDetail.Classification == (int)AssetsType.CCDCSD || acctDebitDetail.Classification == (int)AssetsType.KH) && ledger.DepreciaMonth > 0)
        {

            FixedAssetsModel assetModel = new FixedAssetsModel().FromLedger(ledger);
            assetModel.Name = (!string.IsNullOrEmpty(assetModel.CreditDetailCodeSecondName) ? assetModel.CreditDetailCodeSecondName : (!string.IsNullOrEmpty(assetModel.CreditDetailCodeFirstName) ? assetModel.CreditDetailCodeFirstName : assetModel.CreditCodeName));

            assetModel.CreditCode = ledger.DebitCode;
            assetModel.CreditCodeName = ledger.DebitCodeName;

            assetModel.CreditDetailCodeFirst = ledger?.DebitDetailCodeFirst;
            assetModel.CreditDetailCodeFirstName = ledger.DebitDetailCodeFirstName;

            assetModel.CreditDetailCodeSecond = ledger.DebitDetailCodeSecond;
            assetModel.CreditDetailCodeSecondName = ledger.DebitDetailCodeSecondName;


            assetModel.Use = 1;
            assetModel.CarryingAmount = assetModel.HistoricalCost - (assetModel.DepreciationOfThisPeriod ?? 0);

            assetModel.Type = Enum.Parse(typeof(AssetsType), acctDebitDetail.Classification.ToString()).ToString();
            FixedAsset242 asset = _mapper.Map<FixedAsset242>(assetModel);

            _context.FixedAsset242.Add(asset);
            await _context.SaveChangesAsync();


            // add LedgerFixedAsset
            LedgerFixedAsset ledgerFixedAsset = new()
            {
                LedgerId = ledger.Id,
                FixedAsset242Id = asset.Id,
                IsInternal = ledger.IsInternal
            };
            await _ledgerFixedAssetService.CreateAsync(ledgerFixedAsset);

            string userCodeString = _context.FixedAssetUser.OrderByDescending(x => x.UsedCode).FirstOrDefault()?.UsedCode ?? "0";
            int userCode = int.Parse(userCodeString);
            for (int i = 0; i < asset.Quantity; i++)
            {
                userCode++;
                var assetUser = _mapper.Map<FixedAssetUser>(asset);
                assetUser.Id = 0;
                assetUser.Quantity = 1;
                assetUser.UsedCode = userCode.ToString();
                _context.FixedAssetUser.Add(assetUser);
            }

            await _context.SaveChangesAsync();

            return GetById(asset.Id).Result;
        }
        return null;
    }

    public async Task<List<FixedAssetsModelEdit>> GetListEdit(AssetsType eType, int iMonth, int iYear, int isInternal)
    {
        DateTime dtPeriodDate = new DateTime(iYear, iMonth, 1);
        var listType = new List<string>();
        if (eType == AssetsType.PB)
        {
            listType.Add(AssetsType.PB.ToString());
            listType.Add(AssetsType.CCDCSD.ToString());
        }
        else
        {
            listType.Add(eType.ToString());
        }
        var lst = await _context.FixedAsset242.Where(t => t.Use == 1)
            .GroupJoin(_context.LedgerFixedAssets,
                                    f => f.Id,
                                    l => l.FixedAsset242Id,
                                    (f, l) => new { fixedAsset = f, ledger = l })
                        .SelectMany(x => x.ledger.DefaultIfEmpty(), (f, l) => new { fixedAsset = f.fixedAsset, isInternal = l.IsInternal })
                        .Where(x => (isInternal == 3 ? x.isInternal == 3 : x.isInternal == 2) || x.isInternal == 1)
                               .Where(t => listType.Contains(t.fixedAsset.Type)
                               && (t.fixedAsset.TotalMonth ?? 0) > 0
                                           && (t.fixedAsset.UsedDate != null && t.fixedAsset.UsedDate.Value < dtPeriodDate.AddMonths(1))
                                           && ((t.fixedAsset.EndOfDepreciation != null && t.fixedAsset.EndOfDepreciation.Value > dtPeriodDate) || (t.fixedAsset.EndOfDepreciation == null))
                                )
                               .Select(x => _mapper.Map<FixedAssetsModelEdit>(x.fixedAsset))
                               .Distinct()
                               .ToListAsync();

        lst.ForEach(t => t.Calculate(dtPeriodDate));

        lst = lst.Where(t => t.TotalMonthLeft >= 0 && t.DepreciationOfThisPeriod > 0).ToList();

        return lst;
    }

    public async Task<FixedAsset242> GetById(int id)
    {
        return await _context.FixedAsset242.FindAsync(id);
    }

    public async Task<string> Update(FixedAssetsModelEdit entity, int year)
    {
        FixedAsset242 asset = new FixedAsset242();
        if (entity.Id > 0)
        {
            asset = _context.FixedAsset242.Find(entity.Id);
        }
        if (asset != null)
        {
            asset.CreditCode = entity.CreditCode;
            asset.CreditDetailCodeFirst = entity.CreditDetailCodeFirst;
            asset.CreditDetailCodeSecond = entity.CreditDetailCodeSecond;
            asset.HistoricalCost = entity.HistoricalCost;
            asset.Quantity = entity.Quantity;
            asset.TotalMonth = entity.TotalMonth;
            await UpdateChartOfAccountName(asset, year);

            asset.Name = !string.IsNullOrEmpty(asset.CreditDetailCodeSecondName) ? asset.CreditDetailCodeSecondName :
                    (!string.IsNullOrEmpty(asset.CreditDetailCodeFirstName) ? asset.CreditDetailCodeFirstName : asset.CreditCodeName);
            asset.Use = entity.Use;
            asset.UnitPrice = entity.UnitPrice;
            asset.UsedDate = entity.UsedDate;
            if (entity.Id > 0)
                _context.FixedAsset242.Update(asset);
            else
            {
                _context.FixedAsset242.Add(asset);
            }
        }
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<CustomActionResult<List<FixedAssetsModelEdit>>> UpdateEdit(List<FixedAssetsModelEdit> entities, int isInternal, int year)
    {

        var result = new CustomActionResult<List<FixedAssetsModelEdit>>
        {
            IsSuccess = true,
            SuccessData = new List<FixedAssetsModelEdit>()
        };

        var lstDebitCodeUpdate = new List<string>();
        var lstDebitCodeAdd = new List<string>();

        foreach (var entity in entities)
        {

            //Kiểm tra chưa phân bổ CCDC / Khấu hao TSCĐ tháng trước
            if (entity.TotalMonthLeft < entity.TotalMonth)//totalMonth: khau hao ; TotalMonthLeft: con lai
            {
                FixedAssetsModelEdit item = new FixedAssetsModelEdit
                {
                    Id = entity.Id,
                    Type = entity.Type,
                    PeriodDate = entity.PeriodDate,
                    DebitCode = entity.DebitCode,
                    DebitDetailCodeFirst = entity.DebitDetailCodeFirst,
                    DebitDetailCodeSecond = entity.DebitDetailCodeSecond,
                    CreditCode = entity.CreditCode,
                    CreditDetailCodeFirst = entity.CreditDetailCodeFirst,
                    CreditDetailCodeSecond = entity.CreditDetailCodeSecond
                };
                if (item.PeriodDate.Month == 1 && entity.UsedDate.Value.Year < item.PeriodDate.Year)
                    continue;
                item.PeriodDate = item.PeriodDate.AddMonths(-1);
                var findFixAsset = await _ledgerServices.FindByFixedAssets(item, isInternal, year);
                if (!findFixAsset.IsSuccess)
                {
                    result.IsSuccess = false;
                    int month = await _ledgerServices.FindByFixedAssetCheckMonths(item, year);
                    if (month == 0)
                        result.ErrorMessage = string.Format("Không tìm thấy phát sinh", month);
                    else
                        result.ErrorMessage = string.Format("Bạn cần chọn tháng {0} để phân bổ/khấu hao nha!", month);

                    return result;
                }

            }
        }

        if (result.IsSuccess)
        {
            foreach (var entity in entities)
            {
                var rs = await UpdateEditFromFixedAssets(entity, year);
                if (rs != null)
                {
                    var findFixAsset = await _ledgerServices.FindByFixedAssets(entity, isInternal, year);

                    if (findFixAsset.IsSuccess)
                    {
                        lstDebitCodeUpdate.Add(entity.DebitCode);
                    }
                    else
                    {
                        lstDebitCodeAdd.Add(entity.DebitCode);
                    }
                    // Tạo bút toán Phân bổ CCDC / Khấu hao TSCĐ
                    await _ledgerServices.CreateFromFixedAsset(entity, (AssetsType)Enum.Parse(typeof(AssetsType), entity.Type.ToString()), isInternal, year);
                    result.SuccessData.Add(rs);
                }
            }
        }
        else
        {
            result.IsSuccess = false;
            result.ErrorMessage = "Phân bổ/khấu hao không thành công!";
        }

        if (result.SuccessData.Count == entities.Count)
        {
            if (lstDebitCodeUpdate.Any())
            {
                if (!lstDebitCodeAdd.Any())
                {
                    result.Message = string.Format("Cập nhật phân bổ/khấu hao cho TK <b><span>{0}</span></b> thành công!", string.Join(", ", lstDebitCodeUpdate));
                }
                else
                {
                    result.Message = string.Format("Cập nhật phân bổ/khấu hao cho TK <b><span>{0}</span></b>. Và phát sinh cho TK mới thành công!", string.Join(", ", lstDebitCodeUpdate));
                }
            }
            else
            {
                result.Message = "Phân bổ/khấu hao thành công!";
            }
        }

        return result;
    }


    public string Deletes(IEnumerable<int> ids)
    {
        string message = "";

        if (!_context.FixedAsset242.Any(t => ids.Contains(t.Id)))
        {
            return ErrorMessages.DataNotFound;
        }

        try
        {
            _context.Database.BeginTransaction();

            foreach (var id in ids)
            {
                var entity = _context.FixedAsset242.Find(id);
                if (entity != null)
                    _context.FixedAsset242.Remove(entity);
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
    public string Delete(int id)
    {
        string message = "";
        var asset = _context.FixedAsset242.Find(id);
        if (asset == null)
        {
            return ErrorMessages.DataNotFound;
        }

        try
        {
            _context.FixedAsset242.Remove(asset);

            _context.SaveChanges();
        }
        catch
        {
            message = ErrorMessages.NotDeletedAll;
        }
        return message;
    }

    public PagingResult<FixedAssetExport> SearchFixedAsset(FixedAsset242RequestModel searchRequest)
    {
        try
        {
            var fixedAssets = GetFixedAssetPaging(searchRequest);
            var datas = fixedAssets.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize).ToList();
            return new PagingResult<FixedAssetExport>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = fixedAssets.Count(),
                Data = datas
            };
        }
        catch
        {
            return new PagingResult<FixedAssetExport>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = 0,
                Data = new List<FixedAssetExport>()
            };
        }
    }
    private IQueryable<FixedAssetExport> GetFixedAssetPaging(FixedAsset242RequestModel searchRequest)
    {
        return (from f in _context.FixedAsset242
                join _d in _context.Departments on f.DepartmentId equals _d.Id into _d
                from ds in _d.DefaultIfEmpty()
                join _u in _context.Users on f.UserId equals _u.Id into _u
                from us in _u.DefaultIfEmpty()
                where (string.IsNullOrEmpty(searchRequest.SearchText)
                        || (!string.IsNullOrEmpty(f.Name) && f.Name.ToLower().Contains(searchRequest.SearchText.ToLower())))
                        && f.Use == searchRequest.Use
                         && ((searchRequest.Type == AssetsType.CCDCSD.ToString() || searchRequest.Type == AssetsType.PB.ToString()) ? (f.Type == AssetsType.CCDCSD.ToString() || f.Type == AssetsType.PB.ToString()) : f.Type == searchRequest.Type)
                select new FixedAssetExport
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
                    CarryingAmount = f.CarryingAmount,
                    TotalMonth = f.TotalMonth,
                    Use = f.Use,
                    Quantity = f.Quantity,
                    HistoricalCost = f.HistoricalCost,
                    Name = f.Name,
                    Type = f.Type,
                    UnitPrice = f.UnitPrice,
                });
    }
    public string ExportExcel(FixedAsset242RequestModel searchRequest)
    {
        var datas = GetFixedAssetPaging(searchRequest).ToList();

        string _fileMapServer = $"CongCuDungCu_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                   folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                   _pathSave = Path.Combine(folder, _fileMapServer);

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\FixAsset242.xlsx");
        MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 6;
                int nCol = 14;
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
                        sheet.Cells[rowIdx, 14].Value = item.Type;
                        rowIdx++;
                    }
                    var loaiUse = sheet.Cells[nRowBegin, 13, rowIdx, 13].DataValidation.AddListDataValidation();
                    loaiUse.Formula.Values.Add("Có");
                    loaiUse.Formula.Values.Add("Không");

                    var types = sheet.Cells[nRowBegin, 14, rowIdx, 14].DataValidation.AddListDataValidation();
                    types.Formula.Values.Add(AssetsType.PB.ToString());
                    types.Formula.Values.Add(AssetsType.KH.ToString());
                    types.Formula.Values.Add(AssetsType.XH.ToString());
                    types.Formula.Values.Add(AssetsType.KC.ToString());
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
            FixedAsset242 fixAsset = _mapper.Map<FixedAsset242>(data);
            fixAsset.BuyDate = fixAsset.UsedDate;
            fixAsset.Name = (!string.IsNullOrEmpty(fixAsset.CreditDetailCodeSecondName) ? fixAsset.CreditDetailCodeSecondName : (!string.IsNullOrEmpty(fixAsset.CreditDetailCodeFirstName) ? fixAsset.CreditDetailCodeFirstName : fixAsset.CreditCodeName)); ;
            fixAsset.CarryingAmount = fixAsset.HistoricalCost;


            _context.FixedAsset242.Add(fixAsset);
        }

        _context.SaveChanges();
        return string.Empty;
    }


    public async Task<CustomActionResult<List<FixedAssetsModelEdit>>> UpdateEditAccount(List<FixedAssetsModelEdit> entities)
    {

        var result = new CustomActionResult<List<FixedAssetsModelEdit>>
        {
            IsSuccess = true,
            SuccessData = new List<FixedAssetsModelEdit>()
        };
        var fixAsset242Ids = entities.Select(x => x.Id).ToList();
        var fixAsset242s = await _context.FixedAsset242.Where(x => fixAsset242Ids.Contains(x.Id)).ToListAsync();

        foreach (var fixAsset242 in fixAsset242s)
        {
            var entity = entities.Find(x => x.Id == fixAsset242.Id);
            fixAsset242.DebitCode = entity.DebitCode;
            fixAsset242.DebitDetailCodeFirst = entity.DebitDetailCodeFirst;
            fixAsset242.DebitDetailCodeSecond = entity.DebitDetailCodeSecond;
            fixAsset242.CreditCode = entity.CreditCode;
            fixAsset242.CreditDetailCodeFirst = entity.CreditDetailCodeFirst;
            fixAsset242.CreditDetailCodeSecond = entity.CreditDetailCodeSecond;
        }

        _context.FixedAsset242.UpdateRange(fixAsset242s);

        await _context.SaveChangesAsync();
        result.SuccessData = entities;
        return result;
    }

    private async Task<FixedAssetsModelEdit> UpdateEditFromFixedAssets(FixedAssetsModelEdit entity, int year)
    {
        FixedAsset242 asset = await _context.FixedAsset242.FindAsync(entity.Id);
        if (asset == null)
            asset = new FixedAsset242();
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

        asset.UsedDate = entity.UsedDate;
        asset.Quantity = entity.Quantity;

        await UpdateChartOfAccountName(asset, year);

        asset.Name = !string.IsNullOrEmpty(asset.CreditDetailCodeSecondName) ? asset.CreditDetailCodeSecondName :
                (!string.IsNullOrEmpty(asset.CreditDetailCodeFirstName) ? asset.CreditDetailCodeFirstName : asset.CreditCodeName);

        if (asset.CarryingAmount < asset.DepreciationOfThisPeriod && asset.EndOfDepreciation == null)
        {
            if (asset.CarryingAmount == 0)
                asset.EndOfDepreciation = entity.PeriodDate.AddMonths(1).AddDays(-1);
            else
                asset.EndOfDepreciation = entity.PeriodDate.AddMonths(2).AddDays(-1);
        }
        if (entity.TotalMonthLeft < 1)
        {
            asset.Use = 0;
        }
        if (entity.Id > 0)
            _context.FixedAsset242.Update(asset);
        else
        {
            _context.FixedAsset242.Add(asset);
            for (int i = 0; i < asset.Quantity; i++)
            {
                var assetUser = _mapper.Map<FixedAssetUser>(asset);
                assetUser.Quantity = 1;
                _context.FixedAssetUser.Add(assetUser);
            }
        }
        _context.SaveChanges();
        return entity;
    }

    private async Task UpdateChartOfAccountName(FixedAsset242 cEntity, int year)
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

}
