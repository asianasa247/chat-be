using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.WebEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Web;
public class WebSocialService : IWebSocialService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;

    public WebSocialService(ApplicationDbContext context, IFileService fileService)
    {
        _context = context;
        _fileService = fileService;
    }

    public async Task Create(SocialViewModel request)
    {
        var entity = new Social();
        if (request.File != null)
        {
            var fileName = _fileService.Upload(request.File, "WebSocial");
            entity.FileUrl = fileName;
            entity.FileName = request.File.FileName;
        }
        entity.Title = request.Title;
        entity.ShortContent = request.ShortContent;
        entity.Image = request.Image;
        entity.Content = request.Content;
        entity.UpdatedAt = DateTime.Now;
        entity.CreatedAt = DateTime.Now;
        entity.Type = request.Type;
        await _context.Social.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.Social.FindAsync(id);
        if(data != null)
        {
            _context.Social.Remove(data);
            await _context.SaveChangesAsync();
        }    
    }

    public async Task<IEnumerable<SocialGetListModel>> GetAll()
    {
        var data = await _context.Social.Where(x => !x.IsDelete).Select(x => new SocialGetListModel
        {
            Id = x.Id,
            Title = x.Title,
            ShortContent = x.ShortContent,
            Content = x.Content,
            FileUrl = x.FileUrl,
        }).ToListAsync();
        return data;
    }

    public async Task<SocialViewModel> GetById(int id)
    {
        try
        {
            var itemData = await _context.Social.Where(x => x.Id == id && !x.IsDelete).FirstOrDefaultAsync();
            if (itemData != null)
            {
                return new SocialViewModel
                {
                    Id = itemData.Id,
                    Title = itemData.Title,
                    ShortContent = itemData.ShortContent,
                    Image = itemData.Image,
                    Content = itemData.Content,
                    CreateAt = itemData.CreatedAt,
                    Type = itemData.Type,
                    FileUrl = itemData.FileUrl
                };
            }
            else
            {
                return null;
            }
        }
        catch
        {
            throw new NotImplementedException();
        }
    }

    public async Task<PagingResult<SocialViewModel>> SearchNews(PagingRequestModel searchRequest)
    {
        if (searchRequest.Page < 1)
            searchRequest.Page = 1;
         
        var query = _context.Social.Where(x => string.IsNullOrEmpty(searchRequest.SearchText)
                                    || x.Title.Contains(searchRequest.SearchText)
                                    || x.Content.Contains(searchRequest.SearchText)
                                    || x.ShortContent.Contains(x.ShortContent));
        var data = await query.Skip((searchRequest.Page - 1) * searchRequest.PageSize).Take(searchRequest.PageSize)
            .Select(x => new SocialViewModel
            {
                Id = x.Id,
                Title=x.Title,
                Content = x.Content,
                ShortContent=x.ShortContent,
                Type = x.Type,
                Image = x.Image,
                FileUrl = x.FileUrl
            }).ToListAsync();
        return new PagingResult<SocialViewModel>
        {
            PageSize = searchRequest.PageSize,
            CurrentPage = searchRequest.Page,
            TotalItems = await query.CountAsync(),
            Data = data
        };
    }

    public async Task Update(SocialViewModel request)
    {
        var itemUpdate = await _context.Social.Where(x => x.Id == request.Id && !x.IsDelete).FirstOrDefaultAsync();
        if (itemUpdate == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        if (request.File != null)
        {
            var fileName = _fileService.Upload(request.File, "WebSocial");
            itemUpdate.FileUrl = fileName;
            itemUpdate.FileName = request.File.FileName;
        }
        itemUpdate.Title = request.Title;
        itemUpdate.ShortContent = request.ShortContent;
        itemUpdate.Image = request.Image;
        itemUpdate.Content = request.Content;
        itemUpdate.UpdatedAt = DateTime.Now;
        itemUpdate.Type = request.Type;
        _context.Social.Update(itemUpdate);
        await _context.SaveChangesAsync();
    }
}
