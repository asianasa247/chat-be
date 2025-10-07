namespace ManageEmployee.DataTransferObject.InOutModels;

public class ShiftUserModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Note { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public List<ShiftUserDetailModel>? Items { get; set; }
}
