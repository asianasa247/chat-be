using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerModelView
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string IdentityCardNo { get; set; }
    public int? CustomerClassficationId { get; set; }
    public string DebitCode { get; set; }
    public string DebitDetailCodeFirst { get; set; }
    public string DebitDetailCodeSecond { get; set; }
    public string CustomerClassficationName { get; set; }
    public bool IsAccountant { get; set; }
    public string PriceList { get; set; }
    public GenderEnum Gender { get; set; }
    public string Address { get; set; }
    public string AccountNumber { get; set; }
    public string TaxCode { get; set; }
    public string Bank { get; set; }
    public int? UserCreated { get; set; }
}
