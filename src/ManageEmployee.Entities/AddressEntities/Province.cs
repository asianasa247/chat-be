namespace ManageEmployee.Entities.AddressEntities;

public class Province
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Type { get; set; }
    public string? ZipCode { get; set; }
    public int SortCode { get; set; }
    public bool IsDeleted { get; set; }
    public int Area { get; set; }//0: mien bac; 1:mien nam

}