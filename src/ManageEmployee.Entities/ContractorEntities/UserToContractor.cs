namespace ManageEmployee.Entities.ContractorEntities;

public class UserToContractor
{
    public Guid UserToContractorId { get; set; }
    public int UserId { get; set; }
    public string? Domain { get; set; }
    public bool? IsDeleted { get; set; }
    public virtual ICollection<ContractorToCategory>? ContractorToCategories { get; set; }
}