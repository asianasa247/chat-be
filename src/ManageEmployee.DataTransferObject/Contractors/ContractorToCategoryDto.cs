namespace ManageEmployee.DataTransferObject.Contractors;

public class ContractorToCategoryDto
{
    public Guid ContractorToCategoryId { get; set; }
    public Guid UserToContractorId { get; set; }
    public string? CategoryName { get; set; }
}