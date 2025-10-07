namespace ManageEmployee.DataTransferObject.Reports;

public class TotalAmountDKCTGS
{
    public string? Title_CT { get; set; }
    public double Sum_CT { get; set; }
    public string? Title_LKDN { get; set; }
    public double Sum_LKDN { get; set; }
    public string Company { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string CEOName { get; set; } = string.Empty;
    public string ChiefAccountName { get; set; } = string.Empty;
    public string CEONote { get; set; } = string.Empty;
    public string ChiefAccountNote { get; set; } = string.Empty;
}
