// File: ManageEmployee/Entities/CustomerEntities/Customer.cs
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities.CustomerEntities;

public class Customer
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string Name { get; set; } = "";
    public string? Avatar { get; set; }
    public DateTime? Birthday { get; set; }
    public GenderEnum Gender { get; set; }
    public string? Phone { get; set; }
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }
    public string Email { get; set; } = "";
    public bool? SendEmail { get; set; }
    public string? Address { get; set; }
    public string? Facebook { get; set; }
    public string? IdentityCardNo { get; set; }
    public DateTime IdentityCardIssueDate { get; set; } = DateTime.Now;
    public string? IdentityCardIssuePlace { get; set; }
    public DateTime? IdentityCardValidUntil { get; set; }
    public int? IdentityCardProvinceId { get; set; }
    public int? IdentityCardDistrictId { get; set; }
    public int? IdentityCardWardId { get; set; }
    public string? IdentityCardPlaceOfPermanent { get; set; }
    public string? IdentityCardAddressInCard { get; set; }
    public int? UserCreated { get; set; }
    public int? UserUpdated { get; set; }
    public string? Password { get; set; }
    public string? DebitCode { get; set; }
    public string? DebitDetailCodeFirst { get; set; }
    public string? DebitDetailCodeSecond { get; set; }
    public int? CustomerClassficationId { get; set; }
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public string? PriceList { get; set; }
    public int? Order { get; set; } = 0;
    public DateTime? UpdateHistoryContactAt { get; set; }
    public int Type { get; set; } = 0;//  0 khách hàng 1 nhà cung cấp 2 web
    public string? Provider { get; set; }

    public string? ProviderId { get; set; }

    public int? EventCustomerId { get; set; } // CHANGE: thêm khóa ngoại mềm (nullable) sang EventCustomer
    // Có thể bổ sung navigation nếu bạn muốn:
    // public EventCustomer? EventCustomer { get; set; }
}
