using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.DataTransferObject.SearchModels;

public class CustomerImport : Customer
{
    public string? TaxCompanyName { get; set; }
    public string? TaxTaxCode { get; set; }
    public string? TaxAddress { get; set; }
    public string? TaxPhone { get; set; }
    public string? TaxBank { get; set; }
    public string? TaxAccountNumber { get; set; }
    public string? ProvinceName { get; set; }
    public string? DistrictName { get; set; }
    public string? WardName { get; set; }
}
