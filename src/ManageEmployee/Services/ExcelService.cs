using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Excels;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace ManageEmployee.Services;

public class ExcelService: IExcelService
{
    private readonly ApplicationDbContext _context;

    public ExcelService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task PrepareLocationRawSheetDataExcel(ExcelPackage package, ExcelWorksheet mainSheet,
        int startRowIndex, int recordsCount, int provinceCol, int districtCol, int wardCol, bool isMinusIndex = false)
    {
        int endRowIndex = startRowIndex + recordsCount;
        var wards = await (from province in _context.Provinces
            join district in _context.Districts on province.Id equals district.ProvinceId
                into districtDefault
            from district in districtDefault.DefaultIfEmpty()
            join ward in _context.Wards on district.Id equals ward.DistrictId
                into wardsDefault
            from ward in wardsDefault.DefaultIfEmpty()
            select new
            {
                Province = province.Name,
                District = district.Name,
                Ward = ward.Name
            }).ToListAsync();

        var provinces = wards.Select(x => x.Province).Distinct().ToList();
        var districts = wards.GroupBy(x => new { x.Province, x.District }).ToList();

        // Create Raw Sheet
        var rawSheetName = "RawData";
        var rawDataSheet = package.Workbook.Worksheets.Add(rawSheetName);

        #region Add Province Table

        var provinceRowIndex = 1;
        foreach (var province in provinces)
        {
            provinceRowIndex++;
            rawDataSheet.Cells[provinceRowIndex, 1].Value = province;
        }

        // Create province table
        var provinceTable = "ProvinceTable";
        var tableDataRange = rawDataSheet.Cells[1, 1, provinceRowIndex, 1];
        var table = rawDataSheet.Tables.Add(tableDataRange, provinceTable);
        table.Columns[0].Name = "Province";

        #endregion

        #region District Table

        var districtRowIndex = 1;
        var districtProvinceColIndex = 4;
        var districtColIndex = 5;

        foreach (var district in districts)
        {
            districtRowIndex++;
            rawDataSheet.Cells[districtRowIndex, districtProvinceColIndex].Value = district.Key.Province;
            rawDataSheet.Cells[districtRowIndex, districtColIndex].Value = district.Key.District;
        }

        // Create province districts table
        var districtTableName = "DistrictTable";
        var districtTable = rawDataSheet.Tables.Add(
            rawDataSheet.Cells[1, districtProvinceColIndex, districtRowIndex, districtColIndex],
            districtTableName);
        districtTable.Columns[0].Name = "Province";
        districtTable.Columns[1].Name = "District";

        #endregion

        #region Ward Table

        var wardRowIndex = 1;
        var wardProvinceColIndex = 7;
        var wardDistrictColIndex = 8;
        var wardColIndex = 9;

        foreach (var ward in wards)
        {
            wardRowIndex++;
            rawDataSheet.Cells[wardRowIndex, wardProvinceColIndex].Value = ward.Province;
            rawDataSheet.Cells[wardRowIndex, wardDistrictColIndex].Value = ward.District;
            rawDataSheet.Cells[wardRowIndex, wardColIndex].Value = ward.Ward;
        }

        // Create ward table
        var wardTableName = "WardTable";
        var wardTable = rawDataSheet.Tables.Add(
            rawDataSheet.Cells[1, wardProvinceColIndex, wardRowIndex, wardColIndex], wardTableName);
        wardTable.Columns[0].Name = "Province";
        wardTable.Columns[1].Name = "District";
        wardTable.Columns[2].Name = "Ward";

        #endregion

        // Set main sheet
        // Province
        string provinceCell = GetCellAddress(startRowIndex, isMinusIndex ? provinceCol -1 : provinceCol);
        string districtCell = GetCellAddress(startRowIndex, isMinusIndex ? districtCol -1 : districtCol);

        var headerRowIndex = startRowIndex - 1;
        mainSheet.Cells[headerRowIndex, provinceCol].Value = "Tỉnh/Thành phố";
        var provinceColRange = mainSheet.Cells[startRowIndex, provinceCol, endRowIndex, provinceCol];
        provinceColRange.DataValidation.AddListDataValidation().Formula.ExcelFormula =
            $"=INDIRECT(\"{provinceTable}[Province]\")";

        // District
        mainSheet.Cells[headerRowIndex, districtCol].Value = "Quận/Huyện";
        var districtColRange = mainSheet.Cells[startRowIndex, districtCol, endRowIndex, districtCol];
        var indirectFormula = $"INDIRECT(\"{districtTableName}[Province]\")";
        var districtTableStartColumn = "$D$1";
        var districtFormula =
            $"=OFFSET({rawSheetName}!{districtTableStartColumn},MATCH({provinceCell},{indirectFormula},0),1,COUNTIF({indirectFormula},{provinceCell}),1)";
        districtColRange.DataValidation.AddListDataValidation().Formula.ExcelFormula = districtFormula;

        // Ward
        mainSheet.Cells[headerRowIndex, wardCol].Value = "Xã/Phường";
        var wardColRange = mainSheet.Cells[startRowIndex, wardCol, endRowIndex, wardCol];
        var wardProvinceIndirectFormula = $"INDIRECT(\"{wardTableName}[Province]\")";
        var wardDistrictIndirectFormula = $"INDIRECT(\"{wardTableName}[District]\")";
        var matchFormula =
            $"MATCH({provinceCell}&{districtCell}, INDEX({wardProvinceIndirectFormula}&{wardDistrictIndirectFormula},0), 0)";
        var countIfFormula =
            $"COUNTIFS({wardProvinceIndirectFormula},{provinceCell},{wardDistrictIndirectFormula},{districtCell})";
        var wardFormula = $"=OFFSET({rawSheetName}!$G$1,{matchFormula},2,{countIfFormula},1)";
        wardColRange.DataValidation.AddListDataValidation().Formula.ExcelFormula = wardFormula;
    }
    
    private string GetCellAddress(int row, int column)
    {
        string columnLetter = GetColumnLetter(column);
        return $"{columnLetter}{row}";
    }

    private string GetColumnLetter(int column)
    {
        int dividend = column;
        string columnLetter = string.Empty;

        while (dividend > 0)
        {
            int modulo = (dividend - 1) % 26;
            columnLetter = Convert.ToChar(65 + modulo) + columnLetter;
            dividend = (dividend - modulo) / 26;
        }

        return columnLetter;
    }
    
}