namespace ManageEmployee.DataTransferObject.AddressModels;

public class DistrictModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? LatiLongTude { get; set; }
    public int ProvinceId { get; set; }
    public int SortOrder { get; set; }
}
