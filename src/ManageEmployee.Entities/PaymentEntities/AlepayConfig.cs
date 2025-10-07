using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.PaymentEntities
{
    public class AlepayConfig : BaseEntity
    {
        public int Id { get; set; }
        public bool IsSandbox { get; set; } = true;

        [Required, StringLength(128)]
        public string TokenKey { get; set; } = "";

        [Required, StringLength(256)]
        public string ChecksumKey { get; set; } = "";

        [StringLength(64)]
        public string? CustomMerchantId { get; set; }

        public int? AdditionWebId { get; set; }
    }
}
