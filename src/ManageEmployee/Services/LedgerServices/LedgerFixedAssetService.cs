using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.LedgerServices;
public class LedgerFixedAssetService : ILedgerFixedAssetService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChartOfAccountService _chartOfAccountService;


    public LedgerFixedAssetService(ApplicationDbContext context, IMapper mapper, IChartOfAccountService chartOfAccountService)
    {
        _context = context;
        _mapper = mapper;
        _chartOfAccountService = chartOfAccountService;
    }
    public async Task CreateAsync(LedgerFixedAsset form)
    {
        _context.LedgerFixedAssets.Add(form);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAsync(Ledger ledger, int year)
    {
        var ledgerFixedAssets = await _context.LedgerFixedAssets.Where(x => x.LedgerId == ledger.Id).ToListAsync();
        if (ledgerFixedAssets.Count > 0)
        {
            if (ledgerFixedAssets.Any(x => x.FixedAssetId > 0))
            {
                var ledgerFixedAsset = ledgerFixedAssets.FirstOrDefault(x => x.FixedAssetId > 0);
                if (ledgerFixedAsset is null)
                {
                    return;
                }
                var fixedAsset = await _context.FixedAssets.FindAsync(ledgerFixedAsset.FixedAssetId);
                if (fixedAsset is null)
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


                    FixedAssetsModel assetModel = new FixedAssetsModel().FromLedger(ledger);
                    assetModel.Name = !string.IsNullOrEmpty(assetModel.CreditDetailCodeSecondName) ? assetModel.CreditDetailCodeSecondName : !string.IsNullOrEmpty(assetModel.CreditDetailCodeFirstName) ? assetModel.CreditDetailCodeFirstName : assetModel.CreditCodeName;
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
                    ledgerFixedAsset.FixedAssetId = asset.Id;
                    _context.LedgerFixedAssets.Update(ledgerFixedAsset);

                }
            }
            else if (ledgerFixedAssets.Any(x => x.FixedAsset242Id > 0))
            {
                var ledgerFixedAsset = ledgerFixedAssets.FirstOrDefault(x => x.FixedAsset242Id > 0);

                var fixedAsset = await _context.FixedAsset242.FindAsync(ledgerFixedAsset?.FixedAsset242Id);
                if (fixedAsset is null)
                {
                    string strDebitCode = ledger.DebitCode ?? string.Empty;
                    string strDebitDetailCode = ledger.DebitDetailCodeFirst;
                    string strDebitParentCode = strDebitCode;
                    if (!string.IsNullOrEmpty(ledger.DebitDetailCodeSecond))
                    {
                        strDebitDetailCode = ledger.DebitDetailCodeSecond;
                        strDebitParentCode = strDebitCode + ":" + ledger.DebitDetailCodeFirst;
                    }
                    ChartOfAccount acctDebitDetail = await _chartOfAccountService.GetAccountByCode(strDebitDetailCode, year, strDebitParentCode);

                    FixedAssetsModel assetModel = new FixedAssetsModel().FromLedger(ledger);
                    assetModel.Name = !string.IsNullOrEmpty(assetModel.CreditDetailCodeSecondName) ? assetModel.CreditDetailCodeSecondName : !string.IsNullOrEmpty(assetModel.CreditDetailCodeFirstName) ? assetModel.CreditDetailCodeFirstName : assetModel.CreditCodeName;

                    assetModel.CreditCode = ledger?.DebitCode;
                    assetModel.CreditCodeName = ledger?.DebitCodeName;

                    assetModel.CreditDetailCodeFirst = ledger?.DebitDetailCodeFirst;
                    assetModel.CreditDetailCodeFirstName = ledger?.DebitDetailCodeFirstName;

                    assetModel.CreditDetailCodeSecond = ledger?.DebitDetailCodeSecond;
                    assetModel.CreditDetailCodeSecondName = ledger?.DebitDetailCodeSecondName;


                    assetModel.Use = 1;
                    assetModel.CarryingAmount = assetModel.HistoricalCost - (assetModel.DepreciationOfThisPeriod ?? 0);

                    assetModel.Type = Enum.Parse(typeof(AssetsType), acctDebitDetail.Classification.ToString()).ToString();
                    FixedAsset242 asset = _mapper.Map<FixedAsset242>(assetModel);

                    _context.FixedAsset242.Add(asset);
                    await _context.SaveChangesAsync();


                    ledgerFixedAsset.FixedAsset242Id = asset.Id;
                    _context.LedgerFixedAssets.Update(ledgerFixedAsset);

                }
            }
        }
        await _context.SaveChangesAsync();
    }
}
