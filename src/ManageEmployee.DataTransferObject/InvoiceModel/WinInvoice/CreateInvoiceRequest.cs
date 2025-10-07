using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.InvoiceModel.WinInvoice
{
    public class CreateInvoice
    { 
        public string invName { get; set; }
        public string invSerial { get; set; }
        public string invNumber { get; set; }
        public string invDate { get; set; }
        public string invCustomer { get; set; }
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
        public double invDscnAmnt { get; set; }
        public double invTotalAmount { get; set; }
        public string invPayment { get; set; }
        public string invExchangeRate { get; set; }
        public string invCurrency { get; set; }
        public string note { get; set; }
        public string cusType { get; set; }
        public int invAutoSign { get; set; }
        public string privateCode { get; set; }

        public string billNumber { get; set; }
        public List<InvoiceItem> items { get; set; }
    }
    public class InvoiceItem
    {
        public int itemNo { get; set; }
        public string itemCode { get; set; }
        public string itemName { get; set; }
        public string itemPack { get; set; }
        public string itemDate { get; set; }
        public string itemUnit { get; set; }
        public int itemQuantity { get; set; }
        public double itemPrice { get; set; }
        public double itemVatRate { get; set; }
        public double itemVatAmnt { get; set; }
        public double itemDscnAmnt { get; set; }
        public double itemAmountNoVat { get; set; }
        public string itemNote { get; set; }
        public int? itemPromo { get; set; }
    }

    public class CreateInvoiceRequest
    {
        public int Id { get; set; }
    }

    public class GetLinkInvoiceRequest
    {
        public string invSign { get; set; }

        public string invSample { get; set; }

        public string invRef { get; set; }

        public bool pdf { get; set; } = true;
    }

    public class DeleteInvoiceRequest
    {
        public string invcSign { get; set; }

        public string invcCode { get; set; }

        public string invRef { get; set; }

    }

    public class DigitallySignInvoiceRequest
    {
        public string invSerial { get; set; }

        public string invName { get; set; }

        public string invRef { get; set; }
    }
}
