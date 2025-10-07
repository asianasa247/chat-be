namespace ManageEmployee.Entities.CategoryEntities;

public class CategoryStatusWebPeriod
{
    public CategoryStatusWebPeriod()
    {
        CategoryStatusWebPeriodGoods = new HashSet<CategoryStatusWebPeriodGood>();
    }

    public int Id { get; set; }
    public int CategoryId { get; set; }
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public int UserId { get; set; }
    public DateTime CreateAt { get; set; }
    public virtual ICollection<CategoryStatusWebPeriodGood> CategoryStatusWebPeriodGoods { get; set; }
    public virtual Category Category { get; set; }
}
