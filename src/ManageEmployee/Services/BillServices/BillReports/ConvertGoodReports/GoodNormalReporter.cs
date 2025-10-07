using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.ConvertGoodReports
{
    public class GoodNormalReporter : IGoodNormalReporter
    {
        private readonly ILedgerService _ledgerServices;
        private readonly ApplicationDbContext _context;

        public GoodNormalReporter(ILedgerService ledgerServices, ApplicationDbContext context)
        {
            _ledgerServices = ledgerServices;
            _context = context;
        }

        // get good after convert
        public async Task<IEnumerable<GoodNormalReporterModel>> ReporterAsync(BillReportGoodRequestModel param, int year)
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

            var goodConverts = await _context.ConvertProducts
                                    .Where(x => x.OppositeAccount == param.Account
                                                                && (string.IsNullOrEmpty(param.Detail1) || x.OppositeDetail1 == param.Detail1)
                                                                && (string.IsNullOrEmpty(param.Detail2) ||  x.OppositeDetailName2 == param.Detail2))
                                    .ToListAsync();

            var listOut = new List<GoodNormalReporterModel>();
            foreach (var goodConvert in goodConverts)
            {
                var itemOut = new GoodNormalReporterModel
                {
                    Account = goodConvert.OppositeAccount,
                    Detail1 = goodConvert.OppositeDetail1,
                    Detail2 = goodConvert.OppositeDetail2,
                    Warehouse = goodConvert.Warehouse,
                    GoodCode = string.IsNullOrEmpty(goodConvert.OppositeDetail2) ? goodConvert.OppositeDetail1 : goodConvert.OppositeDetail2,
                    GoodName = string.IsNullOrEmpty(goodConvert.OppositeDetailName2) ? goodConvert.OppositeDetailName1 : goodConvert.OppositeDetailName2,
                };

                var goodCheck = datas.FirstOrDefault(x => x.Account == param.Account
                                                                && (string.IsNullOrEmpty(goodConvert.OppositeDetail1) || x.Detail1 == goodConvert.OppositeDetail1)
                                                                && (string.IsNullOrEmpty(goodConvert.OppositeDetail2) || x.Detail2 == goodConvert.OppositeDetail2)
                                                                && (string.IsNullOrEmpty(goodConvert.OppositeWarehouse) || x.Warehouse == goodConvert.OppositeWarehouse));

                var goodOppositeCheck = datas.FirstOrDefault(x => x.Account == param.Account
                                                                && (string.IsNullOrEmpty(goodConvert.Detail1) || x.Detail1 == goodConvert.Detail1)
                                                                && (string.IsNullOrEmpty(goodConvert.Detail2) || x.Detail2 == goodConvert.Detail2)
                                                                && (string.IsNullOrEmpty(goodConvert.Warehouse) || x.Detail2 == goodConvert.Warehouse));
                if (goodCheck != null)
                {
                    itemOut.CloseQuantity = goodCheck.OpenQuantity + goodCheck.InputQuantity - goodCheck.OutputQuantity;
                    itemOut.OpenQuantity = goodCheck.OpenQuantity;
                    itemOut.InputQuantity = goodCheck.InputQuantity;
                    itemOut.OutputQuantity = goodCheck.OutputQuantity;
                    itemOut.OppositeGoodCode = string.IsNullOrEmpty(goodConvert.Detail2) ? goodConvert.Detail1 : goodConvert.Detail2;
                    itemOut.OppositeGoodName = string.IsNullOrEmpty(goodConvert.DetailName2) ? goodConvert.DetailName1 : goodConvert.DetailName2;
                    itemOut.OppositeInputQuantity = goodOppositeCheck?.InputQuantity;
                    itemOut.OppositeOutputQuantity = goodOppositeCheck?.OutputQuantity;
                    itemOut.OppositeCloseQuantity = goodOppositeCheck?.OpenQuantity + goodOppositeCheck?.InputQuantity - goodOppositeCheck?.OutputQuantity;
                    itemOut.OppositeOpenQuantity = goodOppositeCheck?.OpenQuantity;
                }

                listOut.Add(itemOut);   

            }

            return listOut;
        }
    }
}