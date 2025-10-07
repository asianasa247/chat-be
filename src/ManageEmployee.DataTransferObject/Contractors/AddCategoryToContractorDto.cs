namespace ManageEmployee.DataTransferObject.Contractors;

public class AddCategoryToContractorDto
{
    public Guid ContractId { get; set; }
    public string? CategoryName { get; set; }
}