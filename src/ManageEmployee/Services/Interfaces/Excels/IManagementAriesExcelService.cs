namespace ManageEmployee.Services.Interfaces.Excels;

using ManageEmployee.DataTransferObject.AriseModels;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.LedgerEntities;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Defines the <see cref="IManagementAriesExcelService" />.
/// </summary>
public interface IManagementAriesExcelService
{
    /// <summary>
    /// The getLastVoucherNumberInMonth.
    /// </summary>
    /// <param name="request">The request<see cref="AriesExcelSearchModel"/>.</param>
    /// <returns>The <see cref="Ledger"/>.</returns>
    Ledger GetLastVoucherNumberInMonth(AriesExcelSearchModel request, int year);

    /// <summary>
    /// The ExportExcel.
    /// </summary>
    /// <param name="search">The search<see cref="AriesExcelSearchModel"/>.</param>
    /// <returns>The <see cref="MemoryStream"/>.</returns>
    Task<MemoryStream> ExportExcel(AriesExcelSearchModel search, int year);

    /// <summary>
    /// The UpdateOrginalVoucherNumber.
    /// </summary>
    /// <param name="ledger">The ledger<see cref="Ledger"/>.</param>
    Task UpdateOrginalVoucherNumber(AriseUpdateOrginalVoucherRequest request, int year);

    /// <summary>
    /// The ImportExcel.
    /// </summary>
    /// <param name="request">The request<see cref="List{Ledger}"/>.</param>
    Task<string> ImportExcel(List<LedgerExport> request, int year);

    /// <summary>
    /// Thay đổi loại chứng từ hoặc tháng
    /// </summary>
    /// <param name="request"></param>
    Task TransferInfoLedger(TransferModelRequest request, int year);

    /// <summary>
    /// Lay danh sach tai khoan
    /// </summary>
    /// <returns></returns>
    Task<List<ChartOfAccount>> GetDebitAndCreditAccount(int year);

    Task TransferInfoLedgerLuong(TransferModelRequest request, int year);

    Task ImportExcelLocal(int year);
    MemoryStream ExportExcelSample();
}
