using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Invoices;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PuppeteerSharp;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ManageEmployee.Services.Invoice.VintaxOne
{
    public class VintaxOneInvoiceCreator : IVintaxOneInvoiceCreator
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;

        public VintaxOneInvoiceCreator(HttpClient httpClient, ApplicationDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        public async Task CreateInvoice(int billId)
        {
            var requestUri = $"https://prod-api.vitax.one/api/invoice/add_type_2";
            string username = "0317323338";
            string password = "1234567@A";

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

            _httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + svcCredentials);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            var bill = await _context.Bills.FindAsync(billId);
            if (bill is null)
            {
                throw new ErrorException("Không tìm thấy đơn hàng");
            }
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == bill.CustomerId);
            var invoice = new VintaxOneInvoiceModel
            {
                invName = "1",
                invSerial = "C24TGT",
                invDate = DateTime.Today.ToString("yyyy-MM-dd"),
                invRef = "ath-0001-0001",
                invSubTotal = bill.TotalAmount,
                invVatRate = bill.VatRate,
                invTotalAmount = bill.TotalAmount - bill.VatRate,
                invRefDate = DateTime.Today.ToString("yyyy-MM-dd"),
                buyCode = customer?.Code,
                buyerAddress = customer?.Address,
                buyerName = customer?.Name,
                buyerPhone = customer?.Phone,
                buyerEmail = customer?.Email,
                items = new List<VintaxOneInvoiceItemModel>()
            };
            var billDetails = await _context.BillDetails.Where(x => x.BillId == billId).ToListAsync();
            foreach (var billDetail in billDetails)
            {
                var good = await _context.Goods.FindAsync(billDetail.GoodsId);
                if(good is null)
                {
                    throw new ErrorException(ErrorMessages.GoodsCodeAlreadyExist);
                }

                invoice.items.Add(new VintaxOneInvoiceItemModel
                {
                    itemCode = GoodNameGetter.GetCodeFromGood(good),
                    itemName = GoodNameGetter.GetNameFromGood(good),
                    itemQuantity = billDetail.Quantity,
                    itemPrice = billDetail.Price,
                    itemVatRate = billDetail.TaxVAT,
                });
            }

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync(requestUri, invoice);
            var json = await response.Content.ReadAsStringAsync();
            var modelError = JsonConvert.DeserializeObject<VintaxOneInvoiceResponse>(json);
            if (!string.IsNullOrEmpty(modelError.errorMessage))
            {
                throw new ErrorException(modelError.errorMessage);
            }
        }
    }
}

public class VintaxOneInvoiceModel
{
    public string invName { get; set; }
    public string invSerial { get; set; }
    public string invNumber { get; set; }
    public string invDate { get; set; }
    public string invCustomer { get; set; }//string (1|0)
    public string invRef { get; set; }
    public string invRefDate { get; set; }
    public string buyerTax { get; set; }
    public string buyCode { get; set; }
    public string buyerName { get; set; }
    public string buyerCompany { get; set; }
    public string buyerAddress { get; set; }
    public string buyerAcc { get; set; }
    public string buyerBank { get; set; }
    public string buyerEmail { get; set; }
    public string buyerPhone { get; set; }
    public string buyerFax { get; set; }
    public double invSubTotal { get; set; }
    public double invVatRate { get; set; }
    public double invVatAmount { get; set; }
    public double invTotalAmount { get; set; }
    public string invPayment { get; set; }
    public string invExchangeRate { get; set; }
    public string invCurrency { get; set; }
    public string note { get; set; }
    public string cusType { get; set; }
    public string invAutoSign { get; set; }
    public string privateCode { get; set; }
    public List<VintaxOneInvoiceItemModel> items { get; set; }
}

public class VintaxOneInvoiceItemModel
{
    public string itemNo { get; set; }
    public string itemCode { get; set; }
    public string itemName { get; set; }
    public string itemPack { get; set; }
    public string itemDate { get; set; }
    public string itemUnit { get; set; }
    public double itemQuantity { get; set; }
    public double itemPrice { get; set; }
    public double itemVatRate { get; set; }
    public double itemVatAmnt { get; set; }
    public double itemDscnAmnt { get; set; }
    public double itemAmountNoVat { get; set; }
    public string itemNote { get; set; }
}

public class VintaxOneInvoiceResponse
{
    public string data { get; set; }
    public bool isSuccess { get; set; }
    public string errorMessage { get; set; }
    public int ErrorCode { get; set; }
}