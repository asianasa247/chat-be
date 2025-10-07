using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.EventModels
{
    // --- Request (khớp form FE) ---
    public class EventRegistrationRequest
    {
        [Required]
        public int EventId { get; set; }                // BẮT BUỘC

        [Required, JsonPropertyName("fullname")]
        public string FullName { get; set; } = default!;

        [Required]
        public string Phone { get; set; } = default!;   // map cả Customer.Phone & TaxInfo.Phone

        public string? TaxCode { get; set; }
        public string? CompanyName { get; set; }

        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }

        // FE muốn address cho TaxInformation
        public string? Address { get; set; }
        public string? Email { get; set; }
        // note: bỏ qua
    }

    // --- Response cho POST ---
    public class EventRegistrationResponse
    {
        public int CustomerId { get; set; }
        public int? CustomerTaxInformationId { get; set; }
        public int EventCustomerId { get; set; }
        public bool IsNewCustomer { get; set; }
    }

    // --- View item cho GET list ---
    public class EventRegistrationListItem
    {
        public int CustomerId { get; set; }
        public DateTime CustomerCreatedAt { get; set; }

        public int EventCustomerId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? EventCode { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime? EventEndDate { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }          // từ Customer.Phone (có fallback)
        public int? ProvinceId { get; set; }
        public int? DistrictId { get; set; }

        public string? CompanyName { get; set; }    // TaxInfo
        public string? TaxCode { get; set; }
        public string? TaxPhone { get; set; }
        public string? TaxAddress { get; set; }
    }

    // --- View detail cho GET by customer ---
    public class EventRegistrationDetail : EventRegistrationListItem { }
}
