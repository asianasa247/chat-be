using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities;

public class News : BaseEntity
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    [StringLength(255)]
    public string? Title { get; set; }
    public LanguageEnum Type { get; set; }
    public string? ShortContent { get; set; }
    [StringLength(1000)]
    public string? Image { get; set; }
    public string? Content { get; set; }

    public string? TitleEnglish { get; set; }

    public string? ShortContentEnglish { get; set; }

    public string? ContentEnglish { get; set; }

    public string? ImageEnglish { get; set; }

    public string? TitleKorean { get; set; }

    public string? ShortContentKorean { get; set; }

    public string? ContentKorean { get; set; }

    public string? ImageKorean { get; set; }
    public string? Author { get; set; }
    public string? PublishDate { get; set; }
}
