using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillExporter
{
    Task<string> ExportExcelSale(BillReportRequestModel param);
    Task<string> ExportPdfGoodForSale(BillReportRequestModel param);
}
