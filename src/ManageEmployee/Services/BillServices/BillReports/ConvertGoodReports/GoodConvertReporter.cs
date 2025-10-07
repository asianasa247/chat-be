using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.ConvertGoodReports
{
    public class GoodConvertReporter: IGoodConvertReporter
    {
        private readonly ILedgerService _ledgerServices;
        private readonly ApplicationDbContext _context;

        public GoodConvertReporter(ILedgerService ledgerServices, ApplicationDbContext context)
        {
            _ledgerServices = ledgerServices;
            _context = context;
        }
        // get good before convert
        public async Task<IEnumerable<GoodConvertBeforeReporterModel>> ReporterAsync(BillReportGoodRequestModel param, int year)
        {
            LedgerReportParamDetail form = new LedgerReportParamDetail()
            {
                AccountCode = param.Account,
                FromDate = param.FromAt,
                ToDate = param.ToAt,
                IsNoiBo = true,
                FilterType = 2,
                AccountCodeDetail1 = param.Detail1,
                AccountCodeDetail2 = param.Detail2,
            };
            var datas = await _ledgerServices.GetDataReport_SoChiTiet_Six_data(form, year, wareHouseCode: null);
            var listOut = new List<GoodConvertBeforeReporterModel>();

            foreach (var item in datas)
            {
                var goodConvert = await _context.ConvertProducts
                                    .FirstOrDefaultAsync(x => x.OppositeAccount == item.Account
                                                                && x.OppositeDetail1 == item.Detail1
                                                                && (string.IsNullOrEmpty(x.OppositeDetail2) || x.OppositeDetail2 == item.Detail2)
                                                                && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == item.Warehouse));

                
                item.CloseQuantity = item.OpenQuantity + item.InputQuantity - item.OutputQuantity;
                if (goodConvert is null)
                {
                    listOut.Add(new GoodConvertBeforeReporterModel
                    {
                        Account = item.Account,
                        Detail1 = item.Detail1,
                        Detail2 = item.Detail2,
                        Warehouse = item.Warehouse,
                        CloseQuantity = item.CloseQuantity,
                        OpenQuantity = item.OpenQuantity,
                        InputQuantity = item.InputQuantity,
                        OutputQuantity = item.OutputQuantity,
                        GoodCode = string.IsNullOrEmpty(item.Detail2) ? item.Detail1 : item.Detail2,
                        GoodName = item.NameGood,
                    });
                }
                else
                {
                    var itemOut = listOut.FirstOrDefault(x => x.Account == goodConvert.Account
                                                                && x.Detail1 == goodConvert.Detail1
                                                                && (string.IsNullOrEmpty(goodConvert.Detail2) || x.Detail2 == goodConvert.Detail2)
                                                                && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == goodConvert.Warehouse));
                    if (itemOut is null)
                    {
                        itemOut = new GoodConvertBeforeReporterModel
                        {
                            Account = goodConvert.Account,
                            Detail1 = goodConvert.Detail1,
                            Detail2 = goodConvert.Detail2,
                            Warehouse = goodConvert.Warehouse,
                            GoodCode = string.IsNullOrEmpty(goodConvert.Detail2) ? goodConvert.Detail1 : goodConvert.Detail2,
                            GoodName = string.IsNullOrEmpty(goodConvert.DetailName2) ? goodConvert.DetailName1 : goodConvert.DetailName2,
                        };
                        listOut.Add(itemOut);
                    }
                    itemOut.InputQuantity = Math.Round((itemOut.InputQuantity * goodConvert.ConvertQuantity / goodConvert.Quantity + item.InputQuantity) / goodConvert.ConvertQuantity * goodConvert.Quantity, 2);
                    itemOut.OutputQuantity = Math.Round((itemOut.OutputQuantity * goodConvert.ConvertQuantity / goodConvert.Quantity + item.OutputQuantity) / goodConvert.ConvertQuantity * goodConvert.Quantity, 2);

                    double quantityTotal = itemOut.CloseQuantity * goodConvert.ConvertQuantity / goodConvert.Quantity + item.CloseQuantity;
                    itemOut.CloseQuantity = Math.Round(quantityTotal * goodConvert.Quantity / goodConvert.ConvertQuantity, 2);
                    itemOut.OpenQuantity = Math.Round((itemOut.OpenQuantity * goodConvert.ConvertQuantity / goodConvert.Quantity + item.OpenQuantity) / goodConvert.ConvertQuantity * goodConvert.Quantity, 2);
                    string stockUnit = "";
                    if (!string.IsNullOrEmpty(itemOut.Detail2))
                    {
                        string parentRef = itemOut.Account + ":" + itemOut.Detail1;
                        stockUnit = await _context.ChartOfAccounts.Where(x => x.Code == itemOut.Detail2 && x.ParentRef == parentRef &&
                                (string.IsNullOrEmpty(itemOut.Warehouse) || x.WarehouseCode == itemOut.Warehouse)).Select(x => x.StockUnit).FirstOrDefaultAsync();
                    }
                    else if (!string.IsNullOrEmpty(itemOut.Detail1))
                        stockUnit = await _context.ChartOfAccounts.Where(x => x.Code == itemOut.Detail1 && x.ParentRef == itemOut.Account &&
                        (string.IsNullOrEmpty(itemOut.Warehouse) || x.WarehouseCode == itemOut.Warehouse)).Select(x => x.StockUnit).FirstOrDefaultAsync();
                    else
                        stockUnit = await _context.ChartOfAccounts.Where(x => x.Code == itemOut.Account).Select(x => x.StockUnit).FirstOrDefaultAsync();


                    string oppositeStockUnit = "";
                    if (!string.IsNullOrEmpty(item.Detail2))
                    {
                        string parentRef = item.Account + ":" + item.Detail1;
                        oppositeStockUnit = await _context.ChartOfAccounts.Where(x => x.Code == item.Detail2 && x.ParentRef == parentRef &&
                                (string.IsNullOrEmpty(item.Warehouse) || x.WarehouseCode == item.Warehouse)).Select(x => x.StockUnit).FirstOrDefaultAsync();
                    }
                    else if (!string.IsNullOrEmpty(item.Detail1))
                        oppositeStockUnit = await _context.ChartOfAccounts.Where(x => x.Code == item.Detail1 && x.ParentRef == item.Account &&
                        (string.IsNullOrEmpty(item.Warehouse) || x.WarehouseCode == item.Warehouse)).Select(x => x.StockUnit).FirstOrDefaultAsync();
                    else
                        oppositeStockUnit = await _context.ChartOfAccounts.Where(x => x.Code == item.Account).Select(x => x.StockUnit).FirstOrDefaultAsync();


                    itemOut.GoodQuantity = $"{Math.Floor(itemOut.CloseQuantity)} - {stockUnit}";
                    itemOut.OppositeGoodQuantity = $"{quantityTotal - (Math.Floor(itemOut.CloseQuantity) * goodConvert.ConvertQuantity / goodConvert.Quantity)}  - {oppositeStockUnit}";
                }
            }
            return listOut;
        }
    }
}
