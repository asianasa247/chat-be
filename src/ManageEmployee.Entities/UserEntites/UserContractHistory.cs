namespace ManageEmployee.Entities.UserEntites;

public class UserContractHistory
{
    public int Id { get; set; }
    public int ContractTypeId { get; set; }
    public int UserId { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
