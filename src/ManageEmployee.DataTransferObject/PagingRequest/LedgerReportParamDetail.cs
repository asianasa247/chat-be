namespace ManageEmployee.DataTransferObject.PagingRequest;

public class LedgerReportParamDetail : LedgerReportParam
{
    public string AccountCodeReciprocal { get; set; } = string.Empty;
    public string AccountCodeDetail1Reciprocal { get; set; } = string.Empty;
    public string AccountCodeDetail2Reciprocal { get; set; } = string.Empty;
    public bool IsNewReport { get; set; }
    public int Quantity { get; set; }
    public int ConvertQuantity { get; set; }
}