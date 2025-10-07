using OfficeOpenXml;

namespace ManageEmployee.Services.Interfaces.Excels;

public interface IExcelService
{
    Task PrepareLocationRawSheetDataExcel(ExcelPackage package, ExcelWorksheet mainSheet, int startRowIndex,
        int recordsCount, int provinceCol, int districtCol, int wardCol, bool isMinusIndex = false);
}
