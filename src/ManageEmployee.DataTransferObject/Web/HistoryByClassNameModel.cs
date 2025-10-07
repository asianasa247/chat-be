namespace ManageEmployee.DataTransferObject.Web;

public class HistoryByClassNameModel
{
    public string? ClassName { get; set; }
    public List<OptionItem>? Exercises { get; set; }
    public IsoftHistoryViewModel? FirstExercise { get; set; }
}
