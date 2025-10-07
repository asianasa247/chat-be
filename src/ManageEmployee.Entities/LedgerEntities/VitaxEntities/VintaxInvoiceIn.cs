
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.LedgerEntities.VitaxEntities
{
    public class VintaxInvoiceIn
    {
        [Key]
        public string Id { get; set; }
        public string SellerName { get; set; }//nbten
        public string SellerAddress { get; set; }//nbdchi
        public string InvoiceNumber { get; set; }//shdon
        public string SellerTax { get; set; }//nbmst
        public string InvoiceCode { get; set; }//khhdon
        public string InvoiceCodeNumbber { get; set; }//khmshdon
        public string BuyerName { get; set; }//nmten
        public string BuyerTax { get; set; }//nmmst
        public string BuyerAddress { get; set; }//nmdchi
        public double TotalAmount { get; set; }// tgtttbso
        public DateTime? InvoiceDate { get; set; }
    }
    
}
