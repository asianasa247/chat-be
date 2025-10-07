using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.ConvertGoodReports
{
    public class GoodOppositeReporter : IGoodOppositeReporter
    {
        private readonly ILedgerService _ledgerServices;
        private readonly ApplicationDbContext _context;

        public GoodOppositeReporter(ILedgerService ledgerServices, ApplicationDbContext context)
        {
            _ledgerServices = ledgerServices;
            _context = context;
        }
        // get good after convert
        public async Task<IEnumerable<GoodConvertReporterModel>> ReporterAsync(BillReportGoodRequestModel param, int year)
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
            var listOut = new List<GoodConvertReporterModel>();

            foreach (var item in datas)
            {
                var goodConvert = await _context.ConvertProducts
                                    .FirstOrDefaultAsync(x => x.Account == item.Account
                                                                && x.Detail1 == item.Detail1
                                                                && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == item.Detail2)
                                                                && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == item.Warehouse));
                item.CloseQuantity = item.OpenQuantity + item.InputQuantity - item.OutputQuantity;
                if (goodConvert is null)
                {
                    listOut.Add(new GoodConvertReporterModel
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
                    var itemOut = listOut.FirstOrDefault(x => x.Account == goodConvert.OppositeAccount
                                                                && x.Detail1 == goodConvert.OppositeDetail1
                                                                 && (string.IsNullOrEmpty(x.Detail2) || x.Detail2 == goodConvert.OppositeDetail2)
                                                                && (string.IsNullOrEmpty(x.Warehouse) || x.Warehouse == goodConvert.OppositeWarehouse));
                    if (itemOut is null)
                    {
                        itemOut = new GoodConvertReporterModel
                        {
                            Account = goodConvert.OppositeAccount,
                            Detail1 = goodConvert.OppositeDetail1,
                            Detail2 = goodConvert.OppositeDetail2,
                            Warehouse = goodConvert.OppositeWarehouse,
                            GoodCode = string.IsNullOrEmpty(goodConvert.OppositeDetail2) ? goodConvert.OppositeDetail1 : goodConvert.OppositeDetail2,
                            GoodName = string.IsNullOrEmpty(goodConvert.OppositeDetailName2) ? goodConvert.OppositeDetailName1 : goodConvert.OppositeDetailName2,
                        };
                        listOut.Add(itemOut);
                    }
                    itemOut.InputQuantity += item.InputQuantity / goodConvert.Quantity * goodConvert.ConvertQuantity;
                    itemOut.OutputQuantity += item.OutputQuantity / goodConvert.Quantity * goodConvert.ConvertQuantity;
                    itemOut.CloseQuantity += item.CloseQuantity / goodConvert.Quantity * goodConvert.ConvertQuantity;
                    itemOut.OpenQuantity += item.OpenQuantity / goodConvert.Quantity * goodConvert.ConvertQuantity;
                }
            }
            return listOut;
        }
    }
}
