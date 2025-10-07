namespace ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels
{
    public class LedgerVitaxInvoiceInModel
    {
        public string GoodName { get; set; }
        public string OrginalCompanyName { get; set; }
        public string OrginalAddress { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceTaxCode { get; set; }
        public string InvoiceAddress { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public double? Quantity { get; set; }
        public double? UnitPrice { get; set; }
        public double? Amount { get; set; }
        public double? InvoiceTax { get; set; }
        public string StockUnit { get; set; }

    }
}
