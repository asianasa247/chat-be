// File: ManageEmployee/DataTransferObject/CustomerModels/CustomerModel.cs
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerModel
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public DateTime? Birthday { get; set; }
    public GenderEnum Gender { get; set; }
    public string Phone { get; set; }
    public int? ProvinceId { get; set; }
    public int? DistrictId { get; set; }
    public int? WardId { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Facebook { get; set; }
    public string IdentityCardNo { get; set; }
    public DateTime? IdentityCardIssueDate { get; set; }
    public string IdentityCardIssuePlace { get; set; }
    public DateTime? IdentityCardValidUntil { get; set; }
    public int? IdentityCardProvinceId { get; set; }
    public int? IdentityCardDistrictId { get; set; }
    public int? IdentityCardWardId { get; set; }
    public string IdentityCardPlaceOfPermanent { get; set; }
    public string IdentityCardAddressInCard { get; set; }

    public int? UserCreated { get; set; }
    public int? UserUpdated { get; set; }
    public CustomerWithDetail Details { get; set; }

    public double ClosingDebit { get; set; } = 0;
    public string DebitCode { get; set; }
    public string DebitDetailCodeFirst { get; set; }
    public string DebitDetailCodeSecond { get; set; }
    public int? CustomerClassficationId { get; set; }
    public string CustomerClassficationName { get; set; }

    public double? TotalAmountPay { get; set; }
    public double? TotalAmountCN { get; set; }
    public int? CustomerQuoteCount { get; set; }
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public string PriceList { get; set; }
    public string ProvinceName { get; set; }
    public string DistrictName { get; set; }
    public string WardName { get; set; }

    public int? EventCustomerId { get; set; } // CHANGE: thêm id sự kiện (nullable)
}
