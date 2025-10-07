using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.ViewModels
{
    public class InventoryProductStockViewModel
    {
        public int Index { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double OpeningStock { get; set; }
        public double ImportQuantity { get; set; }
        public double CostPrice { get; set; }
        public double ExportQuantity { get; set; }
        public double EndingStock { get; set; }
        public double EndingValue { get; set; }
        public string OppositeCode { get; set; } = string.Empty;
        public string OppositeName { get; set; } = string.Empty;
        public int OppositeOpeningStock { get; set; }
        public int OppositeImportQuantity { get; set; }
        public int OppositeExportQuantity { get; set; }
        public int OppositeEndingStock { get; set; }
        public double OppositeEndingCost { get; set; }
        public double EndingAvgValue { get; set; }
        public double EndingTotalValue { get; set; }
        public string EndingString { get; set; } = string.Empty;
    }
    public class InventoryProductStockSummary
    {
        public int TotalRecords { get; set; }
        public double TotalEndingValue { get; set; }
        public double TotalCostImport { get; set; }
    }
    public class InventoryStockResponse
    {
        public PagingResult<InventoryProductStockViewModel> Items { get; set; }
        public InventoryProductStockSummary Summary { get; set; }
    }
}
