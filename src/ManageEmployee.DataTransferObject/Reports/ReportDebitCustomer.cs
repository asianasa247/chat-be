namespace ManageEmployee.DataTransferObject.Reports;

public class ReportDebitCustomer
{
    public string? Company { get; set; } // Tên công ty
    public string? Address { get; set; } // Địa chỉ
    public string? TaxId { get; set; } // Mã số thuế
    public string? CEOName { get; set; }
    public string? ChiefAccountantName { get; set; }
    public string? CEONote { get; set; }
    public string? ChiefAccountantNote { get; set; }
    public int MethodCalcExportPrice { get; set; } = 0;
    public double InputAmount { get; set; }
    public double ArisingAmountIncrease { get; set; }// tang
    public double ArisingAmountDecrease { get; set; }// giam
    public string? CustomerName { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerTax { get; set; }
    public string? CustomerAddress { get; set; }
    public string? CustomerPhone { get; set; }
    public double EndDebit { get; set; }// cong no cuoi ki
    public double AccumulatedDebit { get; set; }// luy ke no
    public double AccumulatedCredit { get; set; }// luy ke co
}
