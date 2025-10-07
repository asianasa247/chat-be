namespace ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels
{
    public class VintaxInvoiceInCommonResponse
    {
        public int total { get; set; }
        public string status { get; set; }
    }
    public class VintaxInvoiceInResponse<T>: VintaxInvoiceInCommonResponse
    {
        public List<T> result { get; set; }
    }
}