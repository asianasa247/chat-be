namespace ManageEmployee.DataTransferObject.Contractors;

public class UserToContractorDto
{
    public Guid UserToContractorId { get; set; }
    public int UserId { get; set; }
    public string? Domain { get; set; }
    public bool IsDeleted { get; set; }
}