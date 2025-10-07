using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities
{
    public class AdditionWeb : BaseEntity
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string? DbName { get; set; }

        [StringLength(255)]
        public string? UrlWeb { get; set; }

        [StringLength(255)]
        public string? ImageHost { get; set; }

        public string? ConnectionString { get; set; }
        public bool? IsActive { get; set; }
    }
}
