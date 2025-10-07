namespace ManageEmployee.Entities.ContractorEntities;

public class ContractorToCategory
{
    public Guid ContractorToCategoryId { get; set; }
    public Guid UserToContractorId { get; set; }
    public string? CategoryName { get; set; }
    public int SortOrder { get; set; }
    public bool? IsDeleted { get; set; }

    public virtual ICollection<ContractorToCategoryToProduct>? ContractorToCategoryToProducts { get; set; }
}