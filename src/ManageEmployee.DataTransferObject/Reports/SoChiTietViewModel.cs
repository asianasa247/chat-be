namespace ManageEmployee.DataTransferObject.Reports;

public class SoChiTietViewModel : LedgerReportViewModel
{
    public string? NameGood { get; set; }

    public string? DebitDetailCodeFirst { get; set; }
    public string? CreditDetailCodeFirst { get; set; }
    public string? DebitDetailCodeSecond { get; set; }
    public string? CreditDetailCodeSecond { get; set; }
    public string? CreditWarehouseCode { get; set; }
    public string? DebitWarehouseCode { get; set; }
    public double ArisingDebit { get; set; }
    public double ArisingCredit { get; set; }
    public double ResidualAmount { get; set; }
    public double ResidualDebit { get; set; }
    public double ResidualCredit { get; set; }
    public double Quantity { get; set; }
    public double ExchangeRate { get; set; }
    public long Temp { get; set; }
    public string? DetailCode { get; set; }
    public double OrginalCurrency { get; set; }
    public double Amount { get; set; }
    public double UnitPrice { get; set; }
    //5
    public string? NameOfPerson { get; set; }
    public double Thu_Amount { get; set; }
    public double Chi_Amount { get; set; }
    public double Residual_Amount { get; set; }
    //3
    public double ArisingDebit_Foreign { get; set; }
    public double ArisingCredit_Foreign { get; set; }
    public double ResidualAmount_OrginalCur { get; set; }
    public double ResidualAmount_Foreign { get; set; }
    public double Residual_Quantity { get; set; }
    public double Input_Quantity { get; set; }
    public double Output_Quantity { get; set; }
    public string? InvoiceNumber { get; set; }
}
