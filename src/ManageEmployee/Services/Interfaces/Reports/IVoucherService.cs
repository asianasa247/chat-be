using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.SelectModels;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IVoucherService
{
    Task<List<VoucherReportItem>> GenerateVoucherReportV2(DateTime from, DateTime to, string InvoiceTaxCode, string InvoiceNumber,
        string voucherType = "PT", bool isNoiBo = false);

    Task<List<VoucherInforItem>> GenerateTransactionList(DateTime from, DateTime to, int year, string voucherType = "PT",
        bool isNoiBo = false, string InvoiceNumber = "", string InvoiceTaxCode = "");

    string ExportDataVoucher(VoucherReportViewModel voucher, VoucherReportParam param);
    string ExportDataTransactionList(TransactionListViewModel voucher, TransactionListParam param);
    IEnumerable<SelectListModel> GetListInvoiceTaxCode(DateTime from, DateTime to, int year, string voucherType = "PT");
    IEnumerable<SelectListModel> GetListInvoiceNumber(DateTime from, DateTime to, int year, string voucherType = "PT");
}
