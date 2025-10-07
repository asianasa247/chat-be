namespace ManageEmployee.Entities.AddressEntities;

public class Ward
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int DistrictId { get; set; }
    public int SortCode { get; set; }
    public bool IsDeleted { get; set; }
}