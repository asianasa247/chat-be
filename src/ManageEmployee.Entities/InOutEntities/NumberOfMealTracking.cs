using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class NumberOfMealsTracking : BaseEntityCommon
{
    public long Id { get; set; }
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public int EstimatedNumberOfMeals { get; set; }
    public int Breakfast { get; set; }
    public int Lunch { get; set; }
    public int Afternoon { get; set; }
    public int Dinner { get; set; }
}