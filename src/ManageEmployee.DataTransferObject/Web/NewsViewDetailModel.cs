using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.Web;

public class NewsViewDetailModel
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? ShortContent { get; set; }
    public string? Content { get; set; }
    public DateTime? CreateAt { get; set; }
    public LanguageEnum Type { get; set; }
    public int? CategoryId { get; set; }
    public string? TitleEnglish { get; set; }

    public string? ShortContentEnglish { get; set; }

    public string? ContentEnglish { get; set; }
    public string? TitleKorean { get; set; }

    public string? ShortContentKorean { get; set; }

    public string? ContentKorean { get; set; }
    public List<FileDetailModel>? File { get; set; }
    public string? Author { get; set; }
    public string? PublishDate { get; set; }
}
