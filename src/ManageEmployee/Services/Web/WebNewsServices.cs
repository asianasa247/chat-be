using Common.Helpers;
using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace ManageEmployee.Services.Web;

public class WebNewsServices : IWebNewsService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public WebNewsServices(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task Delete(int id)
    {
        var itemDelete = await _context.News.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefaultAsync();
        if (itemDelete != null)
        {
            itemDelete.IsDelete = true;
            itemDelete.DeleteAt = DateTime.Now;
            _context.News.Update(itemDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<NewsViewModel>> GetAll()
    {
        return await _context.News.Where(x => !x.IsDelete).Select(x => new NewsViewModel
        {
            Id = x.Id,
            Title = x.Title,
            ShortContent = x.ShortContent,
            Content = x.Content,
            TitleEnglish = x.TitleEnglish,
            ShortContentEnglish = x.ShortContentEnglish,
            ContentEnglish = x.ContentEnglish,
            TitleKorean = x.TitleKorean,
            ShortContentKorean = x.ShortContentKorean,
            ContentKorean = x.ShortContentKorean,
        }).ToListAsync();



    }
   public async Task<IEnumerable<NewsViewModel>> GetByCategory(int categoryId)
    {
        return await _context.News.Where(x => !x.IsDelete && x.CategoryId == categoryId).Select(x => new NewsViewModel
        {
            Id = x.Id,
            Title = x.Title,
            ShortContent = x.ShortContent,
            Content = x.Content,
            TitleEnglish = x.TitleEnglish,
            ShortContentEnglish = x.ShortContentEnglish,
            ContentEnglish = x.ContentEnglish,
            TitleKorean = x.TitleKorean,
            ShortContentKorean = x.ShortContentKorean,
            ContentKorean = x.ShortContentKorean,
            Images = x.Image.Deserialize<List<FileDetailModel>>(),
            CreateAt =x.CreatedAt,
            Author = x.Author,
            PublishDate = x.PublishDate,
            
        }).ToListAsync();
    }
    public async Task<NewsViewDetailModel> GetById(int id)
    {
        var itemData = await _context.News.Where(x => x.Id == id && !x.IsDelete).FirstOrDefaultAsync();
        if (itemData is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        return new NewsViewDetailModel
        {
            Id = itemData.Id,
            Title = itemData.Title,
            ShortContent = itemData.ShortContent,
            File = JsonConvert.DeserializeObject<List<FileDetailModel>>(itemData.Image),
            Content = itemData.Content,
            CreateAt = itemData.CreatedAt,
            Type = itemData.Type,
            CategoryId = itemData.CategoryId,
            TitleEnglish = itemData.TitleEnglish,
            ShortContentEnglish = itemData.ShortContentEnglish,
            ContentEnglish = itemData.ContentEnglish,
            TitleKorean = itemData.TitleKorean,
            ShortContentKorean = itemData.ShortContentKorean,
            ContentKorean = itemData.ShortContentKorean,
            Author = itemData.Author,
            PublishDate = itemData.PublishDate,
        };
    }

    public async Task<PagingResult<NewsViewModel>> SearchNews(WebNewPagingRequestModel searchRequest)
    {
        try
        {
            var news = _context.News.Where(x => !x.IsDelete && x.Id != 0)
                                         .Where(x => searchRequest.Type == null || x.Type == searchRequest.Type)
                                         .Where(x => string.IsNullOrEmpty(searchRequest.SearchText) ||
                                                    (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(searchRequest.SearchText.ToLower())))
                                         .Where(x => searchRequest.CategoryId == null || x.CategoryId == searchRequest.CategoryId)

                                         .Select(x => new NewsViewModel
                                         {
                                             Id = x.Id,
                                             Title = x.Title,
                                             ShortContent = x.ShortContent,
                                             Content = x.Content,
                                             CreateAt = x.CreatedAt,
                                             Type = x.Type,
                                             Images = x.Image.Deserialize<List<FileDetailModel>>(),
                                             CategoryId = x.CategoryId,
                                             TitleEnglish = x.TitleEnglish,
                                             ShortContentEnglish = x.ShortContentEnglish,
                                             ContentEnglish = x.ContentEnglish,
                                             TitleKorean = x.TitleKorean,
                                             ShortContentKorean = x.ShortContentKorean,
                                             ContentKorean = x.ContentKorean,
                                             Author = x.Author,
                                             PublishDate = x.PublishDate,
                                             
                                         });

            return new PagingResult<NewsViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = await news.CountAsync(),
                Data = await news.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<NewsViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = 0,
                Data = new List<NewsViewModel>()
            };
        }
    }

    public async Task<PagingResult<NewsViewModel>> GetNewHeadLine(WebNewHeadLinePagingRequestModel searchRequest)
    {
        try
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Code.ToLower() == searchRequest.CategoryCode!.ToLower());
            if (category == null)
            {
                return new PagingResult<NewsViewModel>()
                {
                    CurrentPage = searchRequest.Page,
                    PageSize = searchRequest.PageSize,
                    TotalItems = 0,
                    Data = new List<NewsViewModel>()
                };
            }

            var news = _context.News
                 .Where(x => !x.IsDelete && x.Id != 0)
                 .Where(x => searchRequest.Type == null || x.Type == searchRequest.Type)
                                          .Where(x => string.IsNullOrEmpty(searchRequest.SearchText) ||
                                                     (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(searchRequest.SearchText.ToLower())))
                                          .Where(x => x.CategoryId == category.Id)

                                          .Select(x => new NewsViewModel
                                          {
                                              Id = x.Id,
                                              Title = x.Title,
                                              ShortContent = x.ShortContent,
                                              // Content = x.Content,
                                              CreateAt = x.CreatedAt,
                                              Type = x.Type,
                                              Images = x.Image.Deserialize<List<FileDetailModel>>(),
                                              CategoryId = x.CategoryId,
                                              TitleEnglish = x.TitleEnglish,
                                              ShortContentEnglish = x.ShortContentEnglish,
                                              ContentEnglish = x.ContentEnglish,
                                              TitleKorean = x.TitleKorean,
                                              ShortContentKorean = x.ShortContentKorean,
                                              ContentKorean = x.ContentKorean,

                                          });

            return new PagingResult<NewsViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = await news.CountAsync(),
                Data = await news.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize).ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<NewsViewModel>()
            {
                CurrentPage = searchRequest.Page,
                PageSize = searchRequest.PageSize,
                TotalItems = 0,
                Data = new List<NewsViewModel>()
            };
        }
    }
    private async Task<string> ProcessBase64Images(string content, string folder)
    {
        if (string.IsNullOrEmpty(content))
            return content;

        // Sửa regex để nhận dạng base64 chính xác
        var matches = Regex.Matches(content, @"data:image\/[a-zA-Z]+;base64,[^\s\""]+");

        var replacements = new Dictionary<string, string>();

        foreach (Match match in matches)
        {
            string base64String = match.Value;

            if (!replacements.ContainsKey(base64String)) // Tránh upload trùng ảnh
            {
                string fileName = $"{Guid.NewGuid()}.jpg";
                string imageUrl = await _fileService.UploadBase64(base64String, folder, fileName);
                replacements[base64String] = imageUrl;
            }
        }

        // Dùng StringBuilder để thay thế nhanh hơn
        var sb = new StringBuilder(content);
        foreach (var kvp in replacements)
        {
            sb.Replace(kvp.Key, kvp.Value);
        }

        return sb.ToString();
    }


    public async Task CreateOrUpdate(NewsViewSetupModel request)
    {
        var news = await _context.News.Where(x => x.Id == request.Id && !x.IsDelete).FirstOrDefaultAsync();
        if (news == null)
        {
            news = new News
            {
                CreatedAt = DateTime.Now
            };
        }

        // Xử lý Content & thay ảnh base64 bằng URL ảnh đã upload
        news.Content = await ProcessBase64Images(request.Content, "News");
        news.ContentEnglish = await ProcessBase64Images(request.ContentEnglish, "NewsEnglish");
        news.ContentKorean = await ProcessBase64Images(request.ContentKorean, "NewsKorean");

        // Cập nhật thông tin khác
        news.Title = request.Title;
        news.ShortContent = request.ShortContent;
        news.UpdatedAt = DateTime.Now;
        news.Type = request.Type;
        news.CategoryId = request.CategoryId;
        news.Image = await ProcessUploadedFiles(request.File, request.UploadedFiles, "News");
        news.ImageEnglish = await ProcessUploadedFiles(request.FileEnglish, request.UploadedFilesEnglish, "NewsEnglish");
        news.ImageKorean = await ProcessUploadedFiles(request.FileKorean, request.UploadedFilesKorean, "NewsKorean");
        news.TitleEnglish = request.TitleEnglish;
        news.ShortContentEnglish = request.ShortContentEnglish;
        news.TitleKorean = request.TitleKorean;
        news.ShortContentKorean = request.ShortContentKorean;
        news.Author = request.Author;
        news.PublishDate = request.PublishDate;
        if (news.Id > 0)
            _context.News.Update(news);
        else
            await _context.News.AddAsync(news);
        await _context.SaveChangesAsync();
    }
    private async Task<string> ProcessUploadedFiles(IEnumerable<IFormFile> files, List<FileDetailModel> existingFiles, string folder)
    {
        var fileList = existingFiles ?? new List<FileDetailModel>();

        if (files != null && files.Any())
        {
            foreach (var file in files)
            {
                var fileUrl = _fileService.Upload(file, folder, file.FileName);
                fileList.Add(new FileDetailModel
                {
                    FileName = file.FileName,
                    FileUrl = fileUrl
                });
            }
        }

        return JsonConvert.SerializeObject(fileList);
    }


}