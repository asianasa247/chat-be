using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities.WebEntities;

public class Social : BaseEntity
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public SocialEnum Type { get; set; }
    public string? ShortContent { get; set; }
    public string? Image { get; set; }
    public string? Content { get; set; }
    [StringLength(500)]
    public string? FileUrl { get; set; }
    [StringLength(255)]
    public string? FileName { get; set; }
}
