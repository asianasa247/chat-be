using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.Web;

public class WebCustomerV2Model
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public DateTime? Birthday { get; set; }
    public GenderEnum Gender { get; set; }
    public string? Phone { get; set; }
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Password { get; set; }
}

public class WebCustomerUpdateModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Avatar { get; set; }
    public DateTime? Birthday { get; set; }
    public GenderEnum Gender { get; set; }
    public string? Phone { get; set; }
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}
