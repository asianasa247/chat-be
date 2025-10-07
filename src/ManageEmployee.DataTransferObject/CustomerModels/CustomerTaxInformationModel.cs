namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerTaxInformationModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string CompanyName { get; set; }
    public string Address { get; set; }
    public string TaxCode { get; set; }
    public string AccountNumber { get; set; }
    public string Bank { get; set; }
    public string Phone { get; set; }
    public string Position { get; set; }
    public string PhoneOfAccountant { get; set; }
    public List<CustomerTaxInformationAccountantModel> Accountants { get; set; }
}
