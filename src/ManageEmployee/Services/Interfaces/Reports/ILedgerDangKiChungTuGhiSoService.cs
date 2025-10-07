using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface ILedgerDangKiChungTuGhiSoService
{
    Task<string> GetDataReport_DangKyChungTuGhiSo(LedgerReportParam _param, int year, bool isNoiBo = false);
}
