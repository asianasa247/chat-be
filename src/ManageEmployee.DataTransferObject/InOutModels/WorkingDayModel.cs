namespace ManageEmployee.DataTransferObject.InOutModels;

public class WorkingDayModel
{
    public int Id { get; set; }
    public List<string>? Days { get; set; }
    public List<string>? Holidays { get; set; }
    public int Year { get; set; }
}