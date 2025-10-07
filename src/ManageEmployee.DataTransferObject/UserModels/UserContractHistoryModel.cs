namespace ManageEmployee.DataTransferObject.UserModels;

public class UserContractHistoryModel
{
    public int Id { get; set; }
    public string? ContractTypeName { get; set; }
    public string? UserName { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
