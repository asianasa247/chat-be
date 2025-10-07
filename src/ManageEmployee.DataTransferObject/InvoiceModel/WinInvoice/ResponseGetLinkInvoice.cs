using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.DataTransferObject.InvoiceModel.WinInvoice
{
    public class ResponseGetLinkInvoice
    {
        public string Action { get; set; }
        public string ReturnDate { get; set; }
        public bool IsSuccess { get; set; }
        public Data Data { get; set; }
        public List<object> TokenInfo { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
        public string InvRef { get; set; }
    }

    public class Data
    {
        public string Link { get; set; }
        public int Signed { get; set; }
    }

    public class ResponseDigitallySignInvoice
    {
        public string Action { get; set; }
        public string ReturnDate { get; set; }
        public bool IsSuccess { get; set; }
        public object? Data { get; set; }
        public List<object> TokenInfo { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorCode { get; set; }
        public string InvRef { get; set; }
    }
    
}
