namespace ManageEmployee.DataTransferObject.InOutModels;

public class ShiftUserDetailResponseModel
{
    public ShiftUserDetailResponseModel()
    {
        UserSymbols = new List<UserSymbol>();
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    public int? TargetId { get; set; }
    public List<UserSymbol> UserSymbols { get; set; }
}
