namespace ManageEmployee.Entities.BaseEntities;

public class BaseEntityCommon
{
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int UserCreated { get; set; } = 0;
    public int UserUpdated { get; set; } = 0;
}
