using ManageEmployee.Entities.Enumerations;
using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.Web;

public class SocialViewModel
{
    public int? Id { get; set; }
    public string? Title { get; set; }
    public string? ShortContent { get; set; }
    public string? Image { get; set; }
    public string? FileUrl { get; set; }
    public string? Content { get; set; }
    public DateTime? CreateAt { get; set; }
    public SocialEnum Type { get; set; }
    public IFormFile? File { get; set; }
}
