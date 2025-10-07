namespace ManageEmployee.DataTransferObject.V2;

public class DescriptionV2Model
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? DocumentCode { get; set; }
    public CommonModel? Debit { get; set; }
    public CommonModel? DebitDetailFirst { get; set; }
    public CommonModel? DebitDetailSecond { get; set; }
    public CommonModel? Credit { get; set; }
    public CommonModel? CreditDetailFirst { get; set; }
    public CommonModel? CreditDetailSecond { get; set; }
}
