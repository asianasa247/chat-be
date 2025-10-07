namespace ManageEmployee.Models
{
    public class WinInvoiceConfig
    {
        public string APIBaseUrl { get; set; }
        public string CreateInvoice { get; set; }
        public string GetLinkInvoice { get; set; }
        public string DigitallySignInvoice { get; set; }

        public string CheckSignInvoice { get; set; }
        public string DeleteSignInvoice { get; set; }
    }
}
