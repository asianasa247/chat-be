using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities.WebEntities;

public class Cart : BaseEntity
{
    public int Id { get; set; }
    public int GoodId { get; set; }
    public int CustomerId { get; set; }
    public int Quantity { get; set; }
    public CartStateEnum State { get; set; }
}
