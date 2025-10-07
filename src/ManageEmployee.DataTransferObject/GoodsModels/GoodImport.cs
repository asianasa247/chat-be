namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodImport
{
    public string? PriceList { get; set; }
    public string? GoodsType { get; set; }
    public long MinStockLevel { get; set; }
    public long MaxStockLevel { get; set; }
    public int Status { get; set; } = 1;// 0: ngung kinh doanh; 1: dang kinh doanh
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public string? Detail1 { get; set; }
    public string? DetailName1 { get; set; }
    public string? Detail2 { get; set; }
    public string? DetailName2 { get; set; }
    public double TaxVATPercent { get; set; } = 0;
}
