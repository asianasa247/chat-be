namespace ManageEmployee.DataTransferObject.AddressModels;

public class DistrictGetListModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int ProvinceId { get; set; }
}