using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManageEmployee.Entities.Cultivation
{
    public enum PlantingTypeCategory
    {
        Vùng = 1, // Loại cho Vùng trồng
        Luống = 2     // Loại cho Luống
    }

    public class PlantingType : BaseEntity
    {
        [Key, Required, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required, MaxLength(64)]
        public string Code { get; set; } = "";

        [Required, MaxLength(256)]
        public string Name { get; set; } = "";

        [Required]
        public PlantingTypeCategory Category { get; set; }
    }
}
