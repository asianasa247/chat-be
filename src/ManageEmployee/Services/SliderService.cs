using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Sliders;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services;

public class SliderService : ISliderService
{
    private readonly ApplicationDbContext _context;

    public SliderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Create(SliderModel request)
    {
        var entity = new Slider();
        MappingModelToEntity(entity, request);
        _context.Sliders.Add(entity);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    /// <summary>
    /// Đồng bộ ảnh: BE chỉ lưu 1 cột Img trong DB.
    /// ImgMobile ở response sẽ lấy cùng giá trị Img.
    /// </summary>
    private static void MappingModelToEntity(Slider entity, SliderModel model)
    {
        // Ưu tiên model.Img; nếu null thì dùng model.ImgMobile
        var unifiedImage = model.Img ?? model.ImgMobile;
        if (!string.IsNullOrWhiteSpace(unifiedImage))
        {
            entity.Img = unifiedImage;
            // entity.ImgMobile là NotMapped nên gán hay không đều không ảnh hưởng DB.
            entity.ImgMobile = unifiedImage;
        }

        entity.Name = model.Name;
        entity.Type = model.Type;
        entity.IsVideo = model.IsVideo;
        entity.AdsensePosition = model.AdsensePosition;

        if (entity.Id <= 0)
        {
            entity.CreatedAt = DateTime.Now;
        }
        entity.UpdatedAt = DateTime.Now;
        entity.IsSizeImage = model.IsSizeImage;
    }

    public async Task Delete(int id)
    {
        var itemDelete = await _context.Sliders.FindAsync(id);
        if (itemDelete != null)
        {
            itemDelete.IsDelete = true;
            itemDelete.DeleteAt = DateTime.Now;
            _context.Sliders.Update(itemDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<SliderModel>> GetAll()
    {
        // LƯU Ý: không truy cập x.ImgMobile để tránh EF sinh cột không tồn tại
        return await _context.Sliders
            .Where(x => !x.IsDelete)
            .Select(x => new SliderModel
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type,
                Img = x.Img,
                ImgMobile = x.Img, // luôn mirror
                CreateAt = x.CreatedAt,
                AdsensePosition = x.AdsensePosition,
                IsSizeImage = x.IsSizeImage,
                IsVideo = x.IsVideo,
            }).ToListAsync();
    }

    public async Task<PagingResult<SliderModel>> GetAll(SlideRequestModel param)
    {
        try
        {
            if (param.PageSize <= 0) param.PageSize = 20;
            if (param.Page < 1) param.Page = 1;

            var query = _context.Sliders
                .Where(x => !x.IsDelete && x.Id != 0)
                .Where(x => string.IsNullOrEmpty(param.SearchText) ||
                            (!string.IsNullOrEmpty(x.Name) && x.Name.ToLower().Contains(param.SearchText.ToLower())))
                .Where(x => param.Type == null || (int)x.Type == param.Type)
                .Where(x => param.AdsensePosition == null || x.AdsensePosition == param.AdsensePosition)
                .Select(x => new SliderModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type,
                    Img = x.Img,
                    ImgMobile = x.Img, // mirror
                    CreateAt = x.CreatedAt,
                    AdsensePosition = x.AdsensePosition,
                    IsSizeImage = x.IsSizeImage,
                    IsVideo = x.IsVideo,
                });

            return new PagingResult<SliderModel>
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = await query.CountAsync(),
                Data = await query.Skip((param.Page - 1) * param.PageSize)
                                  .Take(param.PageSize)
                                  .ToListAsync()
            };
        }
        catch
        {
            return new PagingResult<SliderModel>
            {
                CurrentPage = param.Page,
                PageSize = param.PageSize,
                TotalItems = 0,
                Data = new List<SliderModel>()
            };
        }
    }

    public async Task<SliderModel> GetById(int id)
    {
        var itemData = await _context.Sliders
            .Where(x => x.Id == id && !x.IsDelete)
            .FirstOrDefaultAsync();

        if (itemData == null)
            throw new NotImplementedException();

        return new SliderModel
        {
            Id = itemData.Id,
            Name = itemData.Name,
            Type = itemData.Type,
            CreateAt = itemData.CreatedAt,
            Img = itemData.Img,
            ImgMobile = itemData.Img, // mirror
            AdsensePosition = itemData.AdsensePosition,
            IsSizeImage = itemData.IsSizeImage,
            IsVideo = itemData.IsVideo,
        };
    }

    public async Task<string> Update(SliderModel request)
    {
        var itemUpdate = await _context.Sliders
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDelete);

        if (itemUpdate == null)
            return ErrorMessages.DataNotFound;

        MappingModelToEntity(itemUpdate, request);
        _context.Sliders.Update(itemUpdate);
        await _context.SaveChangesAsync();
        return string.Empty;
    }
}
