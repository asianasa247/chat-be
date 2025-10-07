namespace ManageEmployee.DataTransferObject.Contractors;

public class AddCategoryToProductsDto
{
    public AddCategoryToProductsDto()
    {
        ProductIds = new List<int>();
    }

    public Guid CategoryId { get; set; }
    public List<int> ProductIds { get; set; }
}