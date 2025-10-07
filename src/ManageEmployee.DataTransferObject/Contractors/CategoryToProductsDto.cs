namespace ManageEmployee.DataTransferObject.Contractors;

public class CategoryToProductsDto
{
    public Guid ContractorToCategoryToProductId { get; set; }
    public Guid ContractToCategoryId { get; set; }
    public int ProductId { get; set; }
}