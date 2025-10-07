using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.Cultivation
{
    public class PlantingBed : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int RegionId { get; set; }        // FK → PlantingRegion

        [Required, MaxLength(64)]
        public string Code { get; set; } = "";   // Mã luống

        [Required, MaxLength(256)]
        public string Name { get; set; } = "";   // Tên luống

        [MaxLength(1024)]
        public string? Note { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public int? Quantity { get; set; }       // Số lượng (cây, hom...)

        public int? StartYear { get; set; }      // Năm bắt đầu

        [Required]
        public int TypeId { get; set; }          // FK → PlantingType (Category = Bed)

        public DateTime? HarvestDate { get; set; }

        [ForeignKey(nameof(RegionId))]
        public PlantingRegion? Region { get; set; }

        [ForeignKey(nameof(TypeId))]
        public PlantingType? Type { get; set; }
    }
}
