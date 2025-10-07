namespace ManageEmployee.DataTransferObject.V2;

public class TaxRateV2Model
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public double Percent { get; set; }
    public string? Description { get; set; }
    public CommonModel? Debit { get; set; }
    public CommonModel? Credit { get; set; }
    public CommonModel? DebitFirst { get; set; }
    public CommonModel? CreditFirst { get; set; }
    public CommonModel? DebitSecond { get; set; }
    public CommonModel? CreditSecond { get; set; }
}
