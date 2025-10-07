namespace ManageEmployee.DataTransferObject.AddressModels;

public class ProvinceModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? ZipCode { get; set; }
    public int SortOrder { get; set; }
}