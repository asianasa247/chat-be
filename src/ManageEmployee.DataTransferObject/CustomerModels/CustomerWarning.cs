using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerWarning
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string IdentityCardNo { get; set; }
    public int NumberPurchases { get; set; }
    public DateTime? LastPurchasesAt { get; set; }
    public GenderEnum Gender { get; set; }
    public string Address { get; set; }
    public string AccountNumber { get; set; }
    public DateTime UpdateHistoryContactAt { get; set; }
}
