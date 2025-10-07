using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Events;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services;

public class EventWithImageService : IEventWithImageService
{
    private readonly ApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public EventWithImageService(IFileService fileService, ApplicationDbContext context, IMapper mapper)
    {
        _fileService = fileService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResult<EventWithImagePagingGetterModel>> GetAll(PagingRequestModel param)
    {
        var query = _context.EventWithImages
            .Where(x => string.IsNullOrEmpty(param.SearchText) || x.Name.ToLower().Contains(param.SearchText.Trim().ToLower()) ||
                                    x.Note.ToLower().Contains(param.SearchText.Trim().ToLower()));

        if (!string.IsNullOrEmpty(param.SortField))
        {
            param.SortField = param.SortField.ToLower();
            switch (param.SortField)
            {
                case "id":
                    query = param.isSort ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                    break;
                case "name":
                    query = param.isSort ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                    break;
                case "order":
                    query = param.isSort ? query.OrderBy(x => x.Order) : query.OrderByDescending(x => x.Order);
                    break;
            }
        }

        var data = await query.Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var listOut = new List<EventWithImagePagingGetterModel>();
        foreach (var item in data)
        {
            var itemOut = new EventWithImagePagingGetterModel()
            {
                Id = item.Id,
                Note = item.Note,
                Name = item.Name,
                LinkDriver = item.LinkDriver,
                Date = item.Date,
                Order = item.Order,
                Files = string.IsNullOrEmpty(item.FileLink) ? new List<FileDetailModel>() : JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileLink)
            };
            listOut.Add(itemOut);
        }

        return new PagingResult<EventWithImagePagingGetterModel>
        {
            Data = listOut,
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await query.CountAsync()
        };
    }

    public async Task<EventWithImageDetailGetterModel> GetById(int id)
    {
        var item = await _context.EventWithImages.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);
        var itemOut = _mapper.Map<EventWithImageDetailGetterModel>(item);
        if(item.FileLink != null)
        {
            itemOut.Files = JsonConvert.DeserializeObject<List<FileDetailModel>>(item.FileLink);
        }

        return itemOut;
    }

    public async Task Create(EventWithImageModel param)
    {
        var data = new EventWithImage();
        if (param.Files != null && param.Files.Any())
        {
            var files = new List<FileDetailModel>();
            foreach (var file in param.Files)
            {
                var fileUrl = _fileService.Upload(file, "EventWithImage", file.FileName);
                files.Add(new FileDetailModel
                {
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                });
            }
            data.FileLink = JsonConvert.SerializeObject(files).ToString();
        }
        data.Name = param.Name;
        data.Order = param.Order;
        data.Note = param.Note;
        data.Date = param.Date;
        data.LinkDriver = param.LinkDriver;
        _context.EventWithImages.Add(data);
        await _context.SaveChangesAsync();
    }

    public async Task Update(EventWithImageModel param)
    {
        var data = await _context.EventWithImages.FindAsync(param.Id);
        if (data == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        data.FileLink = "";
        var files = JsonConvert.DeserializeObject<List<FileDetailModel>>(param.FileStored);
        if (param.Files != null && param.Files.Any())
        {
            foreach (var file in param.Files)
            {
                var fileUrl = _fileService.Upload(file, "EventWithImage", file.FileName);
                files.Add(new FileDetailModel
                {
                    FileName = file.FileName,
                    FileUrl = fileUrl,
                });
            }
        }
        data.FileLink = JsonConvert.SerializeObject(files).ToString();

        data.Name = param.Name;
        data.Order = param.Order;
        data.Note = param.Note;
        data.Date = param.Date;
        data.LinkDriver = param.LinkDriver;
        _context.EventWithImages.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var data = await _context.EventWithImages.FindAsync(id);
        if (data != null)
        {
            _context.EventWithImages.Remove(data);
            await _context.SaveChangesAsync();
        }
    }
}