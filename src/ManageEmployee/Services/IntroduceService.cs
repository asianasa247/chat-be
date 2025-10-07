using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.IntroduceEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Introduces;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class IntroduceService : IIntroduceService
{
    private readonly ApplicationDbContext _context;

    public IntroduceService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Create(IntroduceModel request)
    {
        var exist = await _context.Introduces.AnyAsync(
            x => x.Title == request.Title && x.Type == request.Type && !x.IsDelete);
        if (exist)
        {
            throw new ErrorException(ErrorMessages.CategoryNameAlreadyExist);
        }

        var entity = new Introduce();
        entity.Type = request.Type;
        entity.Title = request.Title;
        entity.Name = request.Name;
        entity.Content = request.Content;
        entity.Summary = request.Summary;
        entity.IframeYoutube = request.IframeYoutube;
        entity.IntroduceTypeId = request.IntroduceTypeId;
        entity.CreatedAt = DateTime.Now;
        entity.UpdatedAt = DateTime.Now;
        entity.TitleEnglish = request.TitleEnglish;
        entity.ContentEnglish = request.ContentEnglish;
        entity.ShortContentEnglish = request.ShortContentEnglish;
        entity.IframeYoutubeEnglish = request.IframeYoutubeEnglish;
        entity.TitleKorean = request.TitleKorean;
        entity.ContentKorean = request.ContentKorean;
        entity.ShortContentKorean = request.ShortContentKorean;
        entity.IframeYoutubeKorean = request.IframeYoutubeKorean;
        _context.Introduces.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var itemDelete = await _context.Introduces.Where(x => x.Id == id && !x.IsDelete).FirstOrDefaultAsync();
        if (itemDelete != null)
        {
            itemDelete.IsDelete = true;
            itemDelete.DeleteAt = DateTime.Now;
            _context.Introduces.Update(itemDelete);
            _context.SaveChanges();
        }
    }

    public async Task<IEnumerable<IntroduceModel>> GetAll()
    {
        var data = await _context.Introduces.Where(x => !x.IsDelete).Select(x => new IntroduceModel
        {
            Id = x.Id,
            Title = x.Title,
            Type = x.Type,
            IframeYoutube = x.IframeYoutube,
            IntroduceTypeId = x.IntroduceTypeId,
            Name = x.Name,
            TitleEnglish = x.TitleEnglish,
            Content = x.Content,
            ContentEnglish = x.ContentEnglish,
            ShortContentEnglish = x.ShortContentEnglish,
            IframeYoutubeEnglish = x.IframeYoutubeEnglish,
            TitleKorean = x.TitleKorean,
            ContentKorean = x.ContentKorean,
            ShortContentKorean = x.ShortContentKorean,
            IframeYoutubeKorean = x.IframeYoutubeKorean,
        }).ToListAsync();
        return data;
    }

    public async Task<PagingResult<IntroduceModel>> GetAll(PagingRequestTypeModel param)
    {
        try
        {
            if (param.PageSize <= 0)
                param.PageSize = 20;

            if (param.Page < 0)
                param.Page = 1;

            var introduces = _context.Introduces.Where(x => x.IsDelete != true && x.Id != 0)
                                         .Where(x => string.IsNullOrEmpty(param.SearchText) || (!string.IsNullOrEmpty(x.Title) && x.Title.ToLower().Contains(param.SearchText.ToLower())))
                                         .Where(x => param.Type == null || (param.Type != null && (int)x.Type == param.Type))
                                         .Select(x => new IntroduceModel
                                         {
                                             Id = x.Id,
                                             Title = x.Title,
                                             Type = x.Type, 
                                             IframeYoutube = x.IframeYoutube,
                                             CreateAt = x.CreatedAt,
                                             IntroduceTypeId = x.IntroduceTypeId,
                                             Name = x.Name,
                                             TitleEnglish = x.TitleEnglish,
                                             ContentEnglish = x.ContentEnglish,
                                             ShortContentEnglish = x.ShortContentEnglish,
                                             IframeYoutubeEnglish = x.IframeYoutubeEnglish,
                                             TitleKorean = x.TitleKorean,
                                             ContentKorean = x.ContentKorean,
                                             ShortContentKorean = x.ShortContentKorean,
                                             IframeYoutubeKorean = x.IframeYoutubeKorean,
                                         });

            return new PagingResult<IntroduceModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = await introduces.CountAsync(),
                Data = await introduces.Skip((param.Page) * param.PageSize).Take(param.PageSize).ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<IntroduceModel>()
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<IntroduceModel>()
            };
        }
    }

    public async Task<IntroduceModel> GetById(int id)
    {
        var itemData = await _context.Introduces.Where(x => x.Id == id && x.IsDelete != true).FirstOrDefaultAsync();
        if (itemData is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        return new IntroduceModel
        {
            Id = itemData.Id,
            Title = itemData.Title,
            Content = itemData.Content,
            IframeYoutube = itemData.IframeYoutube,
            Type = itemData.Type,
            CreateAt = itemData.CreatedAt,
            IntroduceTypeId = itemData.IntroduceTypeId,
            Name = itemData.Name,
            Summary = itemData.Summary,
            TitleEnglish = itemData.TitleEnglish,
            ContentEnglish = itemData.ContentEnglish,
            ShortContentEnglish = itemData.ShortContentEnglish,
            IframeYoutubeEnglish = itemData.IframeYoutubeEnglish,
            TitleKorean = itemData.TitleKorean,
            ContentKorean = itemData.ContentKorean,
            ShortContentKorean = itemData.ShortContentKorean,
            IframeYoutubeKorean = itemData.IframeYoutubeKorean,
        };
    }

    public async Task Update(IntroduceModel request)
    {
        var itemUpdate = await _context.Introduces.Where(x => x.Id == Convert.ToInt32(request.Id) && x.IsDelete != true).FirstOrDefaultAsync();
        if (itemUpdate == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        var checkItemUpdate = await _context.Introduces.Where(x => x.Title.ToLower() == request.Title.ToLower() && x.Type == request.Type && x.IsDelete != true).FirstOrDefaultAsync();
        if (checkItemUpdate != null && checkItemUpdate.Id != itemUpdate.Id)
        {
            throw new ErrorException(ErrorMessages.NameAlreadyExist);
        }
        itemUpdate.Title = request.Title;
        itemUpdate.Type = request.Type;
        itemUpdate.Content = request.Content;
        itemUpdate.IframeYoutube = request.IframeYoutube;
        itemUpdate.UpdatedAt = DateTime.Now;
        itemUpdate.IntroduceTypeId = request.IntroduceTypeId;
        itemUpdate.Name = request.Name;
        itemUpdate.Summary = request.Summary;
        itemUpdate.UpdatedAt = DateTime.Now;
        itemUpdate.TitleEnglish = request.TitleEnglish;
        itemUpdate.ContentEnglish = request.ContentEnglish;
        itemUpdate.ShortContentEnglish = request.ShortContentEnglish;
        itemUpdate.IframeYoutubeEnglish = request.IframeYoutubeEnglish;
        itemUpdate.TitleKorean = request.TitleKorean;
        itemUpdate.ContentKorean = request.ContentKorean;
        itemUpdate.ShortContentKorean = request.ShortContentKorean;
        itemUpdate.IframeYoutubeKorean = request.IframeYoutubeKorean;
        _context.Introduces.Update(itemUpdate);
        await _context.SaveChangesAsync();
    }
}