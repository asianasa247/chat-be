using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillService
{
    Task<PagingResult<BillPaging>> GetAll(BillRequestModel param, bool isExportDetail = false);
    Task<Bill> Create(BillModel requests, int? orderProduceProductId = null);
    Task<BillModel> GetById(int id);
    Task<(Bill bill, List<BillDetailViewPaging> billDetails)> GetBillPdfByIdAsync(int billId);
    Task<Bill> Update(BillModel requests);
    void Delete(int id);
    Task<Bill> CreateBillEmployee(BillModel requests);
    long GetBillTrackingOrder();
    Task<(long, string)> GetBillTrackingOrder(string billType, List<int> excludeBillIds = null);
    Task<GoodForCustomeViewModel> GetGoodForCustomer(BillReportRequestModel param);
    BillForCustomerInvoice GetBillForCustomerInvoice(int id);
    Task UpdateCustomerInvoice(CustomerTaxInformation customer, int statusPrintInvoice, int billId, int year);
    Task<BillForCustomerInvoice> GetInvoiceForBill(int billId, int year);
    Task CopyBill(int billId, int userId);
    Task UpdateSurCharge(int billId, double surcharge);
    Task<IEnumerable<SelectListModel>> GetCustomerForReportBill(BillReportRequestModel param);
    Task<IEnumerable<SelectListModel>> GetUserForReportBill(BillReportRequestModel param);
    Task<IEnumerable<SelectListModel>> GetChartOfAccountForReportBill(BillReportRequestModel param);
    Task<List<BillDetailImportModel>> ImportBill(string fileName, int type);
    Task<List<Ledger>> GetLedgerFromBillId(int billId);
    Task UpdateUserSale(int id, int userId);

    Task<(bool success, string msg)> CreateInvoice(int Id);

    Task<(bool success, string msg, string link)> GetLinkInvoice(int Id);

    Task<(bool success, string msg)> DigitallySignInvoice(int Id);
    Task<(bool success, string msg)> DeleteSignInvoice(int Id);
}