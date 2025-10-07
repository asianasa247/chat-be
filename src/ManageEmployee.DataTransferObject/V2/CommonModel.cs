namespace ManageEmployee.DataTransferObject.V2;

public class CommonModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? ParentRef { get; set; }
    public bool IsForeignCurrency { get; set; }
    public int AccGroup { get; set; }
    public int Classification { get; set; }
    public bool HasDetails { get; set; }
    public string? WarehouseCode { get; set; }
    public bool DisplayInsert { get; set; }
    public string Duration { get; set; } = "";
    public int Protected { get; set; } = 0;
}
