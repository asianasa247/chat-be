using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.DataTransferObject;

public class TaxRateModel
{
    public long Id { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }
    public decimal Percent { get; set; }
    public string? Description { get; set; }

    public int Type { get; set; }

    public int Order { get; set; }
    public CommonModel? Debit { get; set; }
    public CommonModel? Credit { get; set; }
    public CommonModel? DebitFirst { get; set; }
    public CommonModel? CreditFirst { get; set; }
    public CommonModel? DebitSecond { get; set; }
    public CommonModel? CreditSecond { get; set; }
}
