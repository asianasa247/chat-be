using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.LedgerEntities.VitaxEntities;
using ManageEmployee.Helpers;
using ManageEmployee.HttpClients;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Invoices;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Invoice.VintaxOne
{
    public class InvoiceInGetter : IInvoiceInGetter
    {
        private readonly IVitaxOneClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ICompanyService _companyService;
        private readonly IAesEncryptionHelper _aes;
        public InvoiceInGetter(IVitaxOneClient httpClient, ApplicationDbContext context, ICompanyService companyService, IAesEncryptionHelper aes)
        {
            _httpClient = httpClient;
            _context = context;
            _companyService = companyService;
            _aes = aes;
        }

        public async Task<PagingResult<VintaxInvoiceIn>> GetInvoice(PagingRequestFilterDateModel param)
        {
            var query = _context.VintaxInvoiceIns.Where(x => x.InvoiceDate >= param.FromAt && x.InvoiceDate < param.ToAt);

            return new PagingResult<VintaxInvoiceIn>
            {
                Data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync(),
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = await query.CountAsync(),
            };
        }

        public async Task<IEnumerable<LedgerVitaxInvoiceInModel>> GetDetail(string id)
        {
            var vitaxInvoice = await _context.VintaxInvoiceIns.FirstOrDefaultAsync(x => x.Id == id);
            var vitaxInvoiceDetails = await _context.VintaxInvoiceInDetails.Where(x => x.InvoiceId == vitaxInvoice.Id).ToListAsync();
            var ledgers = new List<LedgerVitaxInvoiceInModel>();

            if (vitaxInvoiceDetails != null && vitaxInvoiceDetails.Any())
            {
                foreach (var result in vitaxInvoiceDetails)
                {
                        ledgers.Add(new LedgerVitaxInvoiceInModel
                        {
                            OrginalCompanyName = vitaxInvoice.BuyerName,
                            OrginalAddress = vitaxInvoice.BuyerAddress,
                            InvoiceNumber = vitaxInvoice.InvoiceNumber,
                            InvoiceTaxCode = vitaxInvoice.BuyerTax,
                            InvoiceAddress = vitaxInvoice.BuyerAddress,
                            InvoiceDate = vitaxInvoice.InvoiceDate,
                            Quantity = result.Quantity,
                            UnitPrice = result.UnitPrice,
                            Amount = result.Amount,
                            GoodName = result.GoodName,
                            StockUnit = result.StockUnit
                        });
                }
            }

            return ledgers;
        }
    }
}