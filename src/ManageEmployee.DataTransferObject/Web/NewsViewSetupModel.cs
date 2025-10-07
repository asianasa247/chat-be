using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.Web;

public class NewsViewSetupModel
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? ShortContent { get; set; }
    public string? Content { get; set; }
    public DateTime? CreateAt { get; set; }
    public LanguageEnum Type { get; set; }
    public int? CategoryId { get; set; }
    public List<IFormFile>? File { get; set; }
    public List<FileDetailModel>? UploadedFiles { get; set; }

    public string? TitleEnglish { get; set; }

    public string? ShortContentEnglish { get; set; }

    public string? ContentEnglish { get; set; }

    public List<IFormFile>? FileEnglish { get; set; }
    public List<FileDetailModel>? UploadedFilesEnglish { get; set; }

    public string? TitleKorean { get; set; }

    public string? ShortContentKorean { get; set; }

    public string? ContentKorean { get; set; }

    public List<IFormFile>? FileKorean { get; set; }
    public List<FileDetailModel>? UploadedFilesKorean { get; set; }
    public string? Author { get; set; }
    public string? PublishDate { get; set; }
}
