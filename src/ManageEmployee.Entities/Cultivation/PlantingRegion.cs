using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.Cultivation
{
    public class PlantingRegion : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int CountryId { get; set; }       // Id Quốc gia

        [Required, MaxLength(64)]
        public string Code { get; set; } = "";   // Mã vùng trồng

        [Required, MaxLength(256)]
        public string Name { get; set; } = "";   // Tên vùng trồng

        [MaxLength(1024)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }    // Vĩ độ
        public double? Longitude { get; set; }   // Kinh độ

        [MaxLength(128)]
        public string? Manager { get; set; }     // Người quản lý

        public int? Quantity { get; set; }       // Số lượng (tuỳ nghiệp vụ)

        [Required]
        public int TypeId { get; set; }          // FK → PlantingType (Category = Region)

        public decimal? Area { get; set; }       // Diện tích (ha, m2... tùy bạn)

        public DateTime? StartDate { get; set; } // Ngày bắt đầu
        public DateTime? HarvestDate { get; set; } // Ngày thu hoạch

        [MaxLength(512)]
        public string? Address { get; set; }     // Địa chỉ

        [MaxLength(64)]
        public string? IssuerUnitCode { get; set; } // Mã đơn vị cấp

        [ForeignKey(nameof(TypeId))]
        public PlantingType? Type { get; set; }
    }
}
