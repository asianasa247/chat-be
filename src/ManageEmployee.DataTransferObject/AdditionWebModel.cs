using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject
{
    public class AdditionWebModel
    {
        public string? DbName { get; set; }
        [StringLength(255)]
        public string? UrlWeb { get; set; }
        public string? ConnectionString { get; set; }
        public string ImageHost { get; set; }
        public bool IsActive { get; set; }
    }
}
