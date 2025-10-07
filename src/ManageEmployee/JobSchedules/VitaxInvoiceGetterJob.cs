using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;
using ManageEmployee.Entities.LedgerEntities.VitaxEntities;
using ManageEmployee.Helpers;
using ManageEmployee.HttpClients;
using ManageEmployee.JobSchedules.Interface;
using ManageEmployee.Services.CompanyServices;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.Companies;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ManageEmployee.JobSchedules
{
    public class VitaxInvoiceGetterJob : IVitaxInvoiceGetterJob
    {
        private readonly IVitaxOneClient _httpClient;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IDbContextFactory _dbContextFactory;
        private readonly IAesEncryptionHelper _aes;
        private readonly IVitaxOneClient _vitaxClient;
        private readonly ApplicationDbContext _currentContext;
        
        public VitaxInvoiceGetterJob(
            IVitaxOneClient httpClient,
            IConnectionStringProvider connectionStringProvider,
            IDbContextFactory dbContextFactory,
            IAesEncryptionHelper aes,
            IVitaxOneClient vitaxClient,
            ApplicationDbContext currentContext)
        {
            _httpClient = httpClient;
            _connectionStringProvider = connectionStringProvider;
            _dbContextFactory = dbContextFactory;
            _aes = aes;
            _vitaxClient = vitaxClient;
            _currentContext = currentContext;
        }

        public async Task<(bool success, string msg)> GetInvoiceJob(DateTime? fromAt, DateTime? toAt)
        {
            try
            {
                var dataSellers = DataSellerHelper.GetData();
                if (dataSellers?.Companies == null || !dataSellers.Companies.Any())
                    return (false,"Không có công ty");

                var databaseNames = dataSellers.Companies.Select(x => x.Id).ToList();

                foreach (var dbName in databaseNames)
                {
                    var connectionStr = _connectionStringProvider.GetConnectionString(dbName);

                    await using var _context = _dbContextFactory.GetDbContext(connectionStr);
                    var company = await _context.Companies.FirstOrDefaultAsync();
                    if (company == null)
                        continue;
                    if (string.IsNullOrEmpty(company.PasswordAccountTax)) continue;
                    var passwordAccountTax = _aes.Decrypt(company.PasswordAccountTax);
                    if(fromAt == null)
                    {
                        fromAt = await _context.VintaxInvoiceIns
                        .OrderByDescending(x => x.InvoiceDate)
                        .Select(x => x.InvoiceDate)
                        .FirstOrDefaultAsync();
                    }
                    

                    if (fromAt == null)
                        fromAt = company.SyncInvoiceAt ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                    if(toAt == null)
                    {
                        toAt = DateTime.Today;
                    }

                    var invoiceIds = await _context.VintaxInvoiceIns
                        .Where(x => x.InvoiceDate != null && x.InvoiceDate.Value.Date >= fromAt.Value.Date && x.InvoiceDate.Value.Date < toAt.Value.Date )
                        .Select(x => x.Id)
                        .ToListAsync();

                    var requestUri = $"Invoices/get-list-invoice?datefrom={fromAt:yyyy-MM-dd}&dateto={toAt:yyyy-MM-dd}&mst={company.MST}";
                    await _vitaxClient.Login(passwordAccountTax, company.MST);

                    var responseModel = await _httpClient.GetAsync<VintaxInvoiceInResponseModel>(requestUri, company.MST, passwordAccountTax);

                    if (responseModel?.result == null || !responseModel.result.Any())
                        continue;

                    var results = responseModel.result.Where(x => !invoiceIds.Contains(x.id)).ToList();

                    if (!results.Any())
                        continue;


                    foreach (var chunk in results.Chunk(50))
                    {
                        var vitaxAdds = chunk.Select(x => new VintaxInvoiceIn
                        {
                                SellerName = x.nbten,
                                SellerAddress = x.nbdchi,
                                InvoiceNumber = x.shdon,
                                TotalAmount = x.tgtttbso,
                                BuyerTax = x.nmmst,
                                BuyerAddress = x.nmdchi,
                                InvoiceDate = x.ntao,
                                BuyerName = x.nmten,
                                Id = x.id,
                                InvoiceCode = x.khhdon,
                                InvoiceCodeNumbber = x.khmshdon,
                                SellerTax = x.nbmst
                            }).ToList();

                        await _context.VintaxInvoiceIns.AddRangeAsync(vitaxAdds);
                        var details = new List<VintaxInvoiceInDetail>();
                        var semaphore = new SemaphoreSlim(5);

                        var tasks = vitaxAdds.Select(async invoice =>
                        {
                            await semaphore.WaitAsync();
                            try
                            {
                                var tempDetails = new List<VintaxInvoiceInDetail>();
                                await GetVinTaxDetail(invoice, company.MST, passwordAccountTax, tempDetails);
                                lock (details)
                                {
                                    details.AddRange(tempDetails);
                                }
                            }
                            finally
                            {
                                semaphore.Release();
                            }
                        });

                        await Task.WhenAll(tasks);

                        if (details.Any())
                        {
                            await _context.VintaxInvoiceInDetails.AddRangeAsync(details);
                        }

                    }
                    try
                    {
                        company.SyncInvoiceAt = DateTime.Now;
                        _context.Companies.Update(company);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception dbEx)
                    {
                        return (false, $"Lỗi khi lưu dữ liệu DB: {dbEx.Message}");
                    }
                }
                return (true, "Dồng hộ hóa đơn thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VitaxInvoiceGetterJob: {ex.Message}");
                return (false, ex.Message);
                // Có thể log chi tiết hơn tại đây
            }
        }

        public async Task<(bool success, string msg)> GetInvoice(DateTime? fromAt, DateTime? toAt)
        {
            try
            {
                var company = await _currentContext.Companies.FirstOrDefaultAsync();
                if (company == null)
                    return (false, $"Không có công ty");

                if (string.IsNullOrEmpty(company.PasswordAccountTax)) return (false, $"Chưa có thông tin tài khoản thuế");

                var passwordAccountTax = _aes.Decrypt(company.PasswordAccountTax);
                if (fromAt == null)
                {
                    fromAt = await _currentContext.VintaxInvoiceIns
                    .OrderByDescending(x => x.InvoiceDate)
                    .Select(x => x.InvoiceDate)
                    .FirstOrDefaultAsync();
                }


                if (fromAt == null)
                    fromAt = company.SyncInvoiceAt ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                if (toAt == null)
                {
                    toAt = DateTime.Today;
                }

                var invoiceIds = await _currentContext.VintaxInvoiceIns
                    .Where(x => x.InvoiceDate != null && x.InvoiceDate.Value.Date >= fromAt.Value.Date && x.InvoiceDate.Value.Date < toAt.Value.Date)
                    .Select(x => x.Id)
                    .ToListAsync();

                var requestUri = $"Invoices/get-list-invoice?datefrom={fromAt:yyyy-MM-dd}&dateto={toAt:yyyy-MM-dd}&mst={company.MST}";
                await _vitaxClient.Login(passwordAccountTax, company.MST);

                var responseModel = await _httpClient.GetAsync<VintaxInvoiceInResponseModel>(requestUri, company.MST, passwordAccountTax);

                if (responseModel?.result == null || !responseModel.result.Any())
                    return (true, "Dồng hộ hóa đơn thành công");

                var results = responseModel.result.Where(x => !invoiceIds.Contains(x.id)).ToList();

                if (!results.Any())
                    return (true, "Dồng hộ hóa đơn thành công");


                foreach (var chunk in results.Chunk(50))
                {
                    var vitaxAdds = chunk.Select(x => new VintaxInvoiceIn
                    {
                        SellerName = x.nbten,
                        SellerAddress = x.nbdchi,
                        InvoiceNumber = x.shdon,
                        TotalAmount = x.tgtttbso,
                        BuyerTax = x.nmmst,
                        BuyerAddress = x.nmdchi,
                        InvoiceDate = x.ntao,
                        BuyerName = x.nmten,
                        Id = x.id,
                        InvoiceCode = x.khhdon,
                        InvoiceCodeNumbber = x.khmshdon,
                        SellerTax = x.nbmst
                    }).ToList();

                    await _currentContext.VintaxInvoiceIns.AddRangeAsync(vitaxAdds);
                    var details = new List<VintaxInvoiceInDetail>();
                    var semaphore = new SemaphoreSlim(5);

                    var tasks = vitaxAdds.Select(async invoice =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            var tempDetails = new List<VintaxInvoiceInDetail>();
                            await GetVinTaxDetail(invoice, company.MST, passwordAccountTax, tempDetails);
                            lock (details)
                            {
                                details.AddRange(tempDetails);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                    await Task.WhenAll(tasks);

                    if (details.Any())
                    {
                        await _currentContext.VintaxInvoiceInDetails.AddRangeAsync(details);
                    }

                }
                try
                {
                    company.SyncInvoiceAt = DateTime.Now;
                    _currentContext.Companies.Update(company);
                    await _currentContext.SaveChangesAsync();
                }
                catch (Exception dbEx)
                {
                    return (false, $"Lỗi khi lưu dữ liệu DB: {dbEx.Message}");
                }

                return (true, "Dồng hộ hóa đơn thành công");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VitaxInvoiceGetterJob: {ex.Message}");
                return (false, ex.Message);
                // Có thể log chi tiết hơn tại đây
            }
        }

        public async Task GetVinTaxDetail(VintaxInvoiceIn vintaxInvoice,string MST, string passwordAccountTax, List<VintaxInvoiceInDetail> ledgers)
        {

            var requestUri = $"Invoices/invoice-detail?nbmst={vintaxInvoice.SellerTax}&khhdon={vintaxInvoice.InvoiceCode}&shdon={vintaxInvoice.InvoiceNumber}&khmshdon={vintaxInvoice.InvoiceCodeNumbber}&mst={MST}";
            var responseModel = await _httpClient.GetAsync<VintaxInvoiceInModel>(requestUri, MST, passwordAccountTax);
            var results = responseModel.result;
            if (results != null && results.Any())
            {
                foreach (var result in results)
                {
                    foreach (var hdhhdvu in result.hdhhdvu)
                    {
                        ledgers.Add(new VintaxInvoiceInDetail
                        {
                            InvoiceId = vintaxInvoice.Id,   
                            Quantity = hdhhdvu.sluong,
                            UnitPrice = hdhhdvu.dgia,
                            Amount = hdhhdvu.thtien,
                            GoodName = hdhhdvu.ten,
                            StockUnit = hdhhdvu.dvtinh
                        });
                    }
                }
            }
        }

        public async Task RunGetInvoiceJobWrapper()
        {
            var today = DateTime.Today;
            int day = today.Day;
            int lastDay = DateTime.DaysInMonth(today.Year, today.Month);

            int[] fixedDays = new[] { 5, 10, 15, 20, 25, 30 };

            if (fixedDays.Contains(day))
            {
                // Chạy các ngày cố định 5,10,15,20,25,30
                var result = await GetInvoiceJob(null, null);
                Console.WriteLine($"[VitaxInvoiceJob] Success: {result.success} - Message: {result.msg}");
                return;
            }

            if (day == lastDay)
            {
                // Chạy ngày cuối tháng nếu khác 30
                var result = await GetInvoiceJob(null, null);
                Console.WriteLine($"[VitaxInvoiceJob] Success: {result.success} - Message: {result.msg}");
                return;
            }

            // Các ngày khác không chạy
            return;
        }
    }
}