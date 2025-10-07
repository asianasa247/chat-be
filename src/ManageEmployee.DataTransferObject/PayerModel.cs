using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject;

public class PayerPagingationRequestModel : PagingRequestModel
{
    public int PayerType { get; set; } = 1;
}


public class PayerModelView
{
    public long Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string IdentityCardNo { get; set; }
    public string Address { get; set; }
    public string TaxCode { get; set; }
}