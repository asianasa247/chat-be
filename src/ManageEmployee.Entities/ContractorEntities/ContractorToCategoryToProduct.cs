namespace ManageEmployee.Entities.ContractorEntities;

public class ContractorToCategoryToProduct
{
    public Guid ContractorToCategoryToProductId { get; set; }
    public Guid ContractToCategoryId { get; set; }
    public int ProductId { get; set; }

    public virtual ContractorToCategory? Category { get; set; }
}