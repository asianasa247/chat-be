using Common.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Invoices;
using ManageEmployee.DataTransferObject.InvoiceModel;
using ManageEmployee.DataTransferObject.InvoiceModel.ImvoiceRequest;

namespace ManageEmployee.Services.Invoice;
public class InvoiceCreator : IInvoiceCreator
{
    private readonly AppSettingInvoice _appSettingInvoice;
    private readonly IInvoiceAuthorize _invoiceAuthorize;
    private readonly ApplicationDbContext _context;

    public InvoiceCreator(IInvoiceAuthorize invoiceAuthorize, ApplicationDbContext context, IOptions<AppSettingInvoice> appSettingInvoice)
    {
        _invoiceAuthorize = invoiceAuthorize;
        _context = context;
        _appSettingInvoice = appSettingInvoice.Value;
    }
    public async Task PerformAsync(int billId)
    {
        // not implement
        return;

        var bill = await _context.Bills.FindAsync(billId);
        if (bill is null)
            return;
        if (!bill.IsPrintBill)
            return;
        string supplierTaxCode = "0100109106-718";

        var login = await _invoiceAuthorize.PerformAsync();
        using HttpClient client = new();
        // set the base address of the API
        client.BaseAddress = new Uri(_appSettingInvoice.Endpoint);

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login);
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
        

        var generalInvoiceInfo = new GeneralInvoiceInfo()
        {
            transactionUuid = Guid.NewGuid().ToString(),
            invoiceType = "1",
            templateCode = "1/796",
            invoiceSeries = "C22TTT",
            currencyCode = "VND",
            adjustmentType = "1",
            paymentStatus = true,
            cusGetInvoiceRight = true,
            reservationCode = "681DDYKLMOEFD"
        };

        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == bill.CustomerId);
        var customerTax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == bill.CustomerId);
        var buyerInfo = new BuyerInfo()
        {
            buyerName = customer?.Name,
            buyerLegalName = customerTax?.CompanyName,
            buyerTaxCode = customerTax?.TaxCode,
            buyerAddressLine = customerTax?.Address,
            buyerPostalCode = "",
            buyerDistrictName = "",
            buyerCityName = "",
            buyerCountryCode = "84",
            buyerPhoneNumber = customer?.Phone,
            buyerFaxNumber = "",
            buyerEmail = customer?.Email,
            buyerBankName = customerTax?.Bank,
            buyerBankAccount = customerTax?.AccountNumber,
            buyerIdType = "",
            buyerIdNo = customer?.IdentityCardNo,
            buyerCode = customer?.Code,
        };

        var company = await _context.Companies.FirstOrDefaultAsync();

        var sellerInfo = new SellerInfo()
        {
            sellerLegalName = company?.Name,
            sellerTaxCode = supplierTaxCode,
            sellerAddressLine = company?.Address,
            sellerPhoneNumber = company?.Phone,
            sellerFaxNumber = company?.Fax,
            sellerEmail = company?.Email,
            sellerBankName = "",
            sellerBankAccount = "",
            sellerDistrictName = "",
            sellerCityName = "",
            sellerCountryCode = "84",
            sellerWebsite = company?.WebsiteName
        };
        var payments = new List<Payment>()
        {
               new Payment()
               {
                    paymentMethodName = bill.TypePay
               }
        };
        var billDetails = await _context.BillDetails.Where(x => x.BillId == bill.Id).ToListAsync();

        var itemInfos = new List<ItemInfo>();
        int i = 0;
        foreach (var billDetail in billDetails)
        {
            var goods = await _context.Goods.FindAsync(billDetail.GoodsId);
            if (goods is null)
                continue;

            var itemInfo = new ItemInfo()
            {
                lineNumber = i ++,
                selection = 1,
                itemCode = GoodNameGetter.GetCodeFromGood(goods),
                itemName = GoodNameGetter.GetNameFromGood(goods),
                unitCode = null,
                unitName = goods.StockUnit,
                unitPrice = billDetail.UnitPrice,
                quantity = billDetail.Quantity,
                itemTotalAmountWithoutTax = billDetail.UnitPrice * billDetail.Quantity,
                taxPercentage = 10,
                taxAmount = billDetail.TaxVAT,
                itemNote = null,
                discount = billDetail.DiscountPrice,
                itemDiscount = billDetail.Quantity,
                itemTotalAmountWithTax = (billDetail.UnitPrice + billDetail.TaxVAT) * billDetail.Quantity
            };
            itemInfos.Add(itemInfo);
        }
        var summarizeInfo = new SummarizeInfo()
        {
            sumOfTotalLineAmountWithoutTax = bill.TotalAmount,
            totalAmountWithoutTax = bill.TotalAmount,
            totalTaxAmount = bill.TotalAmount,
            totalAmountWithTax = bill.TotalAmount,
            totalAmountAfterDiscount = bill.TotalAmount,
            totalAmountWithTaxInWords = AmountExtension.ConvertFromDecimal(bill.TotalAmount),
            discountAmount = bill.DiscountPrice,
            taxPercentage = 10
        };

        var taxBreakdowns = new List<TaxBreakdown>()
        {
           new TaxBreakdown()
           {
                  taxPercentage= 10,
                  taxableAmount= 400000,
                  taxAmount= 0
            }
        };

        CreateImvoiceRequest request = new()
        {
            generalInvoiceInfo = generalInvoiceInfo,
            buyerInfo = buyerInfo,
            sellerInfo = sellerInfo,
            payments = payments,
            itemInfo = itemInfos,
            metadata = new List<Metadata>(),
            summarizeInfo = summarizeInfo,
            taxBreakdowns = taxBreakdowns
        };

        HttpResponseMessage response = await client.PostAsJsonAsync("/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/createOrUpdateInvoiceDraft/" + supplierTaxCode, request);

        // check if the response was successful
        if (response.IsSuccessStatusCode)
        {
            // read the response content as a string
            return;
        }
        else
        {
            return;
            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
        }
    }
}
