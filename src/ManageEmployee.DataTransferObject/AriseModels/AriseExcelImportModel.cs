using ManageEmployee.DataTransferObject.LedgerWarehouseModels;

namespace ManageEmployee.DataTransferObject.AriseModels;

public class AriseExcelImportModel
{
    public int? Year { get; set; }
    public int? Month { get; set; }
    public List<LedgerExport>? Ledgers { get; set; }
}
